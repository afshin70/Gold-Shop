namespace Gold.ApplicationService.Contract.DTOs.Models.GoldPriceModels
{
    public class GoldPriceExportPdfModel
    {
        public int Id { get; set; }
        public string RegisterDate { get; set; }
        public string Shekel { get; set; }
        public string Anas { get; set; }
        public string Karat18 { get; set; }
        public string Coin { get; set; }
        public string OldCoin { get; set; }
        public string HalfCoin { get; set; }
        public string QuarterCoin { get; set; }
        public string GramCoin { get; set; }
        public string UserName { get; set; }
    }
}
