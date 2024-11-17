namespace Gold.ApplicationService.Contract.DTOs.Models.AccessLevelModels
{
    public class AdminMenuGroupsModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
		public List<AdminMenusModel> AdminMenus { get; set; } = new();
    }
}
