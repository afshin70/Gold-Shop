using Gold.Domain.Enums;
using Gold.Resources;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.SettingsViewModels
{
    public class DefaultMessageViewModel
    {

        public List<SelectListItem> MessageTypes { get; set; } = new();

        [Display(ResourceType = typeof(Captions), Name = "Type")]
        public SettingType MessageType { get; set; }
        

        [Display(ResourceType = typeof(Captions), Name = "Parameters")]
        public string ContentParameters { get; set; } = string.Empty;

        [Display(ResourceType = typeof(Captions), Name = "MessageContent")]
        [MaxLength(500, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? Content { get; set; } = string.Empty;
    }
}
