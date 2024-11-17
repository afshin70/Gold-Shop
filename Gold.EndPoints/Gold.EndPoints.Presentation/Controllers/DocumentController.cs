using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.CustomerPymentsModel;
using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.DocumentModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels.CustomerPaymentsViewModel;
using Gold.ApplicationService.Contract.Interfaces;
using Gold.EndPoints.Presentation.InternalService;
using Gold.EndPoints.Presentation.Models;
using Gold.Infrastracture.Configurations.ValidationAttribute.Authorization;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.FileAddress;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.Enums;
using Gold.SharedKernel.ExtentionMethods;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading;

namespace Gold.EndPoints.Presentation.Controllers
{
    [UserAccess($"{nameof(UserType.Customer)}")]
    public class DocumentController : Controller
    {
        private readonly IViewRenderService _renderService;
        private readonly ICustomerService _customerService;
        private readonly FilePathAddress _filePathAddress;
        private readonly IFileService _fileService;
        public DocumentController(ICustomerService customerService,
            IViewRenderService renderService,
            IOptions<FilePathAddress> filePathAddressOptions,
            IFileService fileService)
        {
            _customerService = customerService;
            _renderService = renderService;
            _filePathAddress = filePathAddressOptions.Value;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var userName = User.GetUserName();
            CommandResult<List<DocumentInfoModel>> listResult = await _customerService.GetDocumentsInfoAsync(userName, cancellationToken);
            if (listResult.Data.Any())
                return View(listResult.Data);
            return Redirect("/Profile");
        }


        //[HttpGet]
        //[Route("/Document/Detail/{documentId}")]
        //public async Task<IActionResult> Detail(long documentId, CancellationToken cancellationToken)
        //{
        //    var userName = User.GetUserName();
        //    CommandResult<CustomerDocumentDetailModel> documentResult = await _customerService.GetCustomerDocumentDetailAsync(userName, documentId, cancellationToken);
        //    if (!documentResult.IsSuccess)
        //        return Redirect("/Document");
        //    return View(documentResult.Data);
        //}

        [HttpGet]
        [Route("/Document/DocDetail/{documentId}")]
        public async Task<IActionResult> DocDetail(long documentId, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new();
            string html = string.Empty;
            var userName = User.GetUserName();
            CommandResult<CustomerDocumentDetailModel> documentResult = await _customerService.GetCustomerDocumentDetailAsync(userName, documentId, cancellationToken);
            // if (!documentResult.IsSuccess)
            //    return Redirect("/Document");
            // return View(documentResult.Data);
            if (documentResult.IsSuccess)
            {
                html = await _renderService.RenderViewToStringAsync("_DocDetail", documentResult.Data, this.ControllerContext);
                toastrResult = ToastrResult<string>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, html);
            }
            else
            {
                toastrResult = ToastrResult<string>.Error(Captions.Error, string.Format(OperationResultMessage.NotFound, Captions.Document), html);
            }
            return Json(toastrResult);
        }

        [HttpGet]
        [Route("/Document/GetCollateralImage/{documentId}")]
        public async Task<IActionResult> GetCollateralImage(long documentId, CancellationToken cancellationToken)
        {
            ToastrResult<string> toastrResult = new();
            string html = string.Empty;
            CommandResult<string> commandResult = await _customerService.GetCollateralImageAsync(documentId, User.GetUserName(), cancellationToken);

            html = await _renderService.RenderViewToStringAsync("_CollateralImage", commandResult.Data, this.ControllerContext);
            toastrResult = ToastrResult<string>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, html);

            return Json(toastrResult);
        }


        //[HttpGet]
        //[Route("/Document/InstallmentPayment/{documentId}")]
        //public async Task<IActionResult> InstallmentPayment(long documentId, CancellationToken cancellationToken)
        //{
        //    ToastrResult<string> toastrResult = new();
        //    string html = string.Empty;
        //    CustomerPaymentViewModel model = new();
        //    CommandResult<DocumentInfoForCustomerPayment> documentInfoResult = await _customerService.GetDocumentInfoForCustomerPaymentAsync(documentId, cancellationToken);
        //    if (documentInfoResult.IsSuccess)
        //    {
        //        ViewBag.DocumentInfoForCustomerPayment = documentInfoResult.Data;
        //        model.DocumentId = documentInfoResult.Data.DocumentId;
        //        html = await _renderService.RenderViewToStringAsync("_Payment", model, this.ControllerContext);
        //        toastrResult = ToastrResult<string>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, html);
        //    }
        //    else
        //        toastrResult = ToastrResult<string>.Error(Captions.Error, documentInfoResult.Message, html);
        //    return Json(toastrResult);

        //}


        [HttpGet]
        [Route("/Document/Payment/{documentId}")]
        public async Task<IActionResult> Payment(long documentId, CancellationToken cancellationToken)
        {
            CommandResult<DocumentInfoForCustomerPayment> documentInfoResult = await _customerService.GetDocumentInfoForCustomerPaymentAsync(documentId, cancellationToken);
            if (documentInfoResult.IsSuccess)
            {
                ViewBag.DocumentInfoForCustomerPayment = documentInfoResult.Data;
                CustomerPaymentViewModel model = new()
                {
                    DocumentId = documentInfoResult.Data.DocumentId
                };
                return View(model);
            }
            else
            {
                return Redirect("/Document");
            }

        }
        [HttpPost]
        public async Task<IActionResult> Payment(CustomerPaymentViewModel model, CancellationToken cancellationToken)
        {
            ToastrResult toastrResult = new();
            if (model.PaymentReceiptImage is null & (model.PayAmount is null | model.PayDate is null | model.PayTime is null))
                ModelState.AddModelError(nameof(model.PaymentReceiptImage), UserMessages.CustomerPaymentValidationMessage);
            if (!ModelState.IsValid)
            {
                toastrResult = ModelState.GetAllErrorMessagesAsToast(ToastrType.Warning, false, string.Empty);
            }
            else
            {
                CommandResult savePaymentResult = await _customerService.CreateCustomerPaymentAsync(model, cancellationToken);
                if (savePaymentResult.IsSuccess)
                    toastrResult = ToastrResult.Success(string.Empty, savePaymentResult.Message);
                else
                    toastrResult = ToastrResult.Warning(string.Empty, savePaymentResult.Message);
            }
            return Json(toastrResult);
        }



        [HttpGet]
        public async Task<IActionResult> AllPayments(long documentId, CancellationToken cancellationToken)
        {
            CommandResult<List<CustomerPaymentModel>> customerPaymentsResult = await _customerService.GetCustomerPaymentsAsync(documentId, cancellationToken);
            ToastrResult<string> toastrResult = new();
            string html = string.Empty;
            if (customerPaymentsResult.IsSuccess)
            {
                html = await _renderService.RenderViewToStringAsync("_AllPayments", customerPaymentsResult.Data, this.ControllerContext, true);
                toastrResult = ToastrResult<string>.Success(Captions.Success, customerPaymentsResult.Message, html);
            }
            else
            {
                toastrResult = ToastrResult<string>.Warning(Captions.Warning, customerPaymentsResult.Message, html);
            }
            return Json(toastrResult);
        }

        [HttpGet]
        [UserAccess($"{nameof(UserType.Customer)}")]
        public async Task<IActionResult> GetCustomerPaymentImage(string? imageName)
        {
            //try
            //{
            //    string paymentImagesPath = $"{_filePathAddress.CustomerPaymentImages}/{imageName}";
            //    var fileBytes = await _fileService.GetFileBytesAsync(paymentImagesPath);
            //    var fileContentType = _fileService.GetMimeTypeForFileExtension(paymentImagesPath);
            //    return File(fileBytes, fileContentType);
            //}
            //catch (Exception)
            //{
            //    return NotFound();
            //}
            string patch = string.Empty;
            try
            {
                 patch = $"{_filePathAddress.CustomerPaymentImages}/{imageName}";
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
        [Route("/Document/InstantSettlement/{documentId}")]
        public async Task<IActionResult> InstantSettlement(long documentId, CancellationToken cancellationToken)
        {
            CommandResult<InstantSettlementModel> instantSettlementResult = await _customerService.GetInstantSettlementInfoAsync(documentId, cancellationToken);
            ToastrResult<string> toastrResult = new();
            string html = string.Empty;
            if (instantSettlementResult.IsSuccess)
            {
                html = await _renderService.RenderViewToStringAsync("_InstantSettlement", instantSettlementResult.Data, this.ControllerContext, true);
                toastrResult = ToastrResult<string>.Success(Captions.Success, instantSettlementResult.Message, html);
            }
            else
            {
                toastrResult = ToastrResult<string>.Warning(Captions.Warning, instantSettlementResult.Message, html);
            }
            return Json(toastrResult);
        }


        [HttpGet]
        [UserAccess($"{nameof(UserType.Customer)}")]
        public async Task<IActionResult> GetCollateralImage(string? imageName)
        {
            try
            {
                string paymentImagesPath = $"{_filePathAddress.CollateralsDocs}/{imageName}";
                var fileBytes = await _fileService.GetFileBytesAsync(paymentImagesPath);
                var fileContentType = _fileService.GetMimeTypeForFileExtension(paymentImagesPath);
                return File(fileBytes, fileContentType);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

    }
}
