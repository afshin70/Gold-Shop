using Gold.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.Models.ProductModels.ProductGallertModels
{
    public class ProductGalleryModel
    {
        public long ProductGalleryId { get; set; }
        public long OwnProductId { get; set; }
        public string FileName { get; set; }
        public MediaFileType FileType { get; set; }
        public int OrderNo { get; set; }
        public bool IsThumbnail { get; set; }
        public string ProductName { get; set; }
    }
}
