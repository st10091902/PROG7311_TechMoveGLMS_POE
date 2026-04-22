using Microsoft.EntityFrameworkCore;
using TechMoveGLMS.Data;
using TechMoveGLMS.Models;
using TechMoveGLMS.Repositories.Interfaces;

namespace TechMoveGLMS.Repositories
{
    public class ServiceRequestRepository : GenericRepository<ServiceRequest>, IServiceRequestRepository
    {
        public ServiceRequestRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ServiceRequest>> GetServiceRequestsWithContractAsync()
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .ThenInclude(c => c.Client)
                .ToListAsync();
        }

        public async Task<ServiceRequest?> GetServiceRequestWithContractByIdAsync(int id)
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .ThenInclude(c => c.Client)
                .FirstOrDefaultAsync(sr => sr.ServiceRequestId == id);
        }
    }
}