using Gold.Domain.Entities.Base;

namespace Gold.Domain.Entities
{
    public class EditInformationRequest : IEntity<int>
    {
        public int Id { get ; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string? ImageName { get; set; }
        public string Description { get; set; }
        public DateTime RegisterDate { get; set; }
        public bool IsActive { get; set; }
    }

}
