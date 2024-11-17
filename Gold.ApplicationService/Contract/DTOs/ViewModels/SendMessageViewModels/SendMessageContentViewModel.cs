using Gold.Resources;
using System.ComponentModel.DataAnnotations;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.SendMessageViewModels
{
    public class SendMessageContentViewModel
    {
        [Display(ResourceType = typeof(Captions), Name = "Title")]
        [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? Title { get; set; } = string.Empty;

        [Display(ResourceType = typeof(Captions), Name = "MessageContent")]
        [MaxLength(500, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? MessageContent { get; set; }

        public int[]? CustomerIds { get; set; }
    }
}
