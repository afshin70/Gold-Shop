using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.Models.GalleryModels
{
    public class GalleryModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

		public bool IsActive { get; set; }

        public bool HasInstallmentSale { get; set; }
    } 
}
