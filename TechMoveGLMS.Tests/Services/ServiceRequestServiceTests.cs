using TechMoveGLMS.Models;
using TechMoveGLMS.Services;
using Xunit;

namespace TechMoveGLMS.Tests.Services
{
    public class ServiceRequestServiceTests
    {
        private readonly ServiceRequestService _service;

        public ServiceRequestServiceTests()
        {
            _service = new ServiceRequestService();
        }

        [Fact]
        public void CanCreateServiceRequest_ReturnsTrue_WhenContractIsActive()
        {
            var contract = new Contract { Status = ContractStatus.Active };

            var result = _service.CanCreateServiceRequest(contract);

            Assert.True(result);
        }

        [Fact]
        public void CanCreateServiceRequest_ReturnsTrue_WhenContractIsDraft()
        {
            var contract = new Contract { Status = ContractStatus.Draft };

            var result = _service.CanCreateServiceRequest(contract);

            Assert.True(result);
        }

        [Fact]
        public void CanCreateServiceRequest_ReturnsFalse_WhenContractIsExpired()
        {
            var contract = new Contract { Status = ContractStatus.Expired };

            var result = _service.CanCreateServiceRequest(contract);

            Assert.False(result);
        }

        [Fact]
        public void CanCreateServiceRequest_ReturnsFalse_WhenContractIsOnHold()
        {
            var contract = new Contract { Status = ContractStatus.OnHold };

            var result = _service.CanCreateServiceRequest(contract);

            Assert.False(result);
        }

        [Fact]
        public void CanCreateServiceRequest_ReturnsFalse_WhenContractIsNull()
        {
            var result = _service.CanCreateServiceRequest(null);

            Assert.False(result);
        }
    }
}