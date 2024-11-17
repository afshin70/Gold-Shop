
using Gold.Domain.Entities.AuthEntities;
using Gold.Domain.Entities.Base;
using Gold.SharedKernel.Enums;

namespace Gold.Domain.Entities
{
    public class CustomerPayment : IEntity<long>
    {
        public long Id { get; set; }
        public long DocumentId { get; set; }
        public Document Document { get; set; }
        public string? ImageName { get; set; }
        public DateTime RegisterDate { get; set; }


        public DateTime? PayDate { get; set; }
        public long? PayAmount { get; set; }

        public string? Description { get; set; }
        public string? AdminDescription { get; set; }
        public ConfirmStatusType ConfirmStatus { get; set; }
        public DateTime? ConfirmDate { get; set; }

        public ICollection<Payment> Payments { get; set; }

    }

}
