using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using System.ComponentModel.DataAnnotations;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.DocumentViewModels
{
 public class SettleDocumentViewModel
    {
        public long DocumentId { get; set; }
        /// <summary>
        /// DocumentNumber
        /// </summary>
        public int? DocumentNumber { get; set; }
        public DateTime? DocumentDate { get; set; }
        public int? InstallmentCount { get; set; }
        public long? InstallmentRemainAmount { get; set; }
        public long TotalRemainAmount { get; set; }
        public int? TotalDelayDay { get; set; }

        [Display(Name = "DelayAmount", ResourceType = typeof(Captions))]
       // [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public long DelayAmount { get; set; }

        [Display(Name = "Discount", ResourceType = typeof(Captions))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MonyFormat(",", ErrorMessageResourceName = "MonyFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? DiscountAmount { get; set; }

        [Display(Name = "SettleDate", ResourceType = typeof(Captions))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [PersianDate("/", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string SettleDate { get; set; } = string.Empty;


		[Display(Name = "DeliveryDate", ResourceType = typeof(Captions))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [PersianDate("/", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string DeliveryDate { get; set; } = string.Empty;

        [Display(Name = "ReturnedAmount", ResourceType = typeof(Captions))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MonyFormat(",", ErrorMessageResourceName = "MonyFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? ReturnedAmount { get; set; }

        [Display(Name = "CustomerMessageContent", ResourceType = typeof(Captions))]
        public string? CustomerSettleMessageContent { get; set; }

    }
}
