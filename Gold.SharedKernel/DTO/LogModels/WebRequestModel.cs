using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.SharedKernel.DTO.LogModels
{
    public class WebRequestModel
    {
        public string IP { get; set; }
        public string UserAgent { get; set; }
        public string UserName { get; set; }
        public string Path { get; set; }
        public string Method { get; set; }
        public string QueryString { get; set; }
        public int StatuseCode { get; set; }
    }
}
