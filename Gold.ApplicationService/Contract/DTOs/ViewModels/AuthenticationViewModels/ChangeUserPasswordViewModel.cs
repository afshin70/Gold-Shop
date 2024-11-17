using Gold.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.AuthenticationViewModels
{
    public class ChangeUserPasswordViewModel
    {
        [Display(ResourceType = typeof(Captions), Name = "CurrentPassword")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MinLength(6, ErrorMessageResourceName = "MinLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string CurrentPassword { get; set; } = string.Empty;
		[Display(ResourceType = typeof(Captions), Name = "NewPassword")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MinLength(6, ErrorMessageResourceName = "MinLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string NewPassword { get; set; } = string.Empty;
		[Display(ResourceType = typeof(Captions), Name = "ConfirmNewPassword")]
        [Compare(nameof(NewPassword), ErrorMessageResourceName = "Compare", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MinLength(6, ErrorMessageResourceName = "MinLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string ConfirmNewPassword { get; set; } = string.Empty;
	}
}
