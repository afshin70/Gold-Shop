namespace Gold.ApplicationService.Contract.DTOs.Models.CustomerModels
{
    public class CustomerReportModel
    {
        public int Id { get; set; }
        public string Gender { get; set; }
        public string FullName { get; set; } 
        public string Nationaity { get; set; }
        public string NationalCode { get; set; } 
        public string Mobile { get; set; } 
        public bool IsActive { get; set; }
        public string? FatherName { get; set; }
        public string? PostalCode { get; set; }
        public string? Address { get; set; }
        public string? JobTitle { get; set; }
        public string?  CityName { get; set; }
        public string? SanaCode { get; set; }
        public string? BirthDate { get; set; }
        public string? EssentialTel { get; set; }
        public string? RelationShip { get; set; }
        public string AccountStatus { get; set; }
        public string BankCardNo { get; set; }
        public string BankCardOwnerName { get; set; }
    }
}
