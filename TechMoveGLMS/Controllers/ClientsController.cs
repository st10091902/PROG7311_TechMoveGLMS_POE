using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using TechMoveGLMS.Models;

namespace TechMoveGLMS.Controllers
{
    public class ClientsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ClientsController(IHttpClientFactory httpClientFactory)
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

            try
            {
                var response = await client.GetAsync("api/clients");

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Unable to load clients from the API.");
                    return View(new List<Client>());
                }

                var clients = await response.Content.ReadFromJsonAsync<IEnumerable<Client>>();

                return View(clients ?? new List<Client>());
            }
            catch
            {
                ModelState.AddModelError("", "The API is not available. Please make sure TechMoveGLMS.Api is running.");
                return View(new List<Client>());
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var client = CreateApiClient();

            var result = await client.GetAsync($"api/clients/{id}");

            if (!result.IsSuccessStatusCode)
                return NotFound();

            var selectedClient = await result.Content.ReadFromJsonAsync<Client>();

            return View(selectedClient);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client clientModel)
        {
            if (!ModelState.IsValid)
                return View(clientModel);

            var client = CreateApiClient();

            var response = await client.PostAsJsonAsync("api/clients", clientModel);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Unable to create client through the API.");
            return View(clientModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var client = CreateApiClient();

            var result = await client.GetAsync($"api/clients/{id}");

            if (!result.IsSuccessStatusCode)
                return NotFound();

            var clientModel = await result.Content.ReadFromJsonAsync<Client>();

            return View(clientModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Client clientModel)
        {
            if (id != clientModel.ClientId)
                return NotFound();

            if (!ModelState.IsValid)
                return View(clientModel);

            var client = CreateApiClient();

            var response = await client.PutAsJsonAsync($"api/clients/{id}", clientModel);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Unable to update client through the API.");
            return View(clientModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var client = CreateApiClient();

            var result = await client.GetAsync($"api/clients/{id}");

            if (!result.IsSuccessStatusCode)
                return NotFound();

            var clientModel = await result.Content.ReadFromJsonAsync<Client>();

            return View(clientModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = CreateApiClient();

            var response = await client.DeleteAsync($"api/clients/{id}");

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Unable to delete client through the API.");
            return RedirectToAction(nameof(Index));
        }
    }
}