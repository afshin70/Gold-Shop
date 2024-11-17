
using Gold.Domain.Entities.AuthEntities;
using Gold.Domain.Entities.Base;
using Gold.SharedKernel.Enums;

namespace Gold.Domain.Entities
{
    public class Collateral : IEntity<long>
    {
        public long Id { get; set; }
        public long DocumentId { get; set; }
        public Document Document { get; set; }
        public string? ImageName { get; set; }
        public string? Description { get; set; }
        public int CollateralTypeId { get; set; }
        public CollateralType CollateralType { get; set; }
       
    }

}
