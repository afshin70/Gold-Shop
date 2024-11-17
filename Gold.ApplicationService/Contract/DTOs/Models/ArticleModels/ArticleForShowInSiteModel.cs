namespace Gold.ApplicationService.Contract.DTOs.Models.ArticleModels
{
    public class ArticleForShowInSiteModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ImageFileName { get; set; } = string.Empty;
        public string VideoFileName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool HasVideo { get; set; }
    }
}
