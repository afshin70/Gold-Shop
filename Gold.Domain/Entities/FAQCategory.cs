using Gold.Domain.Entities.Base;

namespace Gold.Domain.Entities
{
    public class FAQCategory : IEntity<int>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int OrderNo { get; set; }

        public ICollection<FAQ> FAQs { get; set; }
    }
}
