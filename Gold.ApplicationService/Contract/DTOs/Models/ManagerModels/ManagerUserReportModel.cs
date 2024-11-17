namespace Gold.ApplicationService.Contract.DTOs.Models.ManagerModels
{
    public class ManagerUserReportModel
    {
        public int ManagerUserId { get; set; }
        public string FullName { get; set; } = string.Empty;
		public string Mobile { get; set; } = string.Empty;
		public string UserName { get; set; } = string.Empty;
		public string AccessLevelTitle { get; set; } = string.Empty;
		public bool IsActive { get; set; }
    }
}
