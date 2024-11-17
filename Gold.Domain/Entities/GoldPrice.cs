using Gold.Domain.Entities.AuthEntities;
using Gold.Domain.Entities.Base;

namespace Gold.Domain.Entities
{
    public class GoldPrice : IEntity<int>
    {
        public int Id { get; set; }
        public DateTime RegisterDate { get; set; }
        /// <summary>
        /// مظنه طلا
        /// </summary>
        public int? Shekel { get; set; }
        /// <summary>
        /// انس طلا
        /// </summary>
        public double? Anas { get; set; }
        public int Karat18 { get; set; }
        public int? Coin { get; set; }
        public int? OldCoin { get; set; }
        public int? HalfCoin { get; set; }
        public int? QuarterCoin { get; set; }
        public int? GramCoin { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

    }

}
