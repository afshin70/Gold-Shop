using Gold.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.SharedKernel.DTO.Bootstrap
{
    public class BootstrapAlertModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public AlertType Type { get; set; }
    }
    
}
