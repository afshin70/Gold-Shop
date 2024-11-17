namespace Gold.ApplicationService.Contract.DTOs.Models.SellerModels
{
    public class SellerReportModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } 
        public string GalleryName { get; set; }
        public byte ProductRegisterPerMinCount { get; set; }
        public string UserName { get; set; }
        public bool IsActive { get; set; }
        public string Mobile { get; set; }
        public bool HasAccessToRegisterLoan { get; set; }
        public bool HasAccessToRegisterProduct { get; set; }
    }
}
