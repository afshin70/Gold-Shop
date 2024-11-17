using Gold.Domain.Entities.Base;

namespace Gold.Domain.Entities
{
    public class ContactUs : IEntity<int>
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string? Phone { get; set; }
        public string Message { get; set; }
        public DateTime CreateDate { get; set; }
    }

}
