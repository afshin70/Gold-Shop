using Gold.Domain.Enums;
using Gold.Resources;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.SiteContent
{
    public class SiteContentViewModel
    {
        public List<SelectListItem> ContentTypes { get; set; } = new();

        [Display(ResourceType = typeof(Captions), Name = "Title")]
        public SettingType ContentType { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "Text")]
        public string? Text { get; set; } = string.Empty;
    }
}
