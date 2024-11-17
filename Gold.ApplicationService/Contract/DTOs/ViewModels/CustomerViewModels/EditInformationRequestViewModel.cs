using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels
{
    public class EditInformationRequestViewModel
    {
        [Display(Name = "DiscrepancyOfInfoDescruiption", ResourceType = typeof(Captions))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string Description { get; set; }=string.Empty;

        [Display(ResourceType = typeof(Captions), Name = "DocumentImage")]
        [FileChecker(".png|.jpg|.jpeg", "image/png|image/jpeg", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        [FileSizeChecker(0, 2048, ErrorMessageResourceName = "FileSizeChecker", ErrorMessageResourceType = typeof(ValidationMessages))]
        public IFormFile?   Image { get; set; }
    }
}
