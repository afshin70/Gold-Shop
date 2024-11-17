using Gold.ApplicationService.Concrete;
using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.ArticleModels;
using Gold.ApplicationService.Contract.DTOs.Models.FAQModels;
using Gold.ApplicationService.Contract.DTOs.Models.UserModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.ArticleViewModels;
using Gold.EndPoints.Presentation.Models;
using Gold.EndPoints.Presentation.Utility.Statics;
using Gold.Infrastracture.Configurations.ValidationAttribute.Authorization;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.FileAddress;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.Enums;
using Gold.SharedKernel.ExtentionMethods;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Rotativa.AspNetCore.Options;
using Rotativa.AspNetCore;
using System.Data;
using System.IO;
using Gold.ApplicationService.Contract.DTOs.ViewModels.ProductViewModels.ProdctGalleryViewModels;
using Gold.EndPoints.Presentation.InternalService;
using Gold.ApplicationService.Contract.DTOs.Models.AuthenticationModels;

namespace Gold.EndPoints.Presentation.Areas.Dashboard.Controllers
{
    [Area(ControllerConstData.Area_Dashboard)]
    [UserAccess($"{nameof(UserType.Manager)},{nameof(UserType.Admin)}", ManagerPermission.Article)]
    public class ArticleController : Controller
    {
        private readonly IContentManagerService _contentManagerService;
        private readonly IUserService _userService;
        private readonly IFileService _fileService;
        private readonly FilePathAddress _filePathAddress;
        private readonly IViewRenderService _renderService;

        public ArticleController(IContentManagerService contentManagerService, IUserService userService, IFileService fileService, IOptions<FilePathAddress> filePathAddressOptions, IViewRenderService renderService)
        {
            _contentManagerService = contentManagerService;
            _userService = userService;
            _fileService = fileService;
            _filePathAddress = filePathAddressOptions.Value;
            _renderService = renderService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> CreateOrEditArticel(int? id, CancellationToken cancellationToken = default)
        {
            ToastrResult<CreateOrEditArticleViewModel> result = new();
            CreateOrEditArticleViewModel model = new();
            if (id.HasValue)
            {
                //string path = _filePathAddress.SellerProfileImage;
                CommandResult<ArticleModel> commandResult = await _contentManagerService.GetArticleByIdAsync(id.Value, cancellationToken);
                model = new()
                {
                    Id = commandResult.Data.Id,
                    Title = commandResult.Data.Title,
                    Description = commandResult.Data.Description,
                };
            }

            result = ToastrResult<CreateOrEditArticleViewModel>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, model);
            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> CreateOrEditArticel(CreateOrEditArticleViewModel model, CancellationToken cancellationToken)
        {
            ToastrResult result = new();
            string html = string.Empty;

            if (!ModelState.IsValid)
            {
                result = ModelState.GetAllErrorMessagesAsToast(ToastrType.Error, false, UserMessages.FormNotValid);
                return Json(result);
            }

            var addOrEditResult = await _contentManagerService.CreateOrEditArticleAsync(model, cancellationToken);
            if (addOrEditResult.IsSuccess)
            {
                var logParams = new Dictionary<string, string>();
                logParams.Add("0", model.Title);
                var logModel = new LogUserActivityModel()
                {
                    AdminMenuId = (byte)ManagerPermission.Article,
                    Parameters = logParams,
                    UserId = HttpContext.User.GetUserIdAsInt()
                };
                if (model.Id.HasValue)
                {
                    logModel.ActivityType = AdminActivityType.Edit;
                    logModel.DescriptionPattern = AdminActivityLogDescriptions.Article_Edit;
                }
                else
                {
                    logModel.ActivityType = AdminActivityType.Insert;
                    logModel.DescriptionPattern = AdminActivityLogDescriptions.Article_Insert;
                }
                await _userService.LogUserActivityAsync(logModel, cancellationToken);
                result = ToastrResult.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted);
            }
            else
                result = ToastrResult.Error(Captions.Error, addOrEditResult.Message);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetArticleImage(string? imageName)
        {
            string patch = string.Empty;
            try
            {
                patch = $"{_filePathAddress.ArticleImages}/{imageName}";
                var fileBytes = await _fileService.GetFileBytesAsync(patch);
                var fileContentType = _fileService.GetMimeTypeForFileExtension(patch);
                return File(fileBytes, fileContentType);
            }
            catch (Exception)
            {
                patch = $"{_filePathAddress.CustomerPaymentImages}/NoImage.png";
                var fileBytes = await _fileService.GetFileBytesAsync(patch);
                var fileContentType = _fileService.GetMimeTypeForFileExtension(patch);
                return File(fileBytes, fileContentType);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetArticleVideo(string? videoName)
        {
            string patch = string.Empty;
            try
            {
                patch = $"{_filePathAddress.ArticleVideos}/{videoName}";
                var fileBytes = await _fileService.GetFileBytesAsync(patch);
                var fileContentType = _fileService.GetMimeTypeForFileExtension(patch);
                return File(fileBytes, fileContentType);
            }
            catch (Exception)
            {
                patch = $"{_filePathAddress.ArticleVideos}/NoImage.png";
                var fileBytes = await _fileService.GetFileBytesAsync(patch);
                var fileContentType = _fileService.GetMimeTypeForFileExtension(patch);
                return File(fileBytes, fileContentType);
            }
        }


        public IActionResult ArticleList_Read([DataSourceRequest] DataSourceRequest request)
        {
            CommandResult<IQueryable<ArticleModel>> result = _contentManagerService.GetArticleAsQuerable();
            DataSourceResult dataSource = new DataSourceResult();
            if (result.IsSuccess)
                dataSource = result.Data.OrderByDescending(x=>x.OrderNo).ToDataSourceResult(request);
            return Json(dataSource);
        }

        public async Task<JsonResult> ChangeArticleOrder(int id, bool isUp, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            var result = await _contentManagerService.ChangeArticleOrderNumberAsync(id, isUp, cancellationToken);
            string html = string.Empty;
            if (result.IsSuccess)
                toastrResult = ToastrResult<string>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, html);
            else
                toastrResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
            return Json(toastrResult);
        }

        public async Task<JsonResult> ChangeArticleStatus(int id, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            var result = await _contentManagerService.ChangeArticleStatusAsync(id, cancellationToken);
            string html = string.Empty;
            if (result.IsSuccess)
            {
                var logParams = new Dictionary<string, string>();
                logParams.Add("0", result.Data);
                var logModel = new LogUserActivityModel()
                {
                    ActivityType = AdminActivityType.Edit,
                    AdminMenuId = (byte)ManagerPermission.Article,
                    DescriptionPattern = AdminActivityLogDescriptions.Article_StatusChanged,
                    Parameters = logParams,
                    UserId = HttpContext.User.GetUserIdAsInt()
                };
                await _userService.LogUserActivityAsync(logModel, cancellationToken);

                toastrResult = ToastrResult<string>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, html);
            }
            else
                toastrResult = ToastrResult<string>.Error(Captions.Error, result.Message, html);
            return Json(toastrResult);
        }

        [HttpGet]
        public async Task<JsonResult> RemoveArticle(int id, CancellationToken cancellationToken)
        {
            ToastrResult toastrResult = new ToastrResult();
            CommandResult<string> result = await _contentManagerService.RemoveArticleAsync(id, cancellationToken);
            if (result.IsSuccess)
            {
                var logParams = new Dictionary<string, string>();
                logParams.Add("0", result.Data);
                var logModel = new LogUserActivityModel()
                {
                    ActivityType = AdminActivityType.Delete,
                    AdminMenuId = (byte)ManagerPermission.Article,
                    DescriptionPattern = AdminActivityLogDescriptions.Article_Delete,
                    Parameters = logParams,
                    UserId = HttpContext.User.GetUserIdAsInt()
                };
                await _userService.LogUserActivityAsync(logModel, cancellationToken);

                toastrResult = ToastrResult.Success(Captions.Success, result.Message);
            }
            else
                toastrResult = ToastrResult.Error(Captions.Error, result.Message);
            return Json(toastrResult);
        }


        [HttpGet]
        public async Task<IActionResult> ChangeArticleImageOrVideo(int? id,bool isVideo=false, CancellationToken cancellationToken=default)
        {
            ToastrResult<string> toastrResult = new ToastrResult<string>();
            string html = string.Empty;
            CreateOrEditArticleMediaViewModel model = new();
            
            if (id.HasValue)
            {
                var articleResult = await _contentManagerService.GetArticleByIdAsync(id.Value, cancellationToken);
                if (articleResult.IsSuccess)
                {
                    model.Id = articleResult.Data.Id;
                    model.ArticleTitle= articleResult.Data.Title;
                    if (isVideo)
                    {
                        model.FileName = articleResult.Data.VideoFileName;
                        model.IsVideo = true;
                    }
                    else
                    {
                        model.FileName = articleResult.Data.ImageFileName;
                        model.IsVideo = false;
                    }
                    html = await _renderService.RenderViewToStringAsync("_ChangeImageOrVideo", model, this.ControllerContext);
                    toastrResult = ToastrResult<string>.Success(Captions.Success, articleResult.Message, html);
                }
                else
                {
                    toastrResult = ToastrResult<string>.Error(Captions.Error, string.Format(UserMessages.NotFound, Captions.Article), html);
                }
            }
            else
                toastrResult = ToastrResult<string>.Error(Captions.Error, string.Format(UserMessages.NotFound, Captions.Article), html);
            return Json(toastrResult);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeArticleImageOrVideo(CreateOrEditArticleMediaViewModel model, CancellationToken cancellationToken)
        {
            ToastrResult<CreateOrEditArticleMediaViewModel> toastrResult = new ToastrResult<CreateOrEditArticleMediaViewModel>();

            if (ModelState.IsValid)
            {
                CommandResult<CreateOrEditArticleMediaViewModel> commandResult = await _contentManagerService.ChangeImageOrVideoAsync(model, cancellationToken);
                if (commandResult.IsSuccess)
                    toastrResult = ToastrResult<CreateOrEditArticleMediaViewModel>.Success(Captions.Success, commandResult.Message, commandResult.Data);
                else
                    toastrResult = ToastrResult<CreateOrEditArticleMediaViewModel>.Error(Captions.Error, commandResult.Message, commandResult.Data);
            }
            else
            {
                toastrResult = ModelState.GetAllErrorMessagesAsToast<CreateOrEditArticleMediaViewModel>(ToastrType.Error, false, UserMessages.FormNotValid,new());
            }
            return Json(toastrResult);
        }
        
        [HttpPost]
        public async Task<IActionResult> RemoveArticleImageOrVideo(int id,bool isVideo, CancellationToken cancellationToken)
        {
            ToastrResult toastrResult = new ToastrResult();
            if (ModelState.IsValid)
            {
               var commandResult = await _contentManagerService.RemoveImageOrVideoAsync(id,isVideo, cancellationToken);
                if (commandResult.IsSuccess)
                    toastrResult = ToastrResult.Success(Captions.Success, commandResult.Message);
                else
                    toastrResult = ToastrResult.Error(Captions.Error, commandResult.Message);
            }
            else
            {
                toastrResult = ModelState.GetAllErrorMessagesAsToast<CreateOrEditArticleMediaViewModel>(ToastrType.Error, false, UserMessages.FormNotValid,new());
            }
            return Json(toastrResult);
        }
      

        #region export pdf and excle
        public JsonResult InitReportData()
        {
            return Json(true);
        }
        public IActionResult ExportPdf()
        {
            List<ArticleModel> reportData = new();
            CommandResult<IQueryable<ArticleModel>> result = _contentManagerService.GetArticleAsQuerable();
            if (result.IsSuccess)
                reportData = result.Data.OrderBy(x => x.OrderNo).ToList();

            return new ViewAsPdf("ExportPdf", reportData, null)
            {
                PageSize = Size.A4,
                PageMargins = new Margins(8, 0, 4, 0),
                PageOrientation = Orientation.Landscape,
                FileName = $"ArticleReport-{DateTime.Now.ToString("yyyy_MM_dd-HH,mm")}.pdf",
            };
        }
        public IActionResult ExportExcel()
        {
            List<ArticleModel> reportData = new();
            CommandResult<IQueryable<ArticleModel>> result = _contentManagerService.GetArticleAsQuerable();
            if (result.IsSuccess)
                reportData = result.Data.OrderBy(x => x.OrderNo).ToList();

            DataTable reportList = new DataTable("ExportExcel");
            reportList.Columns.Add("#");
            reportList.Columns.Add(Captions.Title);
            reportList.Columns.Add(Captions.Status);
            reportList.Columns.Add(Captions.RegisterDate);

            double[] ColumnWidth = new double[reportList.Columns.Count];

            ColumnWidth[0] = 8;
            ColumnWidth[1] = 25;
            ColumnWidth[2] = 25;
            ColumnWidth[3] = 25;
            int i = 1;
            foreach (var item in reportData)
            {
                reportList.Rows.Add
                    (i++,
                   item.Title,
                   item.StatusTitle,
                   item.RegisterDatePersian
                   );
            }
            byte[] file = _fileService.CreateExcelFileFromDataTable(reportList, ColumnWidth, $"{Captions.List} {Captions.FAQ}", true);
            string filename = string.Format("{0}.xlsx", $"ArticleReport" + DateTime.Now.ToString("yyyy_MM_dd-HH,mm"));
            Response.Headers.Add("Content-Disposition", string.Format("attachment; filename={0}", filename));
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }
        #endregion
    }
}
