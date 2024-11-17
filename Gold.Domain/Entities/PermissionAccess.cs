using Gold.Domain.Entities.Base;

namespace Gold.Domain.Entities
{
    public class PermissionAccess : IEntity<int>
    {
        public int Id { get; set; }
        public int PermissionId { get; set; }
        public byte AdminMenuId { get; set; }
        public Permission Permission { get; set; }
        public AdminMenu AdminMenu { get; set; }
    }
}
