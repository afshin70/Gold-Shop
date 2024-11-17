using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.DocumentViewModels
{
	public class EditDocumentViewModel
	{
		public long DocumentId { get; set; }

		public string? FullName { get; set; }

		[Display(Name = "DocumentDate", ResourceType = typeof(Captions))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
		[PersianDate("/", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
		public string DocumentDate { get; set; }=string.Empty;

		[Display(Name = "DocumentNumber", ResourceType = typeof(Captions))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
		[Range(1, int.MaxValue, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(ValidationMessages))]
		public int DocumentNo { get; set; }

		public int? OldDocumentNo { get; set; }

		[Display(Name = "Remain", ResourceType = typeof(Captions))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
		[MonyFormat(",", ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(ValidationMessages))]
		public string? RemainAmount { get; set; }

		[Display(Name = "Prepayment", ResourceType = typeof(Captions))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
		[MonyFormat(",", ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(ValidationMessages))]
		public string? PrepaymentAmount { get; set; }

		/// <summary>
		/// مبلغ فاکتور
		/// </summary>
		public long InvoiceAmount
		{
			get
			{
				long value1 = 0;
				long value2 = 0;
				if (!string.IsNullOrEmpty(PrepaymentAmount))
					long.TryParse(PrepaymentAmount.Replace(",", ""), out value1);
				if (!string.IsNullOrEmpty(RemainAmount))
					long.TryParse(RemainAmount.Replace(",", ""), out value2);
				return value1 + value2;
			}
		}

		[Display(Name = "InstallmentCount", ResourceType = typeof(Captions))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
		[Range(1, byte.MaxValue, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(ValidationMessages))]
		public byte InstallmentCount { get; set; }

		[Display(Name = "InstallmentAmount", ResourceType = typeof(Captions))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
		[MonyFormat(",", ErrorMessageResourceName = "Invalid", ErrorMessageResourceType = typeof(ValidationMessages))]
		public string? InstallmentAmount { get; set; }

		[Display(Name = "Gallery", ResourceType = typeof(Captions))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
		public int? GalleryId { get; set; }
		[Display(Name = "Seller", ResourceType = typeof(Captions))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationMessages))]
		public int? SellerId { get; set; }


		public List<SelectListItem>? Sellers { get; set; }
		public List<SelectListItem>? Galleries { get; set; }
	}
}
