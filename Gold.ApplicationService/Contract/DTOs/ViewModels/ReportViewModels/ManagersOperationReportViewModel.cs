using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using Gold.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.ReportViewModels
{
    public class ManagersOperationReportViewModel
    {

        [Display(Name = "Page", ResourceType = typeof(Captions))]
        public byte? AdminMenuId { get; set; }

        [Display(Name = "Operation", ResourceType = typeof(Captions))]
        public AdminActivityType? ActivityType { get; set; }

        [Display(Name = "User", ResourceType = typeof(Captions))]
        public int? UserId { get; set; }

        [Display(Name = "FromDate", ResourceType = typeof(Captions))]
        public string? FromDate { get; set; }

        [Display(Name = "ToDate", ResourceType = typeof(Captions))]
        public string? ToDate { get; set; }

        [Display(Name = "Description", ResourceType = typeof(Captions))]
        public string? Description { get; set; }
    }
}
