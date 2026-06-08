using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Json;
using TechMoveGLMS.Models;
using TechMoveGLMS.ViewModels;

namespace TechMoveGLMS.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ServiceRequestsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateApiClient()
        {
            return _httpClientFactory.CreateClient("TechMoveApi");
        }

        public async Task<IActionResult> Index()
        {
            var client = CreateApiClient();

            var serviceRequests = await client.GetFromJsonAsync<IEnumerable<ServiceRequest>>("api/service-requests");

            return View(serviceRequests ?? new List<ServiceRequest>());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var client = CreateApiClient();

            var response = await client.GetAsync($"api/service-requests/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var serviceRequest = await response.Content.ReadFromJsonAsync<ServiceRequest>();

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
            if (!ModelState.IsValid)
            {
                viewModel.Contracts = await GetContractSelectListAsync();
                return View(viewModel);
            }

            var serviceRequest = new ServiceRequest
            {
                ContractId = viewModel.ContractId,
                Description = viewModel.Description,
                CostInUSD = viewModel.CostInUSD,
                CostInZAR = 0,
                Status = viewModel.Status
            };

            var client = CreateApiClient();

            var response = await client.PostAsJsonAsync("api/service-requests", serviceRequest);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            var errorMessage = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(errorMessage))
                errorMessage = "Unable to create service request through the API.";

            ModelState.AddModelError("", errorMessage);

            viewModel.Contracts = await GetContractSelectListAsync();
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var client = CreateApiClient();

            var response = await client.GetAsync($"api/service-requests/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var serviceRequest = await response.Content.ReadFromJsonAsync<ServiceRequest>();

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
            if (!ModelState.IsValid)
            {
                viewModel.Contracts = await GetContractSelectListAsync();
                return View(viewModel);
            }

            var serviceRequest = new ServiceRequest
            {
                ServiceRequestId = viewModel.ServiceRequestId,
                ContractId = viewModel.ContractId,
                Description = viewModel.Description,
                CostInUSD = viewModel.CostInUSD,
                CostInZAR = viewModel.CostInZAR ?? 0,
                Status = viewModel.Status
            };

            var client = CreateApiClient();

            var response = await client.PutAsJsonAsync(
                $"api/service-requests/{viewModel.ServiceRequestId}",
                serviceRequest
            );

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            var errorMessage = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(errorMessage))
                errorMessage = "Unable to update service request through the API.";

            ModelState.AddModelError("", errorMessage);

            viewModel.Contracts = await GetContractSelectListAsync();
            return View(viewModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var client = CreateApiClient();

            var response = await client.GetAsync($"api/service-requests/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var serviceRequest = await response.Content.ReadFromJsonAsync<ServiceRequest>();

            return View(serviceRequest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = CreateApiClient();

            var response = await client.DeleteAsync($"api/service-requests/{id}");

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Unable to delete service request through the API.");
            return RedirectToAction(nameof(Index));
        }

        private async Task<IEnumerable<SelectListItem>> GetContractSelectListAsync()
        {
            var client = CreateApiClient();

            var contracts = await client.GetFromJsonAsync<IEnumerable<Contract>>("api/contracts");

            return (contracts ?? new List<Contract>()).Select(c => new SelectListItem
            {
                Value = c.ContractId.ToString(),
                Text = $"Contract #{c.ContractId} - {c.Client?.Name} ({c.Status})"
            });
        }
    }
}