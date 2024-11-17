namespace Gold.ApplicationService.Contract.DTOs.Models.UserModels
{
    public class EditInformationRequestModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }=string.Empty;
        public string NationalCode { get; set; } = string.Empty;
		public string PersianRequestDate { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
		public string? ImageName { get; set; }
        public bool IsActive { get; set; }
    }
}
