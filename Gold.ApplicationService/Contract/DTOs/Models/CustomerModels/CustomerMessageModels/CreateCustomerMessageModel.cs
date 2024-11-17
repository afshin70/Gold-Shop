using Gold.Domain.Enums;
using Gold.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.CustomerMessageModels
{
	public class CreateCustomerMessageModel
	{
		public int CustomerId { get; set; }
		public long? DocumentId { get; set; }
		public string Title { get; set; } = string.Empty;
		public long? installmentId { get; set; }
		public Dictionary<string, string> Parameters { get; set; } = new();
        public CustomerMessageType MessageType { get; set; }
		public SettingType MessageSettingType { get; set; }
	}
}
