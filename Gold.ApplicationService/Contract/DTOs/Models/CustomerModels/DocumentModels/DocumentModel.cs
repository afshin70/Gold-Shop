using Gold.Domain.Entities;
using Gold.Resources;
using Gold.SharedKernel.Enums;
using Gold.SharedKernel.ExtentionMethods;
using Gold.SharedKernel.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.Models.DocumentModels
{
    public class DocumentModel
    {

        public long Id { get; set; }
        public int DocumentNo { get; set; }
        public string FullName { get; set; } = string.Empty;
        public IEnumerable<CollateralInfoModel> Collaterals { get; set; } = new List<CollateralInfoModel>();
        public string CollateralListInfo
        {
            get
            {
                string desc = string.Empty;
                bool firstTime = true;
                foreach (var item in Collaterals)
                {
                    if (!firstTime)
                        desc += "<br/>";
                    desc += item.Type + (string.IsNullOrEmpty(item.Description) ? string.Empty : $" - {item.Description}");
                    firstTime = false;
                }
                return desc;
            }
        }
        public DateTime DocumentDate { get; set; }
        public string PersianDocumentDate
        {
            get
            {
                return DateTimeTools.GeorgianToPersian(DocumentDate, ShowMode.OnlyDate);
            }
        }
        public long InstallmentAmount { get; set; }
        
        public int RemainInstallmentCount { get; set; }

        /// <summary>
        /// تعداد اقساط سررسید
        /// </summary>
        public int RemainDueDateInstallmentCount { get; set; }
        
        public long SumOfRemainAmount { get; set; }
        public string SumOfRemainAmountStatus { get; set; } = string.Empty;





        public string Gallery { get; set; } = string.Empty;
        public string? AdminDescription { get; set; }
        public string Status { get; set; } = string.Empty;
        public DocumentStatus DocStatus { get; set; }

        public bool IsDeletable { get; set; }

        public string RowColor
        {
            get
            {

                string className = string.Empty;
                if (DocStatus == DocumentStatus.Settled)
                    className = "row-green";
                else if (DocStatus == DocumentStatus.Deleted)
                    className = "row-gray";
                else if (RemainDueDateInstallmentCount >= 2)
                    className = "row-red";
                return className;
            }
        }

		public DateTime? SettleDate { get; set; }
		public string PersianSettleDate
		{
			get
			{
                if (SettleDate.HasValue)
                {
                    return DateTimeTools.GeorgianToPersian(SettleDate.Value, ShowMode.OnlyDate);
                }
                else
                {
                    return string.Empty; //Captions.NotSettle;
                }
				
			}
		}
		

    }

    
}
