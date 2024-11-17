namespace Gold.ApplicationService.Contract.DTOs.Models.GoldPriceModels
{
    public class GoldPriceInfoModel
    {
        public DateTime RegisterDate { get; set; }
        /// <summary>
        /// مظنه طلا
        /// </summary>
        public int? Shekel { get; set; }
        /// <summary>
        /// انس طلا
        /// </summary>
        public double? Anas { get; set; }
        public int Karat18 { get; set; }
        public int? Coin { get; set; }
        public int? OldCoin { get; set; }
        public int? HalfCoin { get; set; }
        public int? QuarterCoin { get; set; }
        public int? GramCoin { get; set; }
    }
}
