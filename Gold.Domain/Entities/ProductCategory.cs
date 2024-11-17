using Gold.Domain.Entities.Base;

namespace Gold.Domain.Entities
{
    public class ProductCategory : IEntity<long>
    {
        public long Id { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public long ProductId { get; set; }
        public Product Product { get; set; }
    }
}
