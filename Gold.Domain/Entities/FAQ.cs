using Gold.Domain.Entities.Base;

namespace Gold.Domain.Entities
{
    public class FAQ : IEntity<int>
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public int OrderNo { get; set; }

        public FAQCategory Category { get; set; }
        public int CategoryId { get; set; }
    }
}
