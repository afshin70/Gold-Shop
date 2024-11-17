using Gold.Domain.Entities.Base;
using Gold.Domain.Enums;

namespace Gold.Domain.Entities
{
    public class Setting : IEntity<int>
    {
        public int Id { get ; set; }
        public SettingType Type { get; set; }
        public string Value { get; set; }
    }
}
