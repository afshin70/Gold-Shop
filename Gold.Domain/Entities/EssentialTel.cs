using Gold.Domain.Entities.Base;

namespace Gold.Domain.Entities
{
    public class EssentialTel : IEntity<long>
    {
        public long Id { get; set; }
        public string? RelationShip { get; set; }
        public int OrderNo { get; set; }
        public string Tel { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

    }
}
