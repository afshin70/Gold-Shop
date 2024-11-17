using Gold.Domain.Entities.Base;

namespace Gold.Domain.Entities
{
    public class Category : IEntity<int>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int OrderNo { get; set; }
        public ICollection<ProductCategory> ProductCategories { get; set; }
    }
}
