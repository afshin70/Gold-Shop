using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using Gold.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels
{

    public class EditCustomerSummaryInfoViewModel
    {
        public int CustomerId { get; set; }
        [Display(ResourceType = typeof(Captions), Name = "FullName")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string CustomerName { get; set; } = string.Empty;

		[Display(ResourceType = typeof(Captions), Name = "Mobile")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(11, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [PersianMobileNumber(ErrorMessageResourceName = "PersianMobileNumber", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string CustomerMobile { get; set; } = string.Empty;

        [Display(Name = "Gender", ResourceType = typeof(Captions))]
        public GenderType? Gender { get; set; }

        [Display(Name = "FatherName", ResourceType = typeof(Captions))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? FatherName { get; set; }
    }
}
