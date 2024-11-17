
using Gold.Domain.Entities.AuthEntities;
using Gold.Domain.Entities.Base;

namespace Gold.Domain.Entities
{
    public class AdminMenuGroup : IEntity<byte>
    {
        public byte Id { get; set; }
        public string Title { get; set; }
        public byte OrderNo { get; set; }
        public string IconName { get; set; }

        public ICollection<AdminMenu> AdminMenus { get; set; }


    }

}
