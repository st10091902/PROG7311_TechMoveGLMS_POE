using Microsoft.EntityFrameworkCore;
using TechMoveGLMS.Data;
using TechMoveGLMS.Models;
using TechMoveGLMS.Repositories.Interfaces;

namespace TechMoveGLMS.Repositories
{
    public class ContractRepository : GenericRepository<Contract>, IContractRepository
    {
        public ContractRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Contract>> GetContractsWithClientAsync()
        {
            return await _context.Contracts
                .Include(c => c.Client)
                .ToListAsync();
        }

        public async Task<Contract?> GetContractWithClientByIdAsync(int id)
        {
            return await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.ContractId == id);
        }

        public async Task<IEnumerable<Contract>> FilterContractsAsync(DateTime? startDateFrom, DateTime? endDateTo, ContractStatus? status)
        {
            var query = _context.Contracts
                .Include(c => c.Client)
                .AsQueryable();

            if (startDateFrom.HasValue)
            {
                query = query.Where(c => c.StartDate >= startDateFrom.Value);
            }

            if (endDateTo.HasValue)
            {
                query = query.Where(c => c.EndDate <= endDateTo.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }

            return await query.ToListAsync();
        }
    }
}