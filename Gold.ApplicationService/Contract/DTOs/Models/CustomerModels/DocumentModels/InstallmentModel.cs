using Gold.Domain.Entities;
using Gold.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.Models.DocumentModels
{
    public class InstallmentModel
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime? PaymentDate { get; set; }
        public int? DelayDays { get; set; }
        public long PaymentAmount { get; set; }
        public bool IsPaid { get; set; }
        public string? Description { get; set; }
        public DocumentStatus DocumentStatus { get; set; }

    }
}
