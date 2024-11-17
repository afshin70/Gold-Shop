using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using System.ComponentModel.DataAnnotations;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.AuthenticationViewModels
{
    public class VerifyResetPasswordCodeViewModel
    {
        [Display(Name = nameof(Captions.UserName), ResourceType = typeof(Captions))]
        [MaxLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxLength))]
        [MinLength(3, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MinLength))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Required))]
        public string? UserName { get; set; }

        [Display(Name = nameof(Captions.Mobile), ResourceType = typeof(Captions))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Required))]
        [PersianMobileNumber(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Required))]
        public string? Mobile { get; set; }

        public TimeSpan Timer { get; set; }

        [Display(Name = nameof(Captions.VerifyCode), ResourceType = typeof(Captions))]
        public string? VerifyCode { get; set; }

        [Display(Name = nameof(Captions.VerifyCode), ResourceType = typeof(Captions))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Required))]
        public string[]? VerifyCodeNumber { get; set; }

        public Guid? Token { get; set; }
    }
}
