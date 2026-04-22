using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechMoveGLMS.Models;
using TechMoveGLMS.Repositories.Interfaces;
using TechMoveGLMS.Services.Interfaces;
using TechMoveGLMS.ViewModels;

namespace TechMoveGLMS.Controllers
{
    public class ContractsController : Controller
    {
        private readonly IContractRepository _contractRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileValidationService _fileValidationService;

        public ContractsController(
            IContractRepository contractRepository,
            IClientRepository clientRepository,
            IWebHostEnvironment webHostEnvironment,
            IFileValidationService fileValidationService)
        {
            _contractRepository = contractRepository;
            _clientRepository = clientRepository;
            _webHostEnvironment = webHostEnvironment;
            _fileValidationService = fileValidationService;
        }

        public async Task<IActionResult> Index(DateTime? startDateFrom, DateTime? endDateTo, ContractStatus? status)
        {
            var filteredContracts = await _contractRepository.FilterContractsAsync(startDateFrom, endDateTo, status);

            var viewModel = new ContractFilterViewModel
            {
                StartDateFrom = startDateFrom,
                EndDateTo = endDateTo,
                Status = status,
                Contracts = filteredContracts
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var contract = await _contractRepository.GetContractWithClientByIdAsync(id.Value);
            if (contract == null)
                return NotFound();

            return View(contract);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new ContractFormViewModel
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(1),
                Clients = await GetClientSelectListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContractFormViewModel viewModel)
        {
            if (viewModel.EndDate < viewModel.StartDate)
            {
                ModelState.AddModelError("EndDate", "End Date cannot be earlier than Start Date.");
            }

            string? uniqueFileName = null;
            string? relativeFilePath = null;

            if (viewModel.SignedAgreementFile != null)
            {
                if (!_fileValidationService.IsPdfFile(viewModel.SignedAgreementFile.FileName))
                {
                    ModelState.AddModelError("SignedAgreementFile", "Only PDF files are allowed.");
                }
            }

            if (ModelState.IsValid)
            {
                if (viewModel.SignedAgreementFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);

                    uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.SignedAgreementFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.SignedAgreementFile.CopyToAsync(fileStream);
                    }

                    relativeFilePath = "/uploads/" + uniqueFileName;
                }

                var contract = new Contract
                {
                    ClientId = viewModel.ClientId,
                    StartDate = viewModel.StartDate,
                    EndDate = viewModel.EndDate,
                    Status = viewModel.Status,
                    ServiceLevel = viewModel.ServiceLevel,
                    SignedAgreementFileName = uniqueFileName,
                    SignedAgreementFilePath = relativeFilePath
                };

                await _contractRepository.AddAsync(contract);
                await _contractRepository.SaveAsync();

                return RedirectToAction(nameof(Index));
            }

            viewModel.Clients = await GetClientSelectListAsync();
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var contract = await _contractRepository.GetByIdAsync(id.Value);
            if (contract == null)
                return NotFound();

            var viewModel = new ContractFormViewModel
            {
                ContractId = contract.ContractId,
                ClientId = contract.ClientId,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                Status = contract.Status,
                ServiceLevel = contract.ServiceLevel,
                ExistingFileName = contract.SignedAgreementFileName,
                ExistingFilePath = contract.SignedAgreementFilePath,
                Clients = await GetClientSelectListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ContractFormViewModel viewModel)
        {
            if (id != viewModel.ContractId)
                return NotFound();

            if (viewModel.EndDate < viewModel.StartDate)
            {
                ModelState.AddModelError("EndDate", "End Date cannot be earlier than Start Date.");
            }

            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
                return NotFound();

            if (viewModel.SignedAgreementFile != null)
            {
                if (!_fileValidationService.IsPdfFile(viewModel.SignedAgreementFile.FileName))
                {
                    ModelState.AddModelError("SignedAgreementFile", "Only PDF files are allowed.");
                }
            }

            if (ModelState.IsValid)
            {
                if (viewModel.SignedAgreementFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.SignedAgreementFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.SignedAgreementFile.CopyToAsync(fileStream);
                    }

                    contract.SignedAgreementFileName = uniqueFileName;
                    contract.SignedAgreementFilePath = "/uploads/" + uniqueFileName;
                }

                contract.ClientId = viewModel.ClientId;
                contract.StartDate = viewModel.StartDate;
                contract.EndDate = viewModel.EndDate;
                contract.Status = viewModel.Status;
                contract.ServiceLevel = viewModel.ServiceLevel;

                _contractRepository.Update(contract);
                await _contractRepository.SaveAsync();

                return RedirectToAction(nameof(Index));
            }

            viewModel.Clients = await GetClientSelectListAsync();
            return View(viewModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var contract = await _contractRepository.GetContractWithClientByIdAsync(id.Value);
            if (contract == null)
                return NotFound();

            return View(contract);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract != null)
            {
                _contractRepository.Delete(contract);
                await _contractRepository.SaveAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DownloadAgreement(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null || string.IsNullOrEmpty(contract.SignedAgreementFilePath))
                return NotFound();

            string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, contract.SignedAgreementFilePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (!System.IO.File.Exists(fullPath))
                return NotFound();

            return PhysicalFile(fullPath, "application/pdf", contract.SignedAgreementFileName ?? "agreement.pdf");
        }

        private async Task<IEnumerable<SelectListItem>> GetClientSelectListAsync()
        {
            var clients = await _clientRepository.GetAllAsync();

            return clients.Select(c => new SelectListItem
            {
                Value = c.ClientId.ToString(),
                Text = c.Name
            });
        }
    }
}