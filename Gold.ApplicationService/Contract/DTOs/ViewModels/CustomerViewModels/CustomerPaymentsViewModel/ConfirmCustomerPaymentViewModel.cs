using Gold.ApplicationService.Contract.DTOs.Models.DocumentModels;
using Gold.Resources;
using Gold.SharedKernel.Enums;
using Gold.SharedKernel.Attributes.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.CustomerPaymentsViewModel
{

    public class ConfirmCustomerPaymentViewModel
    {
        public long CustomerPaymentId { get; set; }
        public long InstallmentId { get; set; }

        // public IFormFile? ImageFile { get; set; }
        [Display(Name = "PaymentReciptImage", ResourceType = typeof(Captions))]
        public string? ImageName { get; set; }

        [Display(Name = "PaymentDate", ResourceType = typeof(Captions))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? PersianPaymentDate { get; set; }

        [Display(Name = "PaymentAmount", ResourceType = typeof(Captions))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MonyFormat(",", ErrorMessageResourceName = "MonyFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? PaymentAmount { get; set; }

        [Display(Name = "PaymentInstallment", ResourceType = typeof(Captions))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public bool IsPayInstallment { get; set; }

        [Display(Name = "Delay", ResourceType = typeof(Captions))]
        [Range(0, 1000, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(ValidationMessages))]
        public int? DelayDay { get; set; }

        [Display(Name = "PaymentType", ResourceType = typeof(Captions))]
        public PaymentType? PaymentType { get; set; }

        [Display(Name = "Description", ResourceType = typeof(Captions))]
		//[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
		public string? Description { get; set; }

        [Display(Name = "CustomerMessageContent", ResourceType = typeof(Captions))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? CustomerMessageContent { get; set; }

        //public List<SelectListItem>? PaymentTypes { get; set; }

        public IEnumerable<PaymentModel>? Payments { get; set; }

        public CustomerPaymentInfoViewModel? CustomerPaymentInfo { get; set; }
    }
}
