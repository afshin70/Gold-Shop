using Gold.ApplicationService.Concrete;
using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.AuthenticationModels;
using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels;
using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.CustomerMessageModels;
using Gold.ApplicationService.Contract.DTOs.Models.CustomerModels.DocumentModels;
using Gold.ApplicationService.Contract.DTOs.Models.UserModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.AuthenticationViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.CustomerViewModels;
using Gold.ApplicationService.Contract.Interfaces;
using Gold.Domain.Entities.AuthEntities;
using Gold.EndPoints.Presentation.InternalService;
using Gold.EndPoints.Presentation.Models;
using Gold.Infrastracture.Configurations.ValidationAttribute.Authorization;
using Gold.Infrastracture.Repositories;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.FileAddress;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.DTO.PaginationModels;
using Gold.SharedKernel.Enums;
using Gold.SharedKernel.ExtentionMethods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using System.Threading;

namespace Gold.EndPoints.Presentation.Controllers
{
	[UserAccess($"{nameof(UserType.Customer)}")]
	public class UserController : Controller
	{
		private readonly ICustomerService _customerService;
		private readonly IAuthenticationManager _authenticationManager;
		private readonly IViewRenderService _renderService;
		private readonly FilePathAddress _filePathAddress;
		private readonly IFileService _fileService;

		public UserController(ICustomerService customerService, IAuthenticationManager authenticationManager, IOptions<FilePathAddress> filePathAddressOptions, IViewRenderService renderService, IFileService fileService)
		{
			_customerService = customerService;
			_authenticationManager = authenticationManager;
			_renderService = renderService;
			_filePathAddress = filePathAddressOptions.Value;
			_fileService = fileService;
		}
		[HttpGet]
		[Route("/Profile")]
		public async Task<IActionResult> Index(CancellationToken cancellationToken)
		{
			var userName = User.GetUserName();
			CommandResult<CustomerProfileModel> customerInfoResult = await _customerService.GetCustomerProfileInfoAsync(userName, cancellationToken);
			return View(customerInfoResult.Data);
		}

		[HttpPost]
		[Route("/Profile")]
		public async Task<IActionResult> Index(CustomerProfileViewModel model, CancellationToken cancellationToken)
		{
			ToastrResult toastrResult = new ToastrResult();
			if (!ModelState.IsValid)
			{
				toastrResult = ModelState.GetAllErrorMessagesAsToast(ToastrType.Warning, false, string.Empty);
			}
			else
			{
				var userName = User.GetUserName();
				var customerProfileResult = await _customerService.EditProfileAsync(model, userName, cancellationToken);
				if (customerProfileResult.IsSuccess)
					toastrResult = ToastrResult.Success(Captions.Success, customerProfileResult.Message);
				else
					toastrResult = ToastrResult.Warning(Captions.Warning, customerProfileResult.Message);
			}
			return Json(toastrResult);
		}

		[HttpGet]
		[Route("/Profile/{imageName}")]
		public async Task<IActionResult> GetProfileImage(string? imageName)
		{
			try
			{
				string patch = $"{_filePathAddress.SellerProfileImage}/{imageName}";
				var fileBytes = await _fileService.GetFileBytesAsync(patch);
				var fileContentType = _fileService.GetMimeTypeForFileExtension(patch);
				return File(fileBytes, fileContentType);
			}
			catch (Exception)
			{
				return NotFound();
			}

		}

		[HttpGet]
		[Route("/Profile/Image")]
		public async Task<IActionResult> GetProfileImage(CancellationToken cancellationToken)
		{
			string patch = string.Empty;
			try
			{
				string imageName = string.Empty;
				if (User.Identity.IsAuthenticated)
				{
					int userId = User.GetUserIdAsInt();
					var result = await _customerService.GetProfileImageByUserIdAsync(userId, cancellationToken);
					if (result.IsSuccess)
						imageName = result.Data.ImageName;
				}
				if (!string.IsNullOrEmpty(imageName))
					patch = $"{_filePathAddress.CustomerProfileImage}/{imageName}";
				else
					patch = $"wwwroot/sitetheme/image/user.jpg";
				var fileBytes = await _fileService.GetFileBytesAsync(patch);
				var fileContentType = _fileService.GetMimeTypeForFileExtension(patch);
				return File(fileBytes, fileContentType);
			}
			catch (Exception)
			{
				patch = $"wwwroot/sitetheme/image/user.jpg";
				var fileBytes = await _fileService.GetFileBytesAsync(patch);
				var fileContentType = _fileService.GetMimeTypeForFileExtension(patch);
				return File(fileBytes, fileContentType);
			}
		}


		[HttpGet]
		[Route("/Messages")]
		public async Task<IActionResult> Messages(int page = 1, CancellationToken cancellationToken = default)
		{
			var userName = User.GetUserName();
			CommandResult<Pagination<CustomerMessageModel>> messagesResult = await _customerService.GetMessagesAsync(userName, page, cancellationToken);
			return View(messagesResult.Data);
		}

		#region علاقه مندی ها
		[HttpGet("/FavoriteProducts")]		
		public async Task<IActionResult> FavoriteProducts(CancellationToken cancellationToken)
		{
            int userId = User.GetUserIdAsInt();
            var result = await _customerService.GetAllProductsInFavoritAsync( userId, cancellationToken);
			return View(result.Data);
        }

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> AddToFavorites(long productId, CancellationToken cancellationToken)
		{

            ToastrResult toastrResult = new();
			if (User.Identity.IsAuthenticated)
			{
                int userId = User.GetUserIdAsInt();
                var result = await _customerService.AddProductToFavoritsAsync(productId, userId, cancellationToken);
                if (result.IsSuccess)
                    toastrResult = ToastrResult.Success(Captions.Success, result.Message);
                else
                    toastrResult = ToastrResult.Error(Captions.Error, result.Message);
            }
			else
			{
                toastrResult = ToastrResult.Error(Captions.Error, UserMessages.PleaseLogin);
            }
            

            return Json(toastrResult);
        }

		[HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> UnFavorite(long productId,CancellationToken cancellationToken)
		{
            ToastrResult toastrResult = new();
            if (User.Identity.IsAuthenticated)
			{

                int userId = User.GetUserIdAsInt();
            var result = await _customerService.RemoveProductFromFavoritsAsync(productId,userId, cancellationToken);
			if (result.IsSuccess)
                toastrResult = ToastrResult.Success(Captions.Success, result.Message);
			else
                toastrResult = ToastrResult.Error(Captions.Error, result.Message);
			}
            else
            {
                toastrResult = ToastrResult.Error(Captions.Error, UserMessages.PleaseLogin);
            }

            return Json(toastrResult);
        }
        #endregion

        #region شماره کارت بانکی
        [HttpGet]
		public async Task<IActionResult> AddBankCardNumbers(CancellationToken cancellationToken)
		{
			ToastrResult<string> toastrResult = new();
			string html = string.Empty;
			int userId = User.GetUserIdAsInt();
			var cardNumberListResult = await _customerService.GetCustomerCardNumberListByUserIdAsync(userId, cancellationToken);
			CardNumberViewModel model = new()
			{
				CardNumbers = cardNumberListResult.Data
			};
			html = await _renderService.RenderViewToStringAsync("_AddBankCardNumbers", model, this.ControllerContext);
			toastrResult = ToastrResult<string>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, html);
			return Json(toastrResult);
		}

		[HttpPost]
		public async Task<JsonResult> AddBankCardNumbers(CardNumberViewModel model, CancellationToken cancellationToken)
		{
			ToastrResult<string> toastrResult = new ToastrResult<string>();
			string html = string.Empty;
			var userId = User.GetUserIdAsInt();
			if (!ModelState.IsValid)
			{
				//invalid model
				toastrResult = ModelState.GetAllErrorMessagesAsToast<string>(ToastrType.Error, false, UserMessages.FormNotValid, html);
			}
			else
			{
				CommandResult<int?> customerIdResult = await _customerService.GetCustomerIdByUserIdAsync(userId, cancellationToken);
				if (customerIdResult.IsSuccess & customerIdResult.Data.HasValue)
				{
					//add new
					model.CustomerId = customerIdResult.Data.Value;
					var addCardNumberResult = await _customerService.AddCardNumberAsync(model, cancellationToken);
					if (addCardNumberResult.IsSuccess)
					{
						model = new();
						model.CustomerId = addCardNumberResult.Data.CustomerId;
						model.CardNumberId = 0;
						var listResult = await _customerService.GetCustomerCardNumberListByUserIdAsync(userId, cancellationToken);
						if (listResult.IsSuccess)
							model.CardNumbers = listResult.Data;
						html = await _renderService.RenderViewToStringAsync("_AddBankCardNumbers", model, this.ControllerContext);
						toastrResult = ToastrResult<string>.Success(Captions.Success, addCardNumberResult.Message, html);
					}
					else
						toastrResult = ToastrResult<string>.Error(Captions.Error, addCardNumberResult.Message, html);
				}
				else
					toastrResult = ToastrResult<string>.Error(Captions.Error, customerIdResult.Message, html);

			}
			return Json(toastrResult);
		}
		#endregion

		#region ثبت مغایرت اطلاعات
		[HttpGet]
		[Route("/EditInformationRequest")]
		public async Task<IActionResult> EditInformationRequest(CancellationToken cancellationToken)
		{
			ToastrResult<string> toastrResult = new();
			string html = string.Empty;
			EditInformationRequestViewModel model = new();
			html = await _renderService.RenderViewToStringAsync("_EditInformationRequest", model, this.ControllerContext);
			toastrResult = ToastrResult<string>.Success(Captions.Success, OperationResultMessage.OperationIsSuccessfullyCompleted, html);

			return Json(toastrResult);
		}

		[HttpPost]
		[Route("/EditInformationRequest")]
		public async Task<IActionResult> EditInformationRequest(EditInformationRequestViewModel model, CancellationToken cancellationToken)
		{
			ToastrResult<string> toastrResult = new();
			string html = string.Empty;
			if (!ModelState.IsValid)
			{
				html = await _renderService.RenderViewToStringAsync("_EditInformationRequest", model, this.ControllerContext);
				toastrResult = ModelState.GetAllErrorMessagesAsToast<string>(ToastrType.Warning, false, string.Empty, html);
			}
			else
			{
				var userName = User.GetUserName();
				CommandResult<EditInformationRequestViewModel> result = await _customerService.EditInformationRequestAsync(model, userName, cancellationToken);
				if (result.IsSuccess)
					toastrResult = ToastrResult<string>.Success(Captions.Success, result.Message, html);
				else
				{
					ModelState.Clear();
					html = await _renderService.RenderViewToStringAsync("_EditInformationRequest", model, this.ControllerContext);
					toastrResult = ToastrResult<string>.Warning(Captions.Warning, result.Message, html);
				}
			}
			return Json(toastrResult);
		}
		#endregion

		#region تغییر رمز ورود مشتری

		[HttpGet]
		[Route("/ChangePassword")]
		public IActionResult ChangePassword(CancellationToken cancellationToken)
		{
			return View();
		}

		[HttpPost]
		[Route("/ChangePassword")]
		public async Task<IActionResult> ChangePassword(ChangeUserPasswordViewModel model, CancellationToken cancellationToken)
		{
			ToastrResult toastrResult = new ToastrResult();
			if (!ModelState.IsValid)
			{
				toastrResult = ModelState.GetAllErrorMessagesAsToast(ToastrType.Warning, false, string.Empty);
			}
			else
			{
				var userName = User.GetUserName();
				CommandResult result = await _authenticationManager.ChangeCustomerPasswordAsync(model, userName, cancellationToken);
				if (result.IsSuccess)
				{
					var loginModel = new LoginViewModel
					{
						Password = model.NewPassword,
						RememberMe = true,
						UserName = userName,
					};
					await _authenticationManager.LoginUserAsync(HttpContext, loginModel, cancellationToken);
					toastrResult = ToastrResult.Success(Captions.Success, result.Message);
				}
				else
					toastrResult = ToastrResult.Warning(Captions.Warning, result.Message);
			}
			return Json(toastrResult);
		}
		#endregion

	}
}
