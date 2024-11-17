using Gold.ApplicationService.Contract.DTOs.Models.ProductModels.ProductGallertModels;
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

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.ProductViewModels.ProdctGalleryViewModels
{
    public class CreateProductGalleryViewModel
    {
        public long ProductGalleryId { get; set; }
        public long OwnProductId { get; set; }
        [Display(ResourceType = typeof(Captions), Name = "VideoOrImageFile")]
        [FileExtentionChecker(".mp4|.mpeg|.png|.jpg|.jpeg", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        [FileSizeChecker(0, 10240, ErrorMessageResourceName = "FileSizeChecker", ErrorMessageResourceType = typeof(ValidationMessages))]
        public IFormFile? UploadedFile { get; set; }
        public string? UploadedFileUrl { get; set; }
        public string? ProductName { get; set; }

        public List<ProductGalleryModel>? ProductGalleries { get; set; }
    }
}
