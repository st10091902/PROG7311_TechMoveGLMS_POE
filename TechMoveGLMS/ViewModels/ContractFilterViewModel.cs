using System.ComponentModel.DataAnnotations;
using TechMoveGLMS.Models;

namespace TechMoveGLMS.ViewModels
{
    public class ContractFilterViewModel
    {
        [Display(Name = "Start Date From")]
        [DataType(DataType.Date)]
        public DateTime? StartDateFrom { get; set; }

        [Display(Name = "End Date To")]
        [DataType(DataType.Date)]
        public DateTime? EndDateTo { get; set; }

        [Display(Name = "Status")]
        public ContractStatus? Status { get; set; }

        public IEnumerable<Contract> Contracts { get; set; } = new List<Contract>();
    }
}