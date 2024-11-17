namespace Gold.ApplicationService.Contract.DTOs.Models.DocumentModels
{

    public class CollateralModel
    {
        public long Id { get; set; }
        public long DocumentId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageName { get; set; }
    }


}
