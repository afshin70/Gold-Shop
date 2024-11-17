namespace Gold.ApplicationService.Contract.DTOs.ViewModels.AuthenticationViewModels
{
    public class AdminMenuModel
    {
        public byte Id { get; set; }
        public byte MenuGroupId { get; set; }
        public string Title { get; set; } = string.Empty;
		public string IconName { get; set; } = string.Empty;
		public string ControllerName { get; set; } = string.Empty;
		public string ActionName { get; set; } = string.Empty;
	}
}
