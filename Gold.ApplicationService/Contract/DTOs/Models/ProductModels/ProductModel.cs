using Gold.Domain.Entities.AuthEntities;
using Gold.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gold.Domain.Enums;
using Gold.SharedKernel.ExtentionMethods;

namespace Gold.ApplicationService.Contract.DTOs.Models.ProductModels
{
    public class ProductModel
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
        public bool IsSold
        {
            get
            {
                return Status == ProductStatus.Sold;
            }
        }
        public ProductStatus Status { get; set; }
        public string StatusTitle => Status.GetDisplayName();
        public string? Description { get; set; }
        public long StonPrice { get; set; }
        public DateTime RegisterDate { get; set; }
        public string RegisterDatePersian { get; set; }
        public string RegistrarUserName { get; set; }
        public int GalleryId { get; set; }
        public string GalleryName { get; set; }
        public List<int>? CategoryIds { get; set; }
        public List<string> CategoryTitles { get; set; }
    }
}
