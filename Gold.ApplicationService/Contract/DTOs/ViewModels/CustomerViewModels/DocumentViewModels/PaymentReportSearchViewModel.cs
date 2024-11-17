using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using System.ComponentModel.DataAnnotations;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.DocumentViewModels
{
    public class PaymentReportSearchViewModel
    {
        [Display(Name = "DocumentNumber", ResourceType = typeof(Captions))]
        [Range(1, int.MaxValue, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(ValidationMessages))]
        public int? DocumentNumber { get; set; }

        [Display(Name = "FromDatePayment", ResourceType = typeof(Captions))]
        [PersianDate("/", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? FromDatePayment { get; set; }

        [Display(Name = "ToDatePayment", ResourceType = typeof(Captions))]
        [PersianDate("/", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? ToDatePayment { get; set; }
    }
}
