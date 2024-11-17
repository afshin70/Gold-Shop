
using Gold.Domain.Entities.AuthEntities;
using Gold.Domain.Entities.Base;

namespace Gold.Domain.Entities
{
    public class Seller : IEntity<int>
    {
        public int Id { get; set; }
        public byte ProductRegisterPerHourCount { get; set; }
        public string? ImageName { get; set; }
        public bool HasAccessToRegisterLoan { get; set; }

        /// <summary>
        /// امکان ایجاد محصول توسط فروشنده
        /// </summary>
        public bool HasAccessToRegisterProduct { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public int GalleryId { get; set; }
        public Gallery Gallery { get; set; }
        public ICollection<Document> Documents { get; set; }

    }

}
