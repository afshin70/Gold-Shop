using Gold.Domain.Enums;
using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.ArticleViewModels
{
    public class CreateOrEditArticleViewModel
    {
        public int? Id { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "Title")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
        [MaxLength(250, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string Title { get; set; }
        
        //[Display(ResourceType = typeof(Captions), Name = "Image")]
        ////[FileExtentionChecker(".mp4|.mpeg", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        //[FileChecker(".png|.jpg|.gif|.jpeg", "image/png|image/jpeg|image/gif", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        //[FileSizeChecker(0, 1024, ErrorMessageResourceName = "FileSizeChecker", ErrorMessageResourceType = typeof(ValidationMessages))]
        //public IFormFile? ImageFile { get; set; }
        //public bool? IsDeleteImageFile { get; set; }
        //public string? ImageFileUrl { get; set; }
        //public string? ImageFileName { get; set; }

        //[Display(ResourceType = typeof(Captions), Name = "VideoFile")]
        //[FileExtentionChecker(".mp4|.mpeg", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        //[FileSizeChecker(0, 10240, ErrorMessageResourceName = "FileSizeChecker", ErrorMessageResourceType = typeof(ValidationMessages))]
        //public IFormFile? VideoFile { get; set; }
        //public bool? IsDeleteVideoFile { get; set; }
        //public string? VideoFileUrl { get; set; }
        //public string? VideoFileName { get; set; }

        [Display(ResourceType = typeof(Captions), Name = "Description")]
        public string? Description { get; set; }
        
        //public bool Status { get; set; }
    }
}
