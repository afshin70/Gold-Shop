using Gold.Domain.Entities.Base;
using Gold.Domain.Enums;

namespace Gold.Domain.Entities
{
    public class Article : IEntity<int>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? ImageFileName { get; set; }
        public string? VideoFileName { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; }
        public DateTime RegisterDate { get; set; }
        public int OrderNo { get; set; }
    }
}
