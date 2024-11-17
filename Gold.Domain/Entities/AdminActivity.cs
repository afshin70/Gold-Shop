
using Gold.Domain.Entities.AuthEntities;
using Gold.Domain.Entities.Base;
using Gold.SharedKernel.Enums;

namespace Gold.Domain.Entities
{
    public class AdminActivity : IEntity<long>
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public byte AdminMenuID { get; set; }
        public AdminMenu AdminMenu { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public AdminActivityType ActivityType { get; set; }

    }

}
