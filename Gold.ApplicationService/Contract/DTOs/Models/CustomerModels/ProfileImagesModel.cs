namespace Gold.ApplicationService.Contract.DTOs.Models.CustomerModels
{
    public class ProfileImagesModel
    {
        public long Id { get; set; }
        public int CustomerId { get; set; }
        public string ImageName { get; set; } = string.Empty;
        public DateTime RegisterDate { get; set; }
    }
}
