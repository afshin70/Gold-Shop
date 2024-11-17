using Gold.Domain.Entities.Base;

namespace Gold.Domain.Entities
{
    public class FavoritProduct : IEntity<int>
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public long ProductId { get; set; }
        public Product Product { get; set; }
    }

}
