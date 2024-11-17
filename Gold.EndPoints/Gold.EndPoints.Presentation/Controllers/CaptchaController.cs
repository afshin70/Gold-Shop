using Gold.SharedKernel.Contract;
using Microsoft.AspNetCore.Mvc;

namespace Gold.EndPoints.Presentation.Controllers
{
    public class CaptchaController : Controller
    {
        private readonly ICaptchaResolver _captchaResolver;

        public CaptchaController(ICaptchaResolver captchaResolver)
        {
            _captchaResolver = captchaResolver;
        }

        [Route("/Captcha/GetImage/{pageName}")]
        public FileContentResult GetImage(string pageName="captchaPage")
        {
            var captchaImage = _captchaResolver.GenerateCaptchaAsFileContentResult(pageName);
            if (captchaImage is not null)
            {
                HttpContext.Session.SetString(captchaImage.CaptchaSessionName, captchaImage.CaptchaSessionValue);
                return captchaImage.Content;
            }
            return new FileContentResult(new byte[] { }, "img/jpeg");
        }
    }
}
