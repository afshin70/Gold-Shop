
using Gold.Domain.Entities.AuthEntities;
using Gold.Domain.Entities.Base;
using Gold.SharedKernel.Enums;

namespace Gold.Domain.Entities
{
    public class Document : IEntity<long>
    {
        public long Id { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int GalleryId { get; set; }
        public Gallery Gallery { get; set; }
        public int SellerId { get; set; }
        public Seller Seller { get; set; }
        public int DocumentNo { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime RegisterDate { get; set; }
        public long PrepaymentAmount { get; set; }
        public long RemainAmount { get; set; }
        public byte InstallmentCount { get; set; }
        public long InstallmentAmount { get; set; }
        public DocumentStatus Status { get; set; }
        public byte DayOfMonth { get; set; }
        public string? AdminDescription { get; set; }
        public long? DelayAmount { get; set; }
        public long DiscountAmount { get; set; }
        public DateTime? SettleDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? SettleRegisterDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public long? ReturnedAmount { get; set; }
        public ICollection<Installment> Installments { get; set; }
        public ICollection<Collateral> Collaterals { get; set; }
        public ICollection<CustomerPayment> CustomerPayments { get; set; }
        public ICollection<CustomerMessage> CustomerMessages { get; set; }
    }

}
