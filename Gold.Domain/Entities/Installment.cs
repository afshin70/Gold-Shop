
using Gold.Domain.Entities.AuthEntities;
using Gold.Domain.Entities.Base;
using Gold.SharedKernel.Enums;

namespace Gold.Domain.Entities
{
    public class Installment : IEntity<long>
    {
        public long Id { get; set; }
        public long DocumentId { get; set; }
        public Document Document { get; set; }
        public DateTime Date { get; set; }
        public long Amount { get; set; }
        public byte Number { get; set; }
        public bool IsPaid { get; set; }
        //public DateTime? PaymentDate { get; set; }
        public int? DelayDays { get; set; }
        //public long PaymentAmount { get; set; }
        public string? Description { get; set; }
        public PaymentType? PaymentType { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public ICollection<CustomerMessage> CustomerMessages { get; set; }

    }

}
