using Gold.Domain.Entities;
using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.FAQViewModels
{
    public  class CreateOrEditFAQCategoryViewModel
    {
        public int? Id { get; set; }
        [Display(ResourceType = typeof(Captions), Name = "Title")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string Title { get; set; }
    } 
}
