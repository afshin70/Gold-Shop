using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.Models.SellerModels
{
    public class SellerModel
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

		public int GalleryId { get; set; }
        public string GalleryName { get; set; } = string.Empty;

		public byte ProductRegisterPerMinCount { get; set; }

        public string UserName { get; set; } = string.Empty;

		public bool IsActive { get; set; }
		public bool HasAccessToRegisterProduct { get; set; }
		public bool HasAccessToRegisterLoan { get; set; }
    }
}
