using Gold.Domain.Entities.Base;

namespace Gold.Domain.Entities
{
    public class Permission:IEntity<int>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<PermissionAccess> PermissionAccesses { get; set; }

    }
}
