using TechMoveGLMS.Services.Interfaces;

namespace TechMoveGLMS.Services
{
    public class FileValidationService : IFileValidationService
    {
        public bool IsPdfFile(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            return Path.GetExtension(fileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase);
        }
    }
}