namespace Gold.ApplicationService.Contract.DTOs.Models.AccessLevelModels
{
    public class AccessLevelReportModel
    {
        public int Id { get; set; }
        public string Title { get; set; } 
        public List<string> PermissionsTitle { get; set; }

    }
}
