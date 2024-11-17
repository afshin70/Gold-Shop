using Gold.Resources;
using Gold.SharedKernel.Enums;
using System.ComponentModel.DataAnnotations;

namespace Gold.ApplicationService.Contract.DTOs.Models.CustomerModels
{
    public class SearchCustomerByPhoneNumberViewModel
    {
        [Display(Name = "Search", ResourceType = typeof(Captions))]
        public CustomerSearchType SearchType { get; set; }

        [Display(Name = "PhoneNumber", ResourceType = typeof(Captions))]
        [MinLength(4, ErrorMessageResourceName = "MinLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(11, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? PhoneNumber { get; set; }
    }
}
