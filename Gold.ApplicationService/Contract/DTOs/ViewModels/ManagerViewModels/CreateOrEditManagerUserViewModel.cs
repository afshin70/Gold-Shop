using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.ManagerViewModels
{
    public class CreateOrEditManagerUserViewModel
    {
     
        public int ManagerUserId { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "FullName")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string FullName { get; set; } = string.Empty;


        [Display(ResourceType = typeof(Captions), Name = "AccessLevel")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public int? AccessLevelId { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "Mobile")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(11, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [PersianMobileNumber(ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string Mobile { get; set; } = string.Empty;

        [Display(ResourceType = typeof(Captions), Name = "UserName")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string UserName { get; set; } = string.Empty;

        [Display(ResourceType = typeof(Captions), Name = "Status")]
        public bool IsActive { get; set; }


        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Captions), Name = "Password")]
        [MinLength(6, ErrorMessageResourceName = "MinLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? Password { get; set; }


        [Display(ResourceType = typeof(Captions), Name = "ConfirmPassword")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessageResourceName = "NotMatch", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? ConfirmPassword { get; set; }

        public List<SelectListItem>? AccessLevels { get; set; }
    }
}
