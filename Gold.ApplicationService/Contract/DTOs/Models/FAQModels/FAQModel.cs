namespace Gold.ApplicationService.Contract.DTOs.Models.FAQModels
{
    public class FAQModel
	{
		public int Id { get; set; }
		public string Question { get; set; }
		public string Answer { get; set; }
		public int OrderNo { get; set; }
		public int CategoryId { get; set; }
		public string Category { get; set; }
	}
}
