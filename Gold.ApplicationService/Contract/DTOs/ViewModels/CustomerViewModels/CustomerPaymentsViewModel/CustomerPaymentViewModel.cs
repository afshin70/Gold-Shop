using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.CustomerPaymentsViewModel
{
    public class CustomerPaymentViewModel
    {
        public long DocumentId { get; set; }
        [Display(ResourceType = typeof(Captions), Name = "PaymentReceiptImage")]
        [FileChecker(".png|.jpg|.gif|.jpeg", "image/png|image/jpeg|image/gif", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        [FileSizeChecker(0, 2048, ErrorMessageResourceName = "FileSizeChecker", ErrorMessageResourceType = typeof(ValidationMessages))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public IFormFile? PaymentReceiptImage { get; set; }
        [Display(Name = "Description", ResourceType = typeof(Captions))]
        public string? Description { get; set; }

        [Display(Name = "PayDate", ResourceType = typeof(Captions))]
        [MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [PersianDate("/", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? PayDate { get; set; }

        [Display(Name = "PayTime", ResourceType = typeof(Captions))]
        [MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [TimeChecker(ErrorMessageResourceName = "TimeChecker", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? PayTime { get; set; }

        [Display(Name = "Price", ResourceType = typeof(Captions))]
       // [Numberic(",", ErrorMessageResourceName = "Numberic", ErrorMessageResourceType = typeof(ValidationMessages))]
		[MaxLength(15, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MonyFormat(",", ErrorMessageResourceName = "MonyFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
		public string? PayAmount { get; set; }
    }
}
