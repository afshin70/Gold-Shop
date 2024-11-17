using Gold.Resources;
using Gold.SharedKernel.Enums;
using System.ComponentModel.DataAnnotations;

namespace Gold.ApplicationService.Contract.DTOs.Models.CustomerModels
{
    public class SearchCustomerByBankCardNumberViewModel
    {
        [Display(Name = "Search", ResourceType = typeof(Captions))]
        public CustomerSearchType SearchType { get; set; }

        [Display(Name = "BankCardNumber", ResourceType = typeof(Captions))]
        [MinLength(4, ErrorMessageResourceName = "MinLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(19, ErrorMessageResourceName = "BankCardNumberMaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? BankCardNumber { get; set; }
    }
}
