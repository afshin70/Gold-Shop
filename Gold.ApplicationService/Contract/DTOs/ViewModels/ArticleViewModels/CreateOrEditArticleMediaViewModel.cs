using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.ArticleViewModels
{
    public class CreateOrEditArticleMediaViewModel
    {
        public int? Id { get; set; }

        [Display(ResourceType = typeof(Captions), Name =nameof(Captions.ImageFile))]
        [FileChecker(".png|.jpg|.gif|.jpeg", "image/png|image/jpeg|image/gif", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        [FileSizeChecker(0, 1024, ErrorMessageResourceName = "FileSizeChecker", ErrorMessageResourceType = typeof(ValidationMessages))]
        public IFormFile? ImageFile { get; set; }


        [Display(ResourceType = typeof(Captions), Name = "VideoFile")]
        [FileExtentionChecker(".mp4|.mpeg", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        [FileSizeChecker(0, 10240, ErrorMessageResourceName = "FileSizeChecker", ErrorMessageResourceType = typeof(ValidationMessages))]
        public IFormFile? VideoFile { get; set; }

        public bool IsVideo { get; set; }
        public string? FileName { get; set; }
        public string? ArticleTitle { get; set; }
    }
}
