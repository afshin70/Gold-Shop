using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.SharedKernel.DTO.Captcha
{
    public class CaptchaAsBase64ContentModel
    {
        public string Content { get; set; }
        public string CaptchaSessionName { get; set; }
        public string CaptchaSessionValue { get; set; }
    }
}
