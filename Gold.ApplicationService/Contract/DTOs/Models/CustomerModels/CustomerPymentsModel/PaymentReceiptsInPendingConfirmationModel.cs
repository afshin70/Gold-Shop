using Gold.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.CustomerPymentsModel
{
    public class PaymentReceiptsInPendingConfirmationModel
    {
        public long Id { get; set; }

        public int DocumentNumber { get; set; }
        public string FullName { get; set; } = string.Empty;
		public string PersianDocumentDate { get; set; } = string.Empty;
		public long   InstallmentAmount { get; set; }
        public string? ImageName { get; set; } = string.Empty;
		public string PersianRegisterDate { get; set; } = string.Empty;
        public string StatusDescription { get; set; }
        public ConfirmStatusType StatusType { get; set; }
        public long? PayAmount { get; set; }
        public string PersianPayDate { get; set; }
        public string PersianPayTime { get; set; }
    }

    
}
