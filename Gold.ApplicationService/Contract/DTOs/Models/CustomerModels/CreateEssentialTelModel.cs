namespace Gold.ApplicationService.Contract.DTOs.Models.CustomerModels
{
    public class CreateEssentialTelModel
    {
        public string? RelationShip { get; set; }
        public int OrderNo { get; set; }
        public string Tel { get; set; } = string.Empty;
    }
    
}
