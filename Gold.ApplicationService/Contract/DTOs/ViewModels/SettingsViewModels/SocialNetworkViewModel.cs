using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.SettingsViewModels
{
    public class SocialNetworkViewModel
    {
        public int SocialNetworkId { get; set; }
        [Display(ResourceType = typeof(Captions), Name = "Title")]
        [MaxLength(50, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string Title { get; set; }=string.Empty;
        [Display(ResourceType = typeof(Captions), Name = "Url")]
        [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string Url { get; set; } = string.Empty;
		//[FileExtentionChecker(".svg", ErrorMessageResourceName = "FileExtentionChecker", ErrorMessageResourceType = typeof(ValidationMessages))]
		//[FileContentChecker("image/svg+xml", ErrorMessageResourceName = "FileExtentionChecker", ErrorMessageResourceType = typeof(ValidationMessages))]
		[Display(ResourceType = typeof(Captions), Name = "Icon")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [FileSizeChecker(0, 1, ErrorMessageResourceName = "FileSizeChecker", ErrorMessageResourceType = typeof(ValidationMessages))]
        [FileChecker(".svg", "image/svg+xml", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public IFormFile? Icon { get; set; }
        public string? IconUrl { get; set; } = string.Empty;
    }
}
