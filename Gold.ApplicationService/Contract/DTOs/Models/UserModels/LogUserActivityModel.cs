using Gold.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.Models.UserModels
{
    public class LogUserActivityModel
    {
        public int UserId { get; set; }
        public byte AdminMenuId { get; set; }
        public AdminActivityType ActivityType { get; set; }
        public string DescriptionPattern { get; set; } = string.Empty;
        public Dictionary<string, string>? Parameters { get; set; } 
    }
}
