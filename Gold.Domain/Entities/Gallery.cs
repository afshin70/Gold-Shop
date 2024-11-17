using Gold.Domain.Entities.Base;

namespace Gold.Domain.Entities
{
    public class Gallery : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        public string? Tel { get; set; }
        public bool IsActive { get; set; }
        public bool HasInstallmentSale { get; set; }
        public string? PurchaseDescription { get; set; }

        public ICollection<Seller> Sellers { get; set; }
        public ICollection<Document> Documents { get; set; }
        public ICollection<Product> Products { get; set; }

    }
}
