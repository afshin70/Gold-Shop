using Gold.ApplicationService.Concrete;
using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.FAQModels;
using Gold.ApplicationService.Contract.DTOs.Models.SiteContent;
using Gold.ApplicationService.Contract.DTOs.Models.UserModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.SettingsViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.SiteContent;
using Gold.Domain.Enums;
using Gold.EndPoints.Presentation.InternalService;
using Gold.EndPoints.Presentation.Models;
using Gold.EndPoints.Presentation.Utility.Statics;
using Gold.Infrastracture.Configurations.ValidationAttribute.Authorization;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.Enums;
using Gold.SharedKernel.ExtentionMethods;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using System.Data;

namespace Gold.EndPoints.Presentation.Areas.Dashboard.Controllers
{
	[Area(ControllerConstData.Area_Dashboard)]
	[UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}")]
	public class SiteContentController : Controller
	{
		private readonly ISettingService _settingService;
		private readonly IUserService _userService;
		private readonly IViewRenderService _renderService;
		private readonly IContentManagerService _contentManager;
		private readonly IFileService _fileService;

		public SiteContentController(ISettingService settingService, IViewRenderService renderService, IUserService userService, IContentManagerService contentManager, IFileService fileService)
		{
			_settingService = settingService;
			_renderService = renderService;
			_userService = userService;
			_contentManager = contentManager;
			_fileService = fileService;
		}
		public async Task<IActionResult> Index(CancellationToken cancellationToken)
		{
			SettingType contentType = SettingType.SiteContent_AboutUs;
			ToastrResult<string> result = new();
			string html = string.Empty;
			var contentTypeResult = await _settingService.GetSettingAsync<string>(contentType, cancellationToken);
			var allContentTypes = _settingService.GetSiteContentTypesAsList().Data.Select(x => new SelectListItem
			{
				Selected = (x == contentType),
				Text = x.GetDisplayName(),
				Value = ((int)x).ToString()
			}).ToList();

			SiteContentViewModel model = new()
			{
				Text = contentTypeResult.Data,
				ContentType = contentType,
				ContentTypes = allContentTypes
			};
			//result = ToastrResult<string>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, html);

			return View(model);
		}
		[HttpGet]
		[UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.SiteContent)]
		public async Task<JsonResult> CreateOrEditContent(SettingType input = SettingType.SiteContent_AboutUs, CancellationToken cancellationToken = default)
		{
			ToastrResult<string> result = new();
			string html = string.Empty;
			var contentTypeResult = await _settingService.GetSettingAsync<string>(input, cancellationToken);
			var allContentTypes = _settingService.GetSiteContentTypesAsList().Data.Select(x => new SelectListItem
			{
				Selected = (x == input),
				Text = x.GetDisplayName(),
				Value = ((int)x).ToString()
			}).ToList();

			result = ToastrResult<string>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, contentTypeResult.Data);
			return Json(result);
		}

		[HttpPost]
		[UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.SiteContent)]
		public async Task<JsonResult> CreateOrEditContent(SiteContentViewModel model, CancellationToken cancellationToken)
		{
			ToastrResult<string> result = new();
			string html = string.Empty;

			ModelState.Remove(nameof(model.ContentType));
			ModelState.Remove(nameof(model.ContentTypes));
			model = new()
			{
				Text = model.Text,
				ContentType = model.ContentType,
				ContentTypes = _settingService.GetSiteContentTypesAsList().Data.Select(x => new SelectListItem
				{
					Selected = (x == model.ContentType),
					Text = x.GetDisplayName(),
					Value = x.ToString()
				}).ToList()
			};
			if (!ModelState.IsValid)
			{
				html = await _renderService.RenderViewToStringAsync("_SiteContentForm", model, this.ControllerContext);
				result = ToastrResult<string>.Error(Captions.Error, UserMessages.FormNotValid, html);
				return Json(result);
			}

			model.Text ??= string.Empty;
			var updateResult = await _settingService.UpdateSettingAsync<string>(model.Text, model.ContentType, cancellationToken);
			if (updateResult.IsSuccess)
			{
				var logModel = new LogUserActivityModel()
				{
					ActivityType = AdminActivityType.Edit,
					AdminMenuId = (byte)ManagerPermission.SiteContent,
					DescriptionPattern = string.Format(AdminActivityLogDescriptions.Setting_Edit_SiteContent, $"[{model.ContentType.GetDisplayName()}]"),
					Parameters = null,
					UserId = HttpContext.User.GetUserIdAsInt()
				};
				await _userService.LogUserActivityAsync(logModel, cancellationToken);
				ModelState.Clear();
				model = new()
				{
					Text = updateResult.Data,
					ContentType = model.ContentType,
					ContentTypes = _settingService.GetSiteContentTypesAsList().Data.Select(x => new SelectListItem
					{
						Selected = (x == model.ContentType),
						Text = x.GetDisplayName(),
						Value = x.ToString()
					}).ToList()
				};
				html = await _renderService.RenderViewToStringAsync("_SiteContentForm", model, this.ControllerContext);
				result = ToastrResult<string>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, html);
			}
			else
			{
				html = await _renderService.RenderViewToStringAsync("_SiteContentForm", model, this.ControllerContext);
				result = ToastrResult<string>.Error(Captions.Error, updateResult.Message, html);
			}
			return Json(result);
		}

		#region ContactUs Messages 



		public async Task<IActionResult> Messages()
		{
			return View();
		}

		[HttpGet]
		[UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.ContactUsMessagesManagement)]
		public async Task<JsonResult> RemoveMessage(int id, CancellationToken cancellationToken)
		{

			ToastrResult toastrResult = new ToastrResult<string>();
			var result = await _contentManager.RemoveMessageAsync(id, cancellationToken);
			if (result.IsSuccess)
			{
				var logParams = new Dictionary<string, string>();
				logParams.Add("0", result.Data.SendDatePersian);
				logParams.Add("1", result.Data.FullName);
				var logModel = new LogUserActivityModel()
				{
					ActivityType = AdminActivityType.Delete,
					AdminMenuId = (byte)ManagerPermission.ContactUsMessagesManagement,
					DescriptionPattern = AdminActivityLogDescriptions.ContactUsMessage_Delete,
					Parameters = logParams,
					UserId = HttpContext.User.GetUserIdAsInt()
				};
				await _userService.LogUserActivityAsync(logModel, cancellationToken);

				toastrResult = ToastrResult.Success(Captions.Success, result.Message);
			}
			else
			{
				toastrResult = ToastrResult.Error(Captions.Error, result.Message);
			}
			return Json(toastrResult);
		}

		[UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.ContactUsMessagesManagement)]
		public IActionResult UserMessageList_Read([DataSourceRequest] DataSourceRequest request)
		{
			CommandResult<IQueryable<ContactUsMessageModel>> result = _contentManager.GetContactUsMessagesAsQuerable();
			DataSourceResult dataSource = new DataSourceResult();
			if (result.IsSuccess)
				dataSource = result.Data.OrderByDescending(x => x.SendDate).ToDataSourceResult(request);
			return Json(dataSource);
		}

		#region export Contact Us Messages data

		public JsonResult InitContactUsMessagesReportData()
		{
			return Json(true);
		}

		public IActionResult ExportPdfContactUsMessages()
		{
			List<ContactUsMessageModel> reportData = new List<ContactUsMessageModel>();
			CommandResult<IQueryable<ContactUsMessageModel>> result = _contentManager.GetContactUsMessagesAsQuerable();
			if (result.IsSuccess)
				reportData = result.Data.OrderByDescending(x => x.SendDate).ToList();

			return new ViewAsPdf("ExportPdfContactUsMessages", reportData, null)
			{
				PageSize = Size.A4,
				PageMargins = new Margins(8, 0, 4, 0),
				PageOrientation = Orientation.Landscape,
				FileName = $"ContactUsMessages-{DateTime.Now.ToString("yyyy_MM_dd-HH,mm")}.pdf",
			};
		}
		public IActionResult ExportExcelContactUsMessages()
		{
			List<ContactUsMessageModel> reportData = new List<ContactUsMessageModel>();
			CommandResult<IQueryable<ContactUsMessageModel>> result = _contentManager.GetContactUsMessagesAsQuerable();
			if (result.IsSuccess)
				reportData = result.Data.OrderByDescending(x => x.SendDate).ToList();

			DataTable reportList = new DataTable("ExportExcel");
			reportList.Columns.Add("#");
			reportList.Columns.Add(Captions.FullName);
			reportList.Columns.Add(Captions.PhoneNumber);
			reportList.Columns.Add(Captions.SendDate);
			reportList.Columns.Add(Captions.Message);

			double[] ColumnWidth = new double[reportList.Columns.Count];

			ColumnWidth[0] = 8;
			ColumnWidth[1] = 25;
			ColumnWidth[2] = 25;
			ColumnWidth[3] = 25;
			ColumnWidth[4] = 50;
			int i = 1;
			foreach (var item in reportData)
			{
				reportList.Rows.Add
					(i++,
				   item.FullName,
				   item.Phone,
				   item.SendDatePersian,
				   item.Message
				   );
			}
			byte[] file = _fileService.CreateExcelFileFromDataTable(reportList, ColumnWidth, $"{Captions.List} {Captions.Messages}", true);
			string filename = string.Format("{0}.xlsx", $"ContactUsMessages" + DateTime.Now.ToString("yyyy_MM_dd-HH,mm"));
			Response.Headers.Add("Content-Disposition", string.Format("attachment; filename={0}", filename));
			return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
		}
		#endregion
		#endregion
	}
}
