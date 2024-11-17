using Gold.Resources;
using System.ComponentModel.DataAnnotations;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.AuthenticationViewModels
{
    public class ChangePasswordByTokenViewModel
    {
        public string? UserName { get; set; }
        public Guid? Token { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "Password")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MinLength(6, ErrorMessageResourceName = "MinLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? Password { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "ConfirmPassword")]
        [Compare(nameof(Password), ErrorMessageResourceName = "Compare", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MinLength(6, ErrorMessageResourceName = "MinLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? ConfirmPassword { get; set; }

        public bool PasswordChanged { get; set; }

    }
}
