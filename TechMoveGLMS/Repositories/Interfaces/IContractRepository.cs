using TechMoveGLMS.Models;

namespace TechMoveGLMS.Repositories.Interfaces
{
    public interface IContractRepository : IGenericRepository<Contract>
    {
        Task<IEnumerable<Contract>> GetContractsWithClientAsync();
        Task<Contract?> GetContractWithClientByIdAsync(int id);
        Task<IEnumerable<Contract>> FilterContractsAsync(DateTime? startDateFrom, DateTime? endDateTo, ContractStatus? status);
    }
}