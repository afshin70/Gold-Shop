namespace Gold.ApplicationService.Contract.DTOs.Models.CityModels
{
    public class CityModel
    {
        public int Id { get; set; }
        public int ProvinceId { get; set; }
        public string Title { get; set; } = string.Empty;
    }
}
