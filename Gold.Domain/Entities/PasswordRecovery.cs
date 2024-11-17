
using Gold.Domain.Entities.AuthEntities;
using Gold.Domain.Entities.Base;
using Gold.SharedKernel.Enums;

namespace Gold.Domain.Entities
{
    public class SendSmsTemp : IEntity<int>
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public Guid Token { get; set; }
        public DateTime SendDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public SendSmsType  Type { get; set; }
        public string UserName { get; set; }
        public string Mobile { get; set; }
    }

}
