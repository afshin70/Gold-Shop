using Gold.EndPoints.Presentation.Models;
using Gold.Resources;
using Gold.SharedKernel.Enums;
using Gold.SharedKernel.ExtentionMethods;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Gold.EndPoints.Presentation.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public ErrorController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

		
		
		[HttpGet]
		[Route("/AccessDenied")]
		public ActionResult AccessDenied()
		{
			//if (Request.IsAjaxRequest()) return PartialView();
			return View();
		}

        [HttpGet]
        [Route("/NotFound")]
        public ActionResult Error404(string message)
		{
			if (string.IsNullOrEmpty(message))
				ViewBag.Message = UserMessages.PageNotFound;
			else
				ViewBag.Message = message;
			if (Request.IsAjaxRequest()) PartialView();
			return View();
		}

        [HttpGet]
        [Route("/Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}