namespace Gold.SharedKernel.Enums
{
    public enum DocumentPaymentStatus : byte
    {
        /// <summary>
        /// قسط پرداخت نشده عقب افتاد-اولویت اوله
        /// </summary>
        UnpaidOverdueInstallment=1,

        /// <summary>
        /// سند تسویه نشده با پرداخت در انتظار تایید
        /// </summary>
        DocumentWithPaymentPending=2,
        /// <summary>
        /// سند تسویه نشده بدون قسط عقب افتاده
        /// </summary>
        UnsettledDocumentWithOutOverdueInstallments,

    }
}
