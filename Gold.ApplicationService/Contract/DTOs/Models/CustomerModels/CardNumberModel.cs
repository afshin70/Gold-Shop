namespace Gold.ApplicationService.Contract.DTOs.Models.CustomerModels
{
    public class CardNumberModel
    {
        public long Id { get; set; }
        public int CustomerId { get; set; }
        public string Owner { get; set; }
        public string Number { get; set; } = string.Empty;
        public int OrderNumber { get; set; }
    }
}
