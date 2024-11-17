using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.Models.SiteContent
{
    public class ContactUsMessageModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string? Phone { get; set; }
        public string Message { get; set; }
        public DateTime SendDate { get; set; }
        public string SendDatePersian { get; set; }

    }
}
