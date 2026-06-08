using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Json;
using TechMoveGLMS.Models;
using TechMoveGLMS.Services.Interfaces;
using TechMoveGLMS.ViewModels;

namespace TechMoveGLMS.Controllers
{
    public class ContractsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileValidationService _fileValidationService;

        public ContractsController(
            IHttpClientFactory httpClientFactory,
            IWebHostEnvironment webHostEnvironment,
            IFileValidationService fileValidationService)
        {
            _httpClientFactory = httpClientFactory;
            _webHostEnvironment = webHostEnvironment;
            _fileValidationService = fileValidationService;
        }

        private HttpClient CreateApiClient()
        {
            return _httpClientFactory.CreateClient("TechMoveApi");
        }

        public async Task<IActionResult> Index(DateTime? startDateFrom, DateTime? endDateTo, ContractStatus? status)
        {
            var client = CreateApiClient();

            var query = $"api/contracts?startDateFrom={startDateFrom:yyyy-MM-dd}&endDateTo={endDateTo:yyyy-MM-dd}&status={status}";

            var contracts = await client.GetFromJsonAsync<IEnumerable<Contract>>(query);

            var viewModel = new ContractFilterViewModel
            {
                StartDateFrom = startDateFrom,
                EndDateTo = endDateTo,
                Status = status,
                Contracts = contracts ?? new List<Contract>()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var client = CreateApiClient();

            var response = await client.GetAsync($"api/contracts/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var contract = await response.Content.ReadFromJsonAsync<Contract>();

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

            if (viewModel.SignedAgreementFile != null)
            {
                if (!_fileValidationService.IsPdfFile(viewModel.SignedAgreementFile.FileName))
                {
                    ModelState.AddModelError("SignedAgreementFile", "Only PDF files are allowed.");
                }
            }

            if (!ModelState.IsValid)
            {
                viewModel.Clients = await GetClientSelectListAsync();
                return View(viewModel);
            }

            string? uniqueFileName = null;
            string? relativeFilePath = null;

            if (viewModel.SignedAgreementFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                uniqueFileName = Guid.NewGuid() + "_" + viewModel.SignedAgreementFile.FileName;
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

            var client = CreateApiClient();

            var response = await client.PostAsJsonAsync("api/contracts", contract);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Unable to create contract through the API.");
            viewModel.Clients = await GetClientSelectListAsync();
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var client = CreateApiClient();

            var response = await client.GetAsync($"api/contracts/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var contract = await response.Content.ReadFromJsonAsync<Contract>();

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

            if (viewModel.SignedAgreementFile != null)
            {
                if (!_fileValidationService.IsPdfFile(viewModel.SignedAgreementFile.FileName))
                {
                    ModelState.AddModelError("SignedAgreementFile", "Only PDF files are allowed.");
                }
            }

            var client = CreateApiClient();

            var existingResponse = await client.GetAsync($"api/contracts/{id}");

            if (!existingResponse.IsSuccessStatusCode)
                return NotFound();

            var existingContract = await existingResponse.Content.ReadFromJsonAsync<Contract>();

            if (existingContract == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                viewModel.Clients = await GetClientSelectListAsync();
                return View(viewModel);
            }

            if (viewModel.SignedAgreementFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid() + "_" + viewModel.SignedAgreementFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await viewModel.SignedAgreementFile.CopyToAsync(fileStream);
                }

                existingContract.SignedAgreementFileName = uniqueFileName;
                existingContract.SignedAgreementFilePath = "/uploads/" + uniqueFileName;
            }

            existingContract.ClientId = viewModel.ClientId;
            existingContract.StartDate = viewModel.StartDate;
            existingContract.EndDate = viewModel.EndDate;
            existingContract.Status = viewModel.Status;
            existingContract.ServiceLevel = viewModel.ServiceLevel;

            var updateResponse = await client.PutAsJsonAsync($"api/contracts/{id}", existingContract);

            if (updateResponse.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Unable to update contract through the API.");
            viewModel.Clients = await GetClientSelectListAsync();
            return View(viewModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var client = CreateApiClient();

            var response = await client.GetAsync($"api/contracts/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var contract = await response.Content.ReadFromJsonAsync<Contract>();

            return View(contract);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = CreateApiClient();

            var response = await client.DeleteAsync($"api/contracts/{id}");

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Unable to delete contract through the API.");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DownloadAgreement(int id)
        {
            var client = CreateApiClient();

            var response = await client.GetAsync($"api/contracts/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var contract = await response.Content.ReadFromJsonAsync<Contract>();

            if (contract == null || string.IsNullOrEmpty(contract.SignedAgreementFilePath))
                return NotFound();

            string fullPath = Path.Combine(
                _webHostEnvironment.WebRootPath,
                contract.SignedAgreementFilePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
            );

            if (!System.IO.File.Exists(fullPath))
                return NotFound();

            return PhysicalFile(fullPath, "application/pdf", contract.SignedAgreementFileName ?? "agreement.pdf");
        }

        private async Task<IEnumerable<SelectListItem>> GetClientSelectListAsync()
        {
            var client = CreateApiClient();

            var clients = await client.GetFromJsonAsync<IEnumerable<Client>>("api/clients");

            return (clients ?? new List<Client>()).Select(c => new SelectListItem
            {
                Value = c.ClientId.ToString(),
                Text = c.Name
            });
        }
    }
}