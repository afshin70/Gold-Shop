using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using Gold.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.AuthenticationViewModels.RegisterCustomerViewModels
{
    public class RegisterCustomerBaseInfoViewModel
    {
        [Display(Name = nameof(Captions.Nationality), ResourceType = typeof(Captions))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Required))]
        public NationalityType? NationalityType { get; set; }

        [Display(Name = nameof(Captions.NationalCode), ResourceType = typeof(Captions))]
        [MaxLength(50, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxLength))]
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Required))]
        [Numberic(null, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.OnlyNumberInput))]
        public string? NationalCode { get; set; } = string.Empty;

        [Display(Name = nameof(Captions.Mobile), ResourceType = typeof(Captions))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Required))]
        //[PersianMobileNumber(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Invalid))]
        [MaxLength(11, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxLength))]
        [MinLength(11, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MinLength))]
        [RegularExpression("^(\\+98|0098|98|0)?9\\d{9}$", ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Invalid))]
        public string? Mobile { get; set; } = string.Empty;

        [Display(Name = nameof(Captions.SecurityCode), ResourceType = typeof(Captions))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Required))]
        [MaxLength(6, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxLength))]
        [MinLength(6, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MinLength))]
        public string? Captcha { get; set; } = string.Empty;
    }

    public class RegisterCustomerVerifyCodeViewModel
    {
        public NationalityType? NationalityType { get; set; }
        public string? NationalCode { get; set; } = string.Empty;
        public string? Mobile { get; set; } = string.Empty;


        public TimeSpan Timer { get; set; }

        [Display(Name = nameof(Captions.VerifyCode), ResourceType = typeof(Captions))]
        public string? VerifyCode { get; set; }

        [Display(Name = nameof(Captions.VerifyCode), ResourceType = typeof(Captions))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Required))]
        public string[]? VerifyCodeNumber { get; set; }

        public Guid? Token { get; set; }
    }


    public class RegisterCustomerViewModel
    {
        public NationalityType? NationalityType { get; set; }
        public string? NationalCode { get; set; } = string.Empty;
        public string? Mobile { get; set; } = string.Empty;
        public Guid? Token { get; set; }

        [Display(Name = nameof(Captions.FullName), ResourceType = typeof(Captions))]
        [MaxLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxLength))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Required))]
        public string? FullName { get; set; }

        [Display(Name = nameof(Captions.Gender), ResourceType = typeof(Captions))]
        public GenderType? Gender { get; set; }

        [Display(Name = nameof(Captions.FatherName), ResourceType = typeof(Captions))]
        [MaxLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxLength))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Required))]
        public string? FatherName { get; set; }

        [Display(ResourceType = typeof(Captions), Name = nameof(Captions.EssentialTel))]
        [MaxLength(11, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Numberic(null, ErrorMessageResourceName = "Numberic", ErrorMessageResourceType = typeof(ValidationMessages))]
        [RegularExpression("^(\\+98|0098|98|0)?9\\d{9}$", ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Invalid))]
        public string? EssentialTel { get; set; } = string.Empty;

        [Display(ResourceType = typeof(Captions), Name = nameof(Captions.EssentialTelRatio))]
        [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? RelationShip { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "BirthDate")]
        [MaxLength(10, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [PersianDate("/", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? BirthDate { get; set; }

    }
}
