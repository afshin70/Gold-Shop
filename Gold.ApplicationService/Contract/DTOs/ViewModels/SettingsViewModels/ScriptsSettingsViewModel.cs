using Gold.Resources;
using System.ComponentModel.DataAnnotations;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.SettingsViewModels
{
    public class ScriptsSettingsViewModel
    {
        /// <summary>
        /// اسکریپت های هدر
        /// </summary>
        [Display(ResourceType = typeof(Captions), Name = "HeaderScript")]
        [MaxLength(800, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? Header { get; set; }
        /// <summary>
        /// اسکریپت های فوتر
        /// </summary>
        [Display(ResourceType = typeof(Captions), Name = "FooterScript")]
        [MaxLength(800, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? Footer { get; set; }
    }
}
