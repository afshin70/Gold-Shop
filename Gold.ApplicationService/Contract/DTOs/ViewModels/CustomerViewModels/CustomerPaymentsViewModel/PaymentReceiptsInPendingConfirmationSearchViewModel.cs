using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using Gold.SharedKernel.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.CustomerPaymentsViewModel
{
    public class PaymentReceiptsInPendingConfirmationSearchViewModel
    {
        [Display(Name = "DocumentDay", ResourceType = typeof(Captions))]
        [Range(1, 31, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(ValidationMessages))]
        public byte? DocumentDay { get; set; }

        [Display(Name = "DocumentNumber", ResourceType = typeof(Captions))]
        [Range(1, int.MaxValue, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(ValidationMessages))]
        public int? DocumentNumber { get; set; }

        [Display(Name = "RegisterFromDate", ResourceType = typeof(Captions))]
        [PersianDate("/", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? FromDate { get; set; }

        [Display(Name = "RegisterToDate", ResourceType = typeof(Captions))]
        [PersianDate("/", ErrorMessageResourceName = "InvalidFormat", ErrorMessageResourceType = typeof(ValidationMessages))]
        public string? ToDate { get; set; }

        [Display(Name = "Status", ResourceType = typeof(Captions))]
        public ConfirmStatusType? ConfirmStatus { get; set; }
    }
}
