using TechMoveGLMS.Services;
using Xunit;

namespace TechMoveGLMS.Tests.Services
{
    public class FileValidationServiceTests
    {
        private readonly FileValidationService _service;

        public FileValidationServiceTests()
        {
            _service = new FileValidationService();
        }

        [Fact]
        public void IsPdfFile_ReturnsTrue_ForPdfExtension()
        {
            var result = _service.IsPdfFile("contract.pdf");

            Assert.True(result);
        }

        [Fact]
        public void IsPdfFile_ReturnsTrue_ForUppercasePdfExtension()
        {
            var result = _service.IsPdfFile("contract.PDF");

            Assert.True(result);
        }

        [Fact]
        public void IsPdfFile_ReturnsFalse_ForNonPdfExtension()
        {
            var result = _service.IsPdfFile("contract.docx");

            Assert.False(result);
        }

        [Fact]
        public void IsPdfFile_ReturnsFalse_ForEmptyFileName()
        {
            var result = _service.IsPdfFile("");

            Assert.False(result);
        }

        [Fact]
        public void IsPdfFile_ReturnsFalse_ForNullFileName()
        {
            var result = _service.IsPdfFile(null);

            Assert.False(result);
        }
    }
}