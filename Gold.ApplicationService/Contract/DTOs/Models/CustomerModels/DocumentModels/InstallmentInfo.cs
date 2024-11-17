using Gold.SharedKernel.Enums;

namespace Gold.ApplicationService.Contract.DTOs.Models.DocumentModels
{
	public class InstallmentInfo
    {
        public DocumentStatus DocumentStatus { get; set; }

        /// <summary>
        /// مبلغ پرداخت شده
        /// </summary>
        public long TotalPayedAmount { get; set; }
        /// <summary>
        /// مبلغ مانده اقساط
        /// </summary>
        public long InstallmentRemainAmount { get; set; }
        /// <summary>
        /// دیرکرد (روز)
        /// </summary>
        public int TotalDelayDay { get; set; }
        /// <summary>
        /// دیرکرد تا امروز
        /// </summary>
        public int TodayTotalDelayDay { get; set; }
        /// <summary>
        /// مبلغ دیرکرد
        /// ---
        /// ضریب دیرکرد * مبلغ قسط * مجموع روزهای دیرکرد
        /// </summary>
        public long DelayAmount { get; set; }
        /// <summary>
        /// مبلغ دیرکرد تا امروز
        /// ----
        /// مبلغ دیرکرد +دیرکرد اقساط پرداخت نشده عقب افتاده تا امروز
        /// </summary>
        public long TodayDelayAmount { get; set; }
        /// <summary>
        /// مبلغ بازگشت خورده
        /// </summary>
        public long ReturnedAmount { get; set; }
        /// <summary>
        /// مبلغ تخفیف
        /// </summary>
        public long DiscountAmount { get; set; }
        /// <summary>
        /// مبلغ تسویه فوری
        /// </summary>
        public long InstantSettlementAmount { get; set; }

        /// <summary>
        ///مبلغ مانده کل 
        /// </summary>
        public long TotalRemainAmount { get; set; }
        /// <summary>
        /// مبلغ مانده کل تا امروز
        /// </summary>
        public long TodayTotalRemainAmount { get; set; }
        /// <summary>
        /// تاریخ تحویل
        /// </summary>
        public DateTime? DeliveryDate { get; set; }
        /// <summary>
        /// تاریخ تسویه
        /// </summary>
        public DateTime? SettleDate { get; set; }
    }
}
