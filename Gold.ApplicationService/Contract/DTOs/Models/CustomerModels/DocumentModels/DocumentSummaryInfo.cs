using Gold.SharedKernel.Enums;

namespace Gold.ApplicationService.Contract.DTOs.Models.DocumentModels
{
    public class DocumentSummaryInfo
    {
        public long Id { get; set; }
        public int DocumentNo { get; set; }
        public DocumentStatus? Status { get; set; }
        public int NotPaidInstallmentCount { get; set; }
        public string RowColor
        {
            get
            {

                string className = string.Empty;
                if (Status == DocumentStatus.Settled)//تسویه شده
                    className = "btn-success";
                else if (Status == DocumentStatus.Active& NotPaidInstallmentCount >= 1)//قسط عقب افتاده
                    className = "btn-danger";
                else if (Status == DocumentStatus.Active)
                    className = "btn-info";
                return className;
            }
        }
    }
}
