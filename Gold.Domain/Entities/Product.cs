using Gold.Domain.Entities.AuthEntities;
using Gold.Domain.Entities.Base;
using Gold.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Gold.Domain.Entities
{
    public class Product : IEntity<long>
    {
        public long Id { get; set; }
        public string Title { get; set; }
        /// <summary>
        /// وزن به گرم
        /// </summary>
        public decimal Weight { get; set; }
        /// <summary>
        /// عیار طلا بین 1 تا 24
        /// </summary>
        public byte Karat { get; set; }
        /// <summary>
        /// اجرت بین 1 تا 500
        /// </summary>
        public decimal Wage { get; set; }
        /// <summary>
        /// سود گالری عدد صحیح بین صفر تا حداکثر تعیین شده در مدیریت
        /// </summary>
        public decimal GalleryProfit { get; set; }
        ///// <summary>
        ///// مالیات بین 1 تا 100
        ///// </summary>
        //public byte Tax { get; set; }
        public ProductStatus Status { get; set; }
        public long VisitedCount { get; set; }
        public string? Description { get; set; }
        public long StonPrice { get; set; }
        public DateTime RegisterDate { get; set; }
        //public long OrderNo { get; set; }
        /// <summary>
        /// آی دی کاربرثبت کننده
        /// </summary>
        public int RegistrarUserId { get; set; }
        /// <summary>
        /// قیمت حدودی برای مرتب سازی - با محاسبه ست میشود
        /// </summary>
        public long RoughPrice { get; set; }
        public User RegistrarUser { get; set; }
        public int GalleryId { get; set; }
        public Gallery Gallery { get; set; }
        public ICollection<ProductCategory> ProductCategories { get; set; }
        public ICollection<ProductGallery> ProductGalleries { get; set; }
        public ICollection<FavoritProduct> FavoritProducts { get; set; }
        
    }
}
