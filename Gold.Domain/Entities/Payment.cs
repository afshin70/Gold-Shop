
using Gold.Domain.Entities.AuthEntities;
using Gold.Domain.Entities.Base;
using Gold.SharedKernel.Enums;

namespace Gold.Domain.Entities
{
    public class Payment : IEntity<long>
    {
        public long Id { get; set; }
        public long InstallmentId { get; set; }
        public Installment Installment { get; set; }
        public DateTime Date { get; set; }
        public long Amount { get; set; }
        public DateTime RegisterDate { get; set; }
        public long? CustomerPaymentId { get; set; }
        public CustomerPayment CustomerPayment { get; set; }
        public string? ImageName { get; set; }
                  


    }

}
