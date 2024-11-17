using Gold.Domain.Entities.Base;
using Gold.Domain.Enums;
using Gold.SharedKernel.Enums;

namespace Gold.Domain.Entities
{
    public class CustomerMessage : IEntity<long>
    {
        public long Id { get ; set; }
        public int CustomerId { get; set; }
        public long? DocumentId { get; set; }
        public string Title { get; set; }
        public CustomerMessageType Type { get; set; }
        public long? installmentId { get; set; }
        public DateTime Date { get; set; }
        public string Message { get; set; }
        public Customer Customer { get; set; }
        public Installment?  Installment { get; set; }
        public Document Document { get; set; }
    }

}
