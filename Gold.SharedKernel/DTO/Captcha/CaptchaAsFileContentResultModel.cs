using Microsoft.AspNetCore.Mvc;

namespace Gold.SharedKernel.DTO.Captcha
{
    public class CaptchaAsFileContentResultModel
    {
        public FileContentResult Content { get; set; }
        public string CaptchaSessionName { get; set; }
        public string CaptchaSessionValue { get; set; }
    }
}
