namespace Gold.ApplicationService.Contract.DTOs.Models.SettingsModels
{
    public class SocialNetworkModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
		public string Url { get; set; } = string.Empty;
		public string Image { get; set; } = string.Empty;
	}
}
