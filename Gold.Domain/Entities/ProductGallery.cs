using Gold.Domain.Entities.Base;
using Gold.Domain.Enums;

namespace Gold.Domain.Entities
{
    public class ProductGallery : IEntity<long>
    {
        public long Id { get; set; }
        public string FileName { get; set; }
        public MediaFileType FileType { get; set; }
        public int OrderNo { get; set; }
        public long ProductId { get; set; }
        public bool IsThumbnail { get; set; }
        public Product Product { get; set; }
    }
}
