using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechMoveGLMS.Models;
using TechMoveGLMS.Repositories.Interfaces;
using TechMoveGLMS.Services.Interfaces;
using TechMoveGLMS.ViewModels;

namespace TechMoveGLMS.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IContractRepository _contractRepository;
        private readonly IServiceRequestService _serviceRequestService;
        private readonly ICurrencyService _currencyService;

        public ServiceRequestsController(
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

        public async Task<IActionResult> Index()
        {
            var serviceRequests = await _serviceRequestRepository.GetServiceRequestsWithContractAsync();
            return View(serviceRequests);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var serviceRequest = await _serviceRequestRepository.GetServiceRequestWithContractByIdAsync(id.Value);
            if (serviceRequest == null)
                return NotFound();

            return View(serviceRequest);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new ServiceRequestFormViewModel
            {
                Contracts = await GetContractSelectListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequestFormViewModel viewModel)
        {
            var contract = await _contractRepository.GetByIdAsync(viewModel.ContractId);

            if (!_serviceRequestService.CanCreateServiceRequest(contract))
            {
                ModelState.AddModelError("ContractId", "A Service Request cannot be created for a contract that is Expired or OnHold.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    decimal exchangeRate = await _currencyService.GetUsdToZarRateAsync();
                    decimal convertedZar = _currencyService.ConvertUsdToZar(viewModel.CostInUSD, exchangeRate);

                    var serviceRequest = new ServiceRequest
                    {
                        ContractId = viewModel.ContractId,
                        Description = viewModel.Description,
                        CostInUSD = viewModel.CostInUSD,
                        CostInZAR = convertedZar,
                        Status = viewModel.Status
                    };

                    await _serviceRequestRepository.AddAsync(serviceRequest);
                    await _serviceRequestRepository.SaveAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    ModelState.AddModelError("", "Unable to retrieve exchange rate at this time. Please try again.");
                }
            }

            viewModel.Contracts = await GetContractSelectListAsync();
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(id.Value);
            if (serviceRequest == null)
                return NotFound();

            var viewModel = new ServiceRequestFormViewModel
            {
                ServiceRequestId = serviceRequest.ServiceRequestId,
                ContractId = serviceRequest.ContractId,
                Description = serviceRequest.Description,
                CostInUSD = serviceRequest.CostInUSD,
                CostInZAR = serviceRequest.CostInZAR,
                Status = serviceRequest.Status,
                Contracts = await GetContractSelectListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ServiceRequestFormViewModel viewModel)
        {
            var contract = await _contractRepository.GetByIdAsync(viewModel.ContractId);

            if (!_serviceRequestService.CanCreateServiceRequest(contract))
            {
                ModelState.AddModelError("ContractId", "A Service Request cannot be linked to a contract that is Expired or OnHold.");
            }

            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(viewModel.ServiceRequestId);
            if (serviceRequest == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    decimal exchangeRate = await _currencyService.GetUsdToZarRateAsync();
                    decimal convertedZar = _currencyService.ConvertUsdToZar(viewModel.CostInUSD, exchangeRate);

                    serviceRequest.ContractId = viewModel.ContractId;
                    serviceRequest.Description = viewModel.Description;
                    serviceRequest.CostInUSD = viewModel.CostInUSD;
                    serviceRequest.CostInZAR = convertedZar;
                    serviceRequest.Status = viewModel.Status;

                    _serviceRequestRepository.Update(serviceRequest);
                    await _serviceRequestRepository.SaveAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    ModelState.AddModelError("", "Unable to retrieve exchange rate at this time. Please try again.");
                }
            }

            viewModel.Contracts = await GetContractSelectListAsync();
            return View(viewModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var serviceRequest = await _serviceRequestRepository.GetServiceRequestWithContractByIdAsync(id.Value);
            if (serviceRequest == null)
                return NotFound();

            return View(serviceRequest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(id);
            if (serviceRequest != null)
            {
                _serviceRequestRepository.Delete(serviceRequest);
                await _serviceRequestRepository.SaveAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<IEnumerable<SelectListItem>> GetContractSelectListAsync()
        {
            var contracts = await _contractRepository.GetContractsWithClientAsync();

            return contracts.Select(c => new SelectListItem
            {
                Value = c.ContractId.ToString(),
                Text = $"Contract #{c.ContractId} - {c.Client?.Name} ({c.Status})"
            });
        }
    }
}