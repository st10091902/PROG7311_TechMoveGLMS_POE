using Microsoft.AspNetCore.Mvc;
using TechMoveGLMS.Models;
using TechMoveGLMS.Repositories.Interfaces;

namespace TechMoveGLMS.Api.Controllers
{
    [Route("api/contracts")]
    [ApiController]
    public class ContractsApiController : ControllerBase
    {
        private readonly IContractRepository _contractRepository;

        public ContractsApiController(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contract>>> GetContracts(
            DateTime? startDateFrom,
            DateTime? endDateTo,
            ContractStatus? status)
        {
            var contracts = await _contractRepository.FilterContractsAsync(startDateFrom, endDateTo, status);
            return Ok(contracts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contract>> GetContract(int id)
        {
            var contract = await _contractRepository.GetContractWithClientByIdAsync(id);

            if (contract == null)
                return NotFound();

            return Ok(contract);
        }

        [HttpPost]
        public async Task<ActionResult<Contract>> CreateContract(Contract contract)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _contractRepository.AddAsync(contract);
            await _contractRepository.SaveAsync();

            return CreatedAtAction(nameof(GetContract), new { id = contract.ContractId }, contract);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContract(int id, Contract contract)
        {
            if (id != contract.ContractId)
                return BadRequest();

            var existingContract = await _contractRepository.GetByIdAsync(id);

            if (existingContract == null)
                return NotFound();

            existingContract.ClientId = contract.ClientId;
            existingContract.StartDate = contract.StartDate;
            existingContract.EndDate = contract.EndDate;
            existingContract.Status = contract.Status;
            existingContract.ServiceLevel = contract.ServiceLevel;
            existingContract.SignedAgreementFileName = contract.SignedAgreementFileName;
            existingContract.SignedAgreementFilePath = contract.SignedAgreementFilePath;

            _contractRepository.Update(existingContract);
            await _contractRepository.SaveAsync();

            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateContractStatus(int id, [FromBody] ContractStatus status)
        {
            var contract = await _contractRepository.GetByIdAsync(id);

            if (contract == null)
                return NotFound();

            contract.Status = status;

            _contractRepository.Update(contract);
            await _contractRepository.SaveAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContract(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);

            if (contract == null)
                return NotFound();

            _contractRepository.Delete(contract);
            await _contractRepository.SaveAsync();

            return NoContent();
        }
    }
}