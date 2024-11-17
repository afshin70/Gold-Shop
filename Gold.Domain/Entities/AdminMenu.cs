
using Gold.Domain.Entities.AuthEntities;
using Gold.Domain.Entities.Base;

namespace Gold.Domain.Entities
{
    public class AdminMenu : IEntity<byte>
    {
        public byte Id { get; set; }
        public string Title { get; set; }
        public byte OrderNo { get; set; }
        public string IconName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }


        public byte AdminMenuGroupID { get; set; }
        public AdminMenuGroup AdminMenuGroup { get; set; }

        public ICollection<PermissionAccess> PermissionAccesses { get; set; }

    }

}
