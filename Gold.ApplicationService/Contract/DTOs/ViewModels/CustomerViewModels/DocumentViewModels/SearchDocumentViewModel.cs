using Gold.Domain.Entities;
using Gold.Resources;
using Gold.SharedKernel.Enums;
using Gold.SharedKernel.Attributes.Validation;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.DocumentViewModels
{
    public class SearchDocumentViewModel
    {

        [Display(Name = "DocumentNumber", ResourceType = typeof(Captions))]
        public int? DocumentNumber { get; set; }
        [Display(Name = "Name", ResourceType = typeof(Captions))]
        [MaxLength(200, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? Name { get; set; }
        [Display(Name = "DocumentStatus", ResourceType = typeof(Captions))]
        public DocumentStatus? DocumentStatus { get; set; }
        [Display(Name = "DocumentDay", ResourceType = typeof(Captions))]
        [Range(1, 31, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(ValidationMessages))]
        public byte? DocumentDay { get; set; }
        [Display(Name = "DocumentDate", ResourceType = typeof(Captions))]
        public string? DocumentDate { get; set; }
        [Display(Name = "InstallmentDate", ResourceType = typeof(Captions))]
        public string? InstallmentDate { get; set; }
        /// <summary>
        /// قسط پرداخت نشده از تاریخ
        /// </summary>
        [Display(Name = "NotPayedInstallmentFromDate", ResourceType = typeof(Captions))]
        [PersianDate("/", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? NotPayedInstallmentFromDate { get; set; }

        /// <summary>
        /// تاریخ تسویه
        /// </summary>
        [Display(Name = "SettleDate", ResourceType = typeof(Captions))]
        [PersianDate("/", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? SettleDate { get; set; }
        /// <summary>
        /// قسط پرداخت نشده تا تاریخ
        /// </summary>
        [Display(Name = "NotPayedInstallmentToDate", ResourceType = typeof(Captions))]
        [PersianDate("/", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? NotPayedInstallmentToDate { get; set; }
        [Display(Name = "Gallery", ResourceType = typeof(Captions))]
        public int? GalleryId { get; set; }
        [Display(Name = "CollateralType", ResourceType = typeof(Captions))]
        public int? CollateralTypeId { get; set; }
        [Display(Name = "CollateralDescription", ResourceType = typeof(Captions))]
        public string? CollateralDescription { get; set; }

        /// <summary>
        /// تعداد اقساط عقب افتاده
        /// </summary>
        [Display(Name = nameof(Captions.OverdueInstallmentsCount), ResourceType = typeof(Captions))]
        //[Range(1, 500, ErrorMessageResourceName = nameof(ValidationMessages.Range), ErrorMessageResourceType = typeof(ValidationMessages))]
        public int? OverdueInstallmentsCount { get; set; }
    }
}
