using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using TechMoveGLMS.Models;

namespace TechMoveGLMS.ViewModels
{
    public class ContractFormViewModel
    {
        public int ContractId { get; set; }

        [Required]
        [Display(Name = "Client")]
        public int ClientId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Required]
        public ContractStatus Status { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Service Level")]
        public string ServiceLevel { get; set; } = string.Empty;

        [Display(Name = "Signed Agreement PDF")]
        public IFormFile? SignedAgreementFile { get; set; }

        public string? ExistingFilePath { get; set; }
        public string? ExistingFileName { get; set; }

        public IEnumerable<SelectListItem> Clients { get; set; } = new List<SelectListItem>();
    }
}