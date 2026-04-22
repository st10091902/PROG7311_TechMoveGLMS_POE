using TechMoveGLMS.Data;
using TechMoveGLMS.Models;
using TechMoveGLMS.Repositories.Interfaces;

namespace TechMoveGLMS.Repositories
{
    public class ClientRepository : GenericRepository<Client>, IClientRepository
    {
        public ClientRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}