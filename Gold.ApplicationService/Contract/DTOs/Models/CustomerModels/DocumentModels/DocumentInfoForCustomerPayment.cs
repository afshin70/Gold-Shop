using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.DocumentModels
{
    public class DocumentInfoForCustomerPayment
    {
        public long DocumentId { get; set; }
        public int   DocumentNo { get; set; }
        public string   PersianDocumentDate { get; set; }=string.Empty;
        public long   InstallmentAmount { get; set; }
    }
}
