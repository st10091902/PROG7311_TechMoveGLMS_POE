using TechMoveGLMS.Models;
using TechMoveGLMS.Services.Interfaces;

namespace TechMoveGLMS.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        public bool CanCreateServiceRequest(Contract? contract)
        {
            if (contract == null)
                return false;

            return contract.Status != ContractStatus.Expired &&
                   contract.Status != ContractStatus.OnHold;
        }
    }
}