namespace Gold.ApplicationService.Contract.DTOs.Models.CustomerModels
{
    public class BirthdayMessageModel
    {
        public string Gender { get; set; }
        public string FullName { get; set; }
        public string? Year { get; set; }
        public string MonthName { get; set; }
        public string? Month { get; set; }
        public string DayName { get; set; }
        public string? Day { get; set; }
        public int CustomerId { get; set; }
    }
}
