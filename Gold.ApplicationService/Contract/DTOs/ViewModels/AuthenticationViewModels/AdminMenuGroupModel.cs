namespace Gold.ApplicationService.Contract.DTOs.ViewModels.AuthenticationViewModels
{
    public class AdminMenuGroupModel
    {
        public byte Id { get; set; }
        public string Title { get; set; } = string.Empty;
		public string IconName { get; set; } = string.Empty;
        public List<AdminMenuModel> AdminMenus { get; set; } = new();
    }
}
