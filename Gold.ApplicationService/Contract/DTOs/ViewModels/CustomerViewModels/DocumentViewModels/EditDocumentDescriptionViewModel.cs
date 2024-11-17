using Gold.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.DocumentViewModels
{
    public class EditDocumentDescriptionViewModel
    {
        public long  Id { get; set; }
        [Display(Name = "AdminDescription", ResourceType = typeof(Captions))]
        public string? Description { get; set; }

        public int? DocumentNo { get; set; }
    }
}
