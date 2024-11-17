using Gold.ApplicationService.Contract.DTOs.Models.DocumentModels;
using Gold.ApplicationService.Contract.DTOs.Models.UserModels;
using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using Gold.SharedKernel.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.DocumentViewModels
{
    //public class CreateOrEditPaymentViewModel
    //{
    //    public long InstallmentId { get; set; }
    //    public long PaymentId { get; set; }

    //    public int DocumentNumber { get; set; }
    //    public DateTime InstallmentDate { get; set; }
    //    public int InstallmentNumber { get; set; }
    //    public int InstallmentCount { get; set; }
    //    public long InstallmentAmount { get; set; }

    //    [Display(Name = "PaymentReciptImage", ResourceType = typeof(Captions))]
    //    public IFormFile? ImageFile { get; set; }
    //    public string? ImageUrl { get; set; }

    //    [Display(Name = "PaymentDate", ResourceType = typeof(Captions))]
    //    [Required(ErrorMessageResourceName = "Required",ErrorMessageResourceType =typeof(ValidationMessages))]
    //    public string? PersianPaymentDate { get; set; }

    //    [Display(Name = "PaymentAmount", ResourceType = typeof(Captions))]
    //    [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
    //    public string? PaymentAmount { get; set; }

    //    [Display(Name = "PaymentInstallment", ResourceType = typeof(Captions))]
    //    [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
    //    public bool IsPayInstallment { get; set; }

    //    [Display(Name = "Delay", ResourceType = typeof(Captions))]
    //    [Range(0,1000,ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(ValidationMessages))]
    //    public int? DelayDay { get; set; } 

    //    [Display(Name = "PaymentType", ResourceType = typeof(Captions))]
    //    public PaymentType? PaymentType { get; set; }

    //    [Display(Name = "Description", ResourceType = typeof(Captions))]
    //    public string? Description { get; set; }

    //    public List<SelectListItem>? PaymentTypes { get; set; }

    //    public IEnumerable<PaymentModel>? Payments { get; set; }
    //}

    public class CreateOrEditPaymentViewModel
    {
        public long? CustomerPaymentId { get; set; }
        public long InstallmentId { get; set; }
        public long PaymentId { get; set; }

        public int DocumentNumber { get; set; }
        public DateTime InstallmentDate { get; set; }
        public int InstallmentNumber { get; set; }
        public int InstallmentCount { get; set; }
        public long InstallmentAmount { get; set; }
        /// <summary>
        /// مبلغ مانده کل تا این قسط
        /// </summary>
        public long SumOfRemainAmount { get; set; }


		[Display(Name = "PaymentReciptImage", ResourceType = typeof(Captions))]
		[FileChecker(".png|.jpg|.gif|.jpeg", "image/png|image/jpeg|image/gif", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
		[FileSizeChecker(0, 3000, ErrorMessageResourceName = "FileSizeChecker", ErrorMessageResourceType = typeof(ValidationMessages))]
		public IFormFile? ImageFile { get; set; }
        public string? ImageUrl { get; set; }

        [Display(Name = "PaymentDate", ResourceType = typeof(Captions))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? PersianPaymentDate { get; set; }

        [Display(Name = "PaymentAmount", ResourceType = typeof(Captions))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
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

        public LogUserActivityModel? LogUserActivity { get; set; }

        public bool IsDeletePaymentImage { get; set; }
    }
}
