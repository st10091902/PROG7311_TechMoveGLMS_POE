using Microsoft.AspNetCore.Mvc;
using TechMoveGLMS.Models;
using TechMoveGLMS.Repositories.Interfaces;
using TechMoveGLMS.Services.Interfaces;

namespace TechMoveGLMS.Api.Controllers
{
    [Route("api/service-requests")]
    [ApiController]
    public class ServiceRequestsApiController : ControllerBase
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IContractRepository _contractRepository;
        private readonly IServiceRequestService _serviceRequestService;
        private readonly ICurrencyService _currencyService;

        public ServiceRequestsApiController(
            IServiceRequestRepository serviceRequestRepository,
            IContractRepository contractRepository,
            IServiceRequestService serviceRequestService,
            ICurrencyService currencyService)
        {
            _serviceRequestRepository = serviceRequestRepository;
            _contractRepository = contractRepository;
            _serviceRequestService = serviceRequestService;
            _currencyService = currencyService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceRequest>>> GetServiceRequests()
        {
            var serviceRequests = await _serviceRequestRepository.GetServiceRequestsWithContractAsync();
            return Ok(serviceRequests);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceRequest>> GetServiceRequest(int id)
        {
            var serviceRequest = await _serviceRequestRepository.GetServiceRequestWithContractByIdAsync(id);

            if (serviceRequest == null)
                return NotFound();

            return Ok(serviceRequest);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceRequest>> CreateServiceRequest(ServiceRequest serviceRequest)
        {
            var contract = await _contractRepository.GetByIdAsync(serviceRequest.ContractId);

            if (!_serviceRequestService.CanCreateServiceRequest(contract))
            {
                return BadRequest("A Service Request cannot be created for a contract that is Expired or OnHold.");
            }

            decimal exchangeRate = await _currencyService.GetUsdToZarRateAsync();
            serviceRequest.CostInZAR = _currencyService.ConvertUsdToZar(serviceRequest.CostInUSD, exchangeRate);

            await _serviceRequestRepository.AddAsync(serviceRequest);
            await _serviceRequestRepository.SaveAsync();

            return CreatedAtAction(nameof(GetServiceRequest), new { id = serviceRequest.ServiceRequestId }, serviceRequest);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServiceRequest(int id, ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.ServiceRequestId)
                return BadRequest();

            var existingServiceRequest = await _serviceRequestRepository.GetByIdAsync(id);

            if (existingServiceRequest == null)
                return NotFound();

            var contract = await _contractRepository.GetByIdAsync(serviceRequest.ContractId);

            if (!_serviceRequestService.CanCreateServiceRequest(contract))
            {
                return BadRequest("A Service Request cannot be linked to a contract that is Expired or OnHold.");
            }

            decimal exchangeRate = await _currencyService.GetUsdToZarRateAsync();

            existingServiceRequest.ContractId = serviceRequest.ContractId;
            existingServiceRequest.Description = serviceRequest.Description;
            existingServiceRequest.CostInUSD = serviceRequest.CostInUSD;
            existingServiceRequest.CostInZAR = _currencyService.ConvertUsdToZar(serviceRequest.CostInUSD, exchangeRate);
            existingServiceRequest.Status = serviceRequest.Status;

            _serviceRequestRepository.Update(existingServiceRequest);
            await _serviceRequestRepository.SaveAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceRequest(int id)
        {
            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(id);

            if (serviceRequest == null)
                return NotFound();

            _serviceRequestRepository.Delete(serviceRequest);
            await _serviceRequestRepository.SaveAsync();

            return NoContent();
        }
    }
}