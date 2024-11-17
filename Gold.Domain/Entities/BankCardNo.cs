using Gold.Domain.Entities.Base;

namespace Gold.Domain.Entities
{
	public class BankCardNo : IEntity<long>
    {
		public long Id { get; set; }
        /// <summary>
        ///  شماره کارت
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// صاحب شماره کارت
        /// </summary>
        public string Owner { get; set; }
        public int OrderNo { get; set; }
		public int CustomerId { get; set; }
		public Customer Customer { get; set; }

	}
}
