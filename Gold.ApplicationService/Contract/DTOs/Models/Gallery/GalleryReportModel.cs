namespace Gold.ApplicationService.Contract.DTOs.Models.GalleryModels
{
    public class GalleryReportModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
		public bool IsActive { get; set; }
        public bool HasInstallmentSale { get; set; }
        public string? Tel { get; set; }
        public string? PurchaseDescription { get; set; }
        public string? Address { get; set; }
    }
}
