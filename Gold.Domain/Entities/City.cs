using Gold.Domain.Entities.Base;

namespace Gold.Domain.Entities
{
    public class City : IEntity<int>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? ParentId { get; set; }
        public ICollection<Customer> Customers { get; set; }
    }
}