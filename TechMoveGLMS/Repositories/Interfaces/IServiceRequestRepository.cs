using TechMoveGLMS.Models;

namespace TechMoveGLMS.Repositories.Interfaces
{
    public interface IServiceRequestRepository : IGenericRepository<ServiceRequest>
    {
        Task<IEnumerable<ServiceRequest>> GetServiceRequestsWithContractAsync();
        Task<ServiceRequest?> GetServiceRequestWithContractByIdAsync(int id);
    }
}