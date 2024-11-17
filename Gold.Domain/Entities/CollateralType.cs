using Gold.Domain.Entities.Base;
using Gold.Domain.Enums;

namespace Gold.Domain.Entities
{
    public class CollateralType : IEntity<int>
    {
        public int Id { get ; set; }
        public string Title { get; set; }

        public IEnumerable<Collateral> Collaterals { get; set; }
    }

}
