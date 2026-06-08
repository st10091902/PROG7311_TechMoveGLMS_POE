using Microsoft.AspNetCore.Mvc;
using TechMoveGLMS.Models;
using TechMoveGLMS.Repositories.Interfaces;

namespace TechMoveGLMS.Api.Controllers
{
    [Route("api/clients")]
    [ApiController]
    public class ClientsApiController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;

        public ClientsApiController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            var clients = await _clientRepository.GetAllAsync();
            return Ok(clients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClient(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);

            if (client == null)
                return NotFound();

            return Ok(client);
        }

        [HttpPost]
        public async Task<ActionResult<Client>> CreateClient(Client client)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _clientRepository.AddAsync(client);
            await _clientRepository.SaveAsync();

            return CreatedAtAction(nameof(GetClient), new { id = client.ClientId }, client);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(int id, Client client)
        {
            if (id != client.ClientId)
                return BadRequest();

            var existingClient = await _clientRepository.GetByIdAsync(id);
            if (existingClient == null)
                return NotFound();

            existingClient.Name = client.Name;
            existingClient.ContactDetails = client.ContactDetails;
            existingClient.Region = client.Region;

            _clientRepository.Update(existingClient);
            await _clientRepository.SaveAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);

            if (client == null)
                return NotFound();

            _clientRepository.Delete(client);
            await _clientRepository.SaveAsync();

            return NoContent();
        }
    }
}