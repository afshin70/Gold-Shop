
using Gold.Domain.Entities.AuthEntities;
using Gold.Domain.Entities.Base;

namespace Gold.Domain.Entities
{
    public class Manager : IEntity<int>
    {
        public int Id { get; set; }
       
        public int UserId { get; set; }
        public User User { get; set; }

        public int PermissionId { get; set; }
        public Permission Permission { get; set; }


    }

}
