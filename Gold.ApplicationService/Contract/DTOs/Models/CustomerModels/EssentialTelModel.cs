namespace Gold.ApplicationService.Contract.DTOs.Models.CustomerModels
{
    public class EssentialTelModel
    {
        public long Id { get; set; }
        public int CustomerId { get; set; }
        public string? RelationShip { get; set; }
        public string Tel { get; set; } = string.Empty;
        public int OrderNumber { get; set; }
    }    
}
