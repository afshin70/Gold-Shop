using Gold.Domain.Entities.Base;

namespace Gold.Domain.Entities
{
    public class ProfileImage : IEntity<long>
    {
        public long Id { get; set; }
        public string ImageName { get; set; }
        public DateTime RegisterDate { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

    }
}
