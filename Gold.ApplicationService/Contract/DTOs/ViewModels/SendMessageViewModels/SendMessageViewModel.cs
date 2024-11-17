using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using Gold.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.SendMessageViewModels
{
    public class SendMessageViewModel
    {
        [Display(ResourceType = typeof(Captions), Name = "FullName")]
        [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? FullName { get; set; } = string.Empty;

        [Display(ResourceType = typeof(Captions), Name = "Gender")]
        public GenderType? GenderType { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "Nationality")]
        public NationalityType? NationalityType { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "UserStatus")]
        public bool? UserStatus { get; set; }

        [Display(Name = "FromRegisterDate", ResourceType = typeof(Captions))]
        [PersianDate("/", ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? FromRegisterDate { get; set; }

        [Display(Name = "ToRegisterDate", ResourceType = typeof(Captions))]
        [PersianDate("/", ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? ToRegisterDate { get; set; }

        [Display(Name = "FromBirthDateDay", ResourceType = typeof(Captions))]
        [Range(1, 31, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(ValidationMessages))]
        public byte? FromBirthDateDay { get; set; }

        [Display(Name = "ToBirthDateDay", ResourceType = typeof(Captions))]
        [Range(1, 31, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(ValidationMessages))]
        public byte? ToBirthDateDay { get; set; }

        [Display(Name = "FromBirthDateMonth", ResourceType = typeof(Captions))]
        [Range(1, 12, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(ValidationMessages))]
        public byte? FromBirthDateMonth { get; set; }

        [Display(Name = "ToBirthDateMonth", ResourceType = typeof(Captions))]
        [Range(1, 12, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(ValidationMessages))]
        public byte? ToBirthDateMonth { get; set; }

        [Display(Name = "FromBirthDateYear", ResourceType = typeof(Captions))]
        [Range(1300, 9999, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(ValidationMessages))]
        public int? FromBirthDateYear { get; set; }

        [Display(Name = "ToBirthDateYear", ResourceType = typeof(Captions))]
        [Range(1300, 9999, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(ValidationMessages))]
        public int? ToBirthDateYear { get; set; }

        [Display(Name = "AccountStatus", ResourceType = typeof(Captions))]
        public AccountStatusType? AccountStatusType { get; set; }

        [Display(Name = "DocumentStatus", ResourceType = typeof(Captions))]
        public DocumentStatus? DocumentStatus { get; set; }

        [Display(Name = "CollateralType", ResourceType = typeof(Captions))]
        public int? CollateralTypeId { get; set; }

        [Display(Name = "Gallery", ResourceType = typeof(Captions))]
        public int? GalleryId { get; set; }

        [Display(Name = "UnpaidInstallmentFromDate", ResourceType = typeof(Captions))]
        [PersianDate("/", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? UnpaidInstallmentFromDate { get; set; }

        [Display(Name = "UnpaidInstallmentToDate", ResourceType = typeof(Captions))]
        [PersianDate("/", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? UnpaidInstallmentToDate { get; set; }

        [Display(Name = "DocumentToDate", ResourceType = typeof(Captions))]
        [PersianDate("/", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? DocumentToDate { get; set; }

        [Display(Name = "DocumentFromDate", ResourceType = typeof(Captions))]
        [PersianDate("/", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? DocumentFromDate { get; set; }

        [Display(Name = "FromInstallmentAmount", ResourceType = typeof(Captions))]
        [MaxLength(int.MaxValue, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MonyFormat(",", ErrorMessageResourceName = "MonyFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? FromInstallmentAmount { get; set; }
        [Display(Name = "ToInstallmentAmount", ResourceType = typeof(Captions))]
        [MonyFormat(",", ErrorMessageResourceName = "MonyFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(int.MaxValue, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? ToInstallmentAmount { get; set; }
    }
}
