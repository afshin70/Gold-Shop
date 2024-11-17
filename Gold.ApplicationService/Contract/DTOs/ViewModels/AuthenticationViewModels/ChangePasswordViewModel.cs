using Gold.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.AuthenticationViewModels
{
    public class ChangePasswordViewModel
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Captions), Name = "Password")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MinLength(6, ErrorMessageResourceName = "MinLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string Password { get; set; }=string.Empty;
        [Display(ResourceType = typeof(Captions), Name = "ConfirmPassword")]
        [Compare(nameof(Password), ErrorMessageResourceName = "Compare", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MinLength(6, ErrorMessageResourceName = "MinLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string ConfirmPassword { get; set; } = string.Empty;
	}
}
