using Gold.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.DocumentModels
{
    public class DocumentInfoModel
    {
        public long Id { get; set; }
        public int Number { get; set; }
        public string PersianDate { get; set; } = string.Empty;
        public DocumentStatus  Status { get; set; }
        public bool HasCollateralImageUrl { get; set; }

        public DocumentPaymentStatus? DocumentPaymentState { get; set; }
    }
}
