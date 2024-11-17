using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.ArticleModels;
using Gold.ApplicationService.Contract.DTOs.Models.ProductModels;
using Gold.ApplicationService.Contract.Interfaces;
using Gold.EndPoints.Presentation.InternalService;
using Gold.EndPoints.Presentation.Models;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.FileAddress;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading;

namespace Gold.EndPoints.Presentation.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IViewRenderService _renderService;
        private readonly ISettingService _settingService;
        private readonly ICustomerService _customerService;
        private readonly IContentManagerService _contentManagerService;
        private readonly FilePathAddress _filePathAddress;
        private readonly IFileService _fileService;
        public ArticleController(IViewRenderService renderService, ISettingService settingService,
             IOptions<FilePathAddress> filePathAddressOptions, IFileService fileService, ICustomerService customerService, IContentManagerService contentManagerService)
        {
            _renderService = renderService;
            _settingService = settingService;
            _filePathAddress = filePathAddressOptions.Value;
            _fileService = fileService;
            _customerService = customerService;
            _contentManagerService = contentManagerService;
        }


        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            CommandResult<List<ArticleForShowInSiteModel>> listResult = await _contentManagerService.GetArticlesForShoInSiteAsync(1, 10, string.Empty, cancellationToken);

            return View(listResult.Data);
        }



        public async Task<IActionResult> All(string? s,int page=1,int? p=null,CancellationToken cancellationToken=default)
        {
            if (page < 1)
                page = 1;
            s ??= string.Empty;
            CommandResult<List< ArticleForShowInSiteModel >> listResult = await _contentManagerService.GetArticlesForShoInSiteAsync(page,10,s, cancellationToken);
			return Json(listResult.Data);
        }

        [HttpGet("/Article/Detail/{postId}")]
        public async Task<IActionResult> Detail(int postId,CancellationToken cancellationToken)
        {
            ToastrResult<ArticleForShowInSiteModel> toastrResult = new();
            var result = await _contentManagerService.GetArticleInfoForShoInSiteAsync(postId, cancellationToken);
            if (result.IsSuccess)
                toastrResult = ToastrResult<ArticleForShowInSiteModel>.Success(Captions.Success, result.Message, result.Data);
            else
                toastrResult = ToastrResult<ArticleForShowInSiteModel>.Error(Captions.Error, result.Message, result.Data);
            return Json(toastrResult);
        }

        [HttpGet]
        [Route("/Article/ThumbnailImage/{fileName}")]
        public async Task<IActionResult> GetThumbnailImage(string? fileName)
        {
            string path = string.Empty;
            fileName ??= string.Empty;

            if (_fileService.IsExist(fileName, _filePathAddress.ArticleThumbnailImages))
                path = $"{_filePathAddress.ArticleThumbnailImages}/{fileName}";
            else
                path = _filePathAddress.ArticleDefaultImage;
                

            var fileBytes = await _fileService.GetFileBytesAsync(path);
            var fileContentType = _fileService.GetMimeTypeForFileExtension(path);
            return File(fileBytes, fileContentType);
        } 
        [HttpGet]
        [Route("/Article/Image/{fileName}")]
        public async Task<IActionResult> GetImage(string? fileName)
        {
            fileName ??= string.Empty;
            string path = string.Empty;

            if (_fileService.IsExist(fileName, _filePathAddress.ArticleImages))
                path = $"{_filePathAddress.ArticleImages}/{fileName}";
            else
                path = _filePathAddress.ProductDefaultImage;

            var fileBytes = await _fileService.GetFileBytesAsync(path);
            var fileContentType = _fileService.GetMimeTypeForFileExtension(path);
            return File(fileBytes, fileContentType);
        }
        [HttpGet]
        [Route("/Article/Video/{fileName}")]
        public async Task<IActionResult> GetVideo(string? fileName)
        {
            fileName ??= string.Empty;
            string path = string.Empty;

            if (_fileService.IsExist(fileName, _filePathAddress.ArticleVideos))
                path = $"{_filePathAddress.ArticleVideos}/{fileName}";
            else
                path = _filePathAddress.ProductDefaultImage;

            var fileBytes = await _fileService.GetFileBytesAsync(path);
            var fileContentType = _fileService.GetMimeTypeForFileExtension(path);
            return File(fileBytes, fileContentType);
        }
    }
}
