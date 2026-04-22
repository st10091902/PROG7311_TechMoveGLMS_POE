using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TechMoveGLMS.ViewModels
{
    public class ServiceRequestFormViewModel
    {
        public int ServiceRequestId { get; set; }

        [Required]
        [Display(Name = "Contract")]
        public int ContractId { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Cost in USD")]
        [Range(0.01, double.MaxValue, ErrorMessage = "USD amount must be greater than 0.")]
        public decimal CostInUSD { get; set; }

        [Display(Name = "Cost in ZAR")]
        public decimal? CostInZAR { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = string.Empty;

        public IEnumerable<SelectListItem> Contracts { get; set; } = new List<SelectListItem>();
    }
}