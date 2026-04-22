using TechMoveGLMS.Models;

namespace TechMoveGLMS.Services.Interfaces
{
    public interface IServiceRequestService
    {
        bool CanCreateServiceRequest(Contract? contract);
    }
}