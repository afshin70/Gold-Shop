using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.ProductModels;
using Gold.ApplicationService.Contract.DTOs.Models.ProductModels.CategoryModels;
using Gold.ApplicationService.Contract.DTOs.Models.ProductModels.ProductGallertModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.ProductViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.ProductViewModels.CategoryViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.ProductViewModels.ProdctGalleryViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.SettingsViewModels;
using Gold.ApplicationService.Contract.Interfaces;
using Gold.Domain.Entities;
using Gold.Domain.Entities.AuthEntities;
using Gold.Domain.Enums;
using Gold.Infrastracture.Repositories.UOW;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.FileAddress;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.Enums;
using Gold.SharedKernel.ExtentionMethods;
using Gold.SharedKernel.Tools;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Immutable;

namespace Gold.ApplicationService.Concrete
{
    public class ProductManagementService : IProductManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogManager _logManager;
        private readonly IFileService _fileService;
        private readonly ISettingService _settingService;
        private readonly ICustomerService _customerService;
        private readonly IContentManagerService _contentManagerService;
        private readonly IGalleryService _galleryService;
        private readonly ISellerService _sellerService;
        private readonly FilePathAddress _filePathAddress;

        public ProductManagementService(IUnitOfWork unitOfWork, ILogManager logManager, IFileService fileService, IOptions<FilePathAddress> filePathAddressOptions, ISettingService settingService, IContentManagerService contentManagerService, ICustomerService customerService, ISellerService sellerService, IGalleryService galleryService)
        {
            _unitOfWork = unitOfWork;
            _logManager = logManager;
            _fileService = fileService;
            _filePathAddress = filePathAddressOptions.Value;
            _settingService = settingService;
            _contentManagerService = contentManagerService;
            _customerService = customerService;
            _sellerService = sellerService;
            _galleryService = galleryService;
        }

        #region Category
        public async Task<CommandResult<CreateOrEditCategoryViewModel>> CreateOrEditCategoryAsync(CreateOrEditCategoryViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Title))
                    return CommandResult<CreateOrEditCategoryViewModel>.Failure(string.Format(ValidationMessages.Required, Captions.Title), model);

                if (model.Id == null)
                {
                    //add 
                    bool isDuoplicate = await _unitOfWork.CategoryRepository.GetAllAsIQueryable().Data
                           .AnyAsync(x => x.Title == model.Title, cancellationToken);
                    if (isDuoplicate)
                        return CommandResult<CreateOrEditCategoryViewModel>.Failure(string.Format(UserMessages.IsDuplicated, Captions.Title), model);

                    var lastOrderNumber = await _unitOfWork.CategoryRepository.GetAllAsIQueryable().Data
                        .OrderByDescending(x => x.OrderNo)
                        .Select(x => x.OrderNo)
                        .FirstOrDefaultAsync(cancellationToken);
                    var item = new Category
                    {
                        Title = model.Title,
                        OrderNo = lastOrderNumber > 0 ? lastOrderNumber + 1 : 1
                    };
                    var addResult = await _unitOfWork.CategoryRepository.InsertAsync(item, cancellationToken);
                    if (addResult.IsSuccess)
                    {
                        var saveResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                        if (saveResult.IsSuccess)
                            return CommandResult<CreateOrEditCategoryViewModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                        else
                            return CommandResult<CreateOrEditCategoryViewModel>.Failure(saveResult.Message, model);
                    }
                    else
                        return CommandResult<CreateOrEditCategoryViewModel>.Failure(addResult.Message, model);
                }
                else
                {
                    //edit
                    var result = await _unitOfWork.CategoryRepository.GetByIdAsync(model.Id.Value, cancellationToken);
                    if (result.IsSuccess)
                    {
                        bool isDuoplicate = await _unitOfWork.CategoryRepository.GetAllAsIQueryable().Data
                           .AnyAsync(x => x.Id != result.Data.Id & x.Title == model.Title, cancellationToken);
                        if (isDuoplicate)
                            return CommandResult<CreateOrEditCategoryViewModel>.Failure(string.Format(UserMessages.IsDuplicated, Captions.Title), model);

                        result.Data.Title = model.Title;
                        var updateResult = _unitOfWork.CategoryRepository.Update(result.Data);
                        if (updateResult.IsSuccess)
                        {
                            var saveResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                            if (saveResult.IsSuccess)
                                return CommandResult<CreateOrEditCategoryViewModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                            else
                                return CommandResult<CreateOrEditCategoryViewModel>.Failure(saveResult.Message, model);
                        }
                        else
                            return CommandResult<CreateOrEditCategoryViewModel>.Failure(updateResult.Message, model);
                    }
                    else
                        return CommandResult<CreateOrEditCategoryViewModel>.Failure(result.Message, model);
                }

            }
            catch (Exception ex)
            {
                _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CreateOrEditCategoryViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
            }
        }

        public CommandResult<IQueryable<CategoryModel>> GetCategoryAsQuerable()
        {
            try
            {
                var result = _unitOfWork.CategoryRepository.GetAllAsIQueryable();
                if (result.IsSuccess)
                {
                    var iQuerableData = result.Data.Select(x => new CategoryModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        OrderNo = x.OrderNo,
                    });
                    return CommandResult<IQueryable<CategoryModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, iQuerableData);
                }
                else
                {
                    return CommandResult<IQueryable<CategoryModel>>.Failure(result.Message, null);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<CategoryModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult<CategoryModel>> GetCategoryByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var categoryResult = await _unitOfWork.CategoryRepository.GetByIdAsync(id, cancellationToken);

                if (categoryResult.IsSuccess)
                {
                    CategoryModel model = new()
                    {
                        Id = categoryResult.Data.Id,
                        OrderNo = categoryResult.Data.OrderNo,
                        Title = categoryResult.Data.Title,
                    };
                    return CommandResult<CategoryModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                }
                else
                    return CommandResult<CategoryModel>.Failure(categoryResult.Message, new());
            }
            catch (Exception ex)
            {
                _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CategoryModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }

        public async Task<CommandResult<string>> RemoveCategoryAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var categoryResult = await _unitOfWork.CategoryRepository.GetByIdAsync(id, cancellationToken);
                if (categoryResult.IsSuccess)
                {
                    _unitOfWork.CategoryRepository.Delete(categoryResult.Data);

                    var result = await _unitOfWork.CommitChangesAsync(cancellationToken);

                    if (result.IsSuccess)
                        return CommandResult<string>.Success(UserMessages.DataDeletedSuccessfully, categoryResult.Data.Title);/*string.Format(UserMessages.Gallery0IsDeleted, galleryResult.Data.Name)*/
                    else
                        return CommandResult<string>.Failure(result.Message, categoryResult.Data.Title);
                }
                else
                    return CommandResult<string>.Failure(categoryResult.Message, string.Empty);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, string.Empty);
            }
        }

        public async Task<CommandResult<List<SelectListItem>>> GetAllCategoriesAsync(int selectedId, CancellationToken cancellationToken)
        {
            try
            {

                var categories = await _unitOfWork.CategoryRepository.GetAllAsIQueryable().Data.ToListAsync(cancellationToken);
                if (categories is not null)
                {
                    var selectList = categories.OrderByDescending(x => x.OrderNo).Select(x => new SelectListItem
                    {
                        Selected = (x.Id == selectedId),
                        Text = x.Title,
                        Value = x.Id.ToString()
                    }).ToList();
                    return CommandResult<List<SelectListItem>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, selectList);
                }
                else
                {
                    return CommandResult<List<SelectListItem>>.Failure(string.Format(UserMessages.NotFound, Captions.Categories), new());
                }
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<SelectListItem>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }

        public async Task<CommandResult<List<CategoryModel>>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var categories = await _unitOfWork.CategoryRepository.GetAllAsIQueryable().Data
                    .Select(x => new CategoryModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        OrderNo = x.OrderNo
                    }).ToListAsync(cancellationToken);
                return CommandResult<List<CategoryModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, categories);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<CategoryModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }

        public async Task<CommandResult> ChangeCategoryOrderNumberAsync(int id, bool isUp, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.CategoryRepository.GetByIdAsync(id, cancellationToken);
                if (result.IsSuccess)
                {
                    if (isUp)
                    {
                        //up
                        var previousItem = _unitOfWork.CategoryRepository.GetAllAsIQueryable().Data
                            .Where(x => x.OrderNo < result.Data.OrderNo)
                            .OrderByDescending(x => x.OrderNo)
                            .FirstOrDefault();
                        if (previousItem is not null)
                        {
                            var tempItem = previousItem.OrderNo;
                            previousItem.OrderNo = result.Data.OrderNo;
                            result.Data.OrderNo = tempItem;
                            _unitOfWork.CategoryRepository.Update(previousItem);
                        }
                    }
                    else
                    {
                        //down
                        var nextItem = _unitOfWork.CategoryRepository.GetAllAsIQueryable().Data
                            .Where(x => x.OrderNo > result.Data.OrderNo)
                            .OrderBy(x => x.OrderNo)
                            .FirstOrDefault();
                        if (nextItem is not null)
                        {
                            var tempItem = nextItem.OrderNo;
                            nextItem.OrderNo = result.Data.OrderNo;
                            result.Data.OrderNo = tempItem;
                            _unitOfWork.CategoryRepository.Update(nextItem);
                        }
                    }
                    //update
                    _unitOfWork.CategoryRepository.Update(result.Data);
                    var updateResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (updateResult.IsSuccess)
                        return CommandResult.Success(UserMessages.DataEditedSuccessfully);
                    else
                        return CommandResult.Failure(updateResult.Message);
                }
                else
                {
                    return CommandResult.Failure(result.Message);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
            }
        }

        #endregion


        #region Product

        public async Task<CommandResult<CreateOrEditProductViewModel>> CreateOrEditProductAsync(CreateOrEditProductViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                var loanSettings = await _settingService.GetSettingAsync<LoanSettingsViewModel>(Domain.Enums.SettingType.LoanSetting, cancellationToken);
                if (loanSettings.IsSuccess)
                {
                    decimal maxGalleryProfit = decimal.Parse(loanSettings.Data.MaxProfitGallery);
                    if (decimal.TryParse(model.GalleryProfit, out decimal galleryProfit))
                    {
                        if (galleryProfit < 0 & galleryProfit > maxGalleryProfit)
                            return CommandResult<CreateOrEditProductViewModel>.Failure(string.Format(ValidationMessages.Range, "0", maxGalleryProfit.ToString()), model);
                    }
                }
                else
                    return CommandResult<CreateOrEditProductViewModel>.Failure(loanSettings.Message, model);



                var gallery = _galleryService.GetGalleryListAsQuerable()
                    .Data.FirstOrDefault(x => x.Id == model.GalleryId.Value);
                if (gallery is null)
                    return CommandResult<CreateOrEditProductViewModel>.Failure(string.Format(ValidationMessages.Invalid, Captions.Gallery), model);

                model.GalleryName = gallery.Name;


                if (model.ProductId == null)
                {

                    if (model.RegistrarUserId.HasValue & model.UserType == UserType.Seller)
                    {
                        var seller = _unitOfWork.SellerRepository.GetAllAsIQueryable().Data
                           .FirstOrDefault(x => x.UserId == model.RegistrarUserId.Value);
                        if (gallery.Id != seller.GalleryId)
                            return CommandResult<CreateOrEditProductViewModel>.Failure(UserMessages.YouAreNotAllowedToEditThisInformation, model);
                    }

                    //add 
                    var item = new Product();
                    item.Title = model.ProductTitle;
                    item.Description = model.Description;
                    item.GalleryId = gallery.Id;//model.GalleryId.Value;
                    item.GalleryProfit = decimal.Parse(model.GalleryProfit);
                    item.Karat = model.Karat.Value;
                    item.RegisterDate = DateTime.Now;
                    item.RegistrarUserId = model.RegistrarUserId.Value;
                    item.Status = model.Status.Value;
                    item.StonPrice = long.Parse(model.StonPrice.Replace(",", ""));
                    item.Wage = model.Wage.Value;
                    item.Weight = decimal.Parse(model.Weight);
                    #region Add Product Categories

                    if (model.CategoryIds is not null)
                    {
                        var categories = await _unitOfWork.CategoryRepository
                        .GetAllAsIQueryable().Data
                        .Where(x => model.CategoryIds.Contains(x.Id))
                        .ToListAsync(cancellationToken);

                        if (categories is null)
                        {
                            return CommandResult<CreateOrEditProductViewModel>.Failure(string.Format(ValidationMessages.Invalid, Captions.Category), model);
                        }
                        var productCategories = new List<ProductCategory>();
                        foreach (var category in categories)
                        {
                            productCategories.Add(new ProductCategory
                            {
                                CategoryId = category.Id,
                                ProductId = item.Id
                            });
                        }
                        item.ProductCategories = productCategories;
                    }

                    #endregion

                    #region محاسبه قیمت تقریبی طلا
                    //قیمت هر گرم طلا برحسب عیار محصول
                    int karatXPrice = 10000 * item.Karat;
                    var invoiceTotalPriceResult = _settingService.CalcInvoiceTotalPrice(karatXPrice, item.Weight, item.Wage, item.StonPrice, item.GalleryProfit, 9);
                    if (invoiceTotalPriceResult.IsSuccess)
                        item.RoughPrice = invoiceTotalPriceResult.Data;
                    else
                        return CommandResult<CreateOrEditProductViewModel>.Failure(invoiceTotalPriceResult.Message, model);
                    #endregion


                    var addResult = await _unitOfWork.AddAsync(item, cancellationToken);
                    if (addResult.IsSuccess)
                    {
                        var saveResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                        if (saveResult.IsSuccess)
                            return CommandResult<CreateOrEditProductViewModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                        else
                            return CommandResult<CreateOrEditProductViewModel>.Failure(saveResult.Message, model);
                    }
                    else
                        return CommandResult<CreateOrEditProductViewModel>.Failure(addResult.Message, model);
                }
                else
                {
                    //edit
                    var product = await _unitOfWork.ProductRepository.GetAllAsIQueryable().Data
                        .Where(x => x.Id == model.ProductId.Value)
                        .Include(x => x.Gallery)
                        .Include(x => x.ProductCategories)
                        .FirstOrDefaultAsync(cancellationToken);



                    if (product is not null)
                    {

                        if (model.RegistrarUserId.HasValue & model.UserType == UserType.Seller)
                        {
                            var seller = _unitOfWork.SellerRepository.GetAllAsIQueryable().Data
                               .FirstOrDefault(x => x.UserId == model.RegistrarUserId.Value);
                            if (gallery.Id != seller.GalleryId | product.GalleryId != seller.GalleryId)
                                return CommandResult<CreateOrEditProductViewModel>.Failure(UserMessages.YouAreNotAllowedToEditThisInformation, model);
                        }

                        #region update Product Fileds
                        product.Title = model.ProductTitle;
                        product.Description = model.Description;
                        product.GalleryId = gallery.Id;
                        product.GalleryProfit = decimal.Parse(model.GalleryProfit);
                        product.Karat = model.Karat.Value;
                        product.Status = model.Status.Value;
                        product.StonPrice = long.Parse(model.StonPrice.Replace(",", ""));
                        product.Wage = model.Wage.Value;
                        product.Weight = decimal.Parse(model.Weight);
                        #region Add Product Categories

                        //delete old product categories
                        if (product.ProductCategories.Any())
                            product.ProductCategories.Clear();

                        if (model.CategoryIds is not null)
                        {
                            var newCategories = await _unitOfWork.CategoryRepository
                           .GetAllAsIQueryable().Data
                           .Where(x => model.CategoryIds.Contains(x.Id))
                           .ToListAsync(cancellationToken);

                            if (newCategories is null)
                            {
                                return CommandResult<CreateOrEditProductViewModel>.Failure(string.Format(ValidationMessages.Required, Captions.Category), model);
                            }
                            var newProductCategories = new List<ProductCategory>();
                            foreach (var category in newCategories)
                            {
                                newProductCategories.Add(new ProductCategory
                                {
                                    CategoryId = category.Id,
                                    ProductId = product.Id
                                });
                            }


                            //add new poduct categories
                            if (newProductCategories.Any())
                                foreach (var item in newProductCategories)
                                    product.ProductCategories.Add(item);
                        }

                        #endregion

                        //#region محاسبه قیمت تقریبی طلا
                        ////قیمت هر گرم طلا برحسب عیار محصول
                        //int karatXPrice = 10000 * product.Karat;
                        //var invoiceTotalPriceResult = _settingService.CalcInvoiceTotalPrice(karatXPrice, product.Weight, product.Wage, product.StonPrice, product.GalleryProfit, 9);
                        //if (invoiceTotalPriceResult.IsSuccess)
                        //    product.RoughPrice = invoiceTotalPriceResult.Data;
                        //else
                        //    return CommandResult<CreateOrEditProductViewModel>.Failure(invoiceTotalPriceResult.Message, model);
                        //#endregion
                        #endregion



                        var updateResult = _unitOfWork.Update(product);
                        if (updateResult.IsSuccess)
                        {
                            var saveResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                            if (saveResult.IsSuccess)
                                return CommandResult<CreateOrEditProductViewModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                            else
                                return CommandResult<CreateOrEditProductViewModel>.Failure(saveResult.Message, model);
                        }
                        else
                            return CommandResult<CreateOrEditProductViewModel>.Failure(updateResult.Message, model);
                    }
                    else
                        return CommandResult<CreateOrEditProductViewModel>.Failure(string.Format(UserMessages.NotFound, Captions.Product), model);
                }

            }
            catch (Exception ex)
            {
                _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CreateOrEditProductViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
            }
        }


        public CommandResult<IQueryable<ProductModel>> GetProductAsQuerable(int? userId = null)
        {
            try
            {
                var result = _unitOfWork.ProductRepository.GetAllAsIQueryable();
                if (result.IsSuccess)
                {
                    var query = result.Data;
                    if (userId.HasValue)
                    {
                        var seller = _unitOfWork.SellerRepository.GetAllAsIQueryable().Data
                            .FirstOrDefault(x => x.UserId == userId.Value);

                        query = query.Where(x => x.GalleryId == seller.GalleryId);
                    }

                    var iQuerableData = query.OrderByDescending(x => x.RegisterDate).Select(x => new ProductModel
                    {
                        Id = x.Id,
                        GalleryName = x.Gallery.Name,
                        Title = x.Title,
                        Weight = x.Weight,
                        Wage = x.Wage,
                        Karat = x.Karat,
                        GalleryProfit = x.GalleryProfit,
                        Status = x.Status,
                        RegistrarUserName = x.RegistrarUser.FullName,
                        RegisterDatePersian = x.RegisterDate.GeorgianToPersian(ShowMode.OnlyDateAndTime),
                        StonPrice = x.StonPrice,
                        CategoryTitles = x.ProductCategories.Select(x => x.Category.Title).ToList(),

                    });
                    return CommandResult<IQueryable<ProductModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, iQuerableData);
                }
                else
                {
                    return CommandResult<IQueryable<ProductModel>>.Failure(result.Message, null);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<ProductModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult<ProductModel>> GetProductByIdAsync(long id, int? userId, CancellationToken cancellationToken)
        {
            try
            {
                CommandResult<Product> productResult = null;
                if (userId.HasValue)
                {
                    var seller = _unitOfWork.SellerRepository.GetAllAsIQueryable().Data
                       .FirstOrDefault(x => x.UserId == userId.Value);
                    productResult = await _unitOfWork.ProductRepository.GetByIdAsync(id, cancellationToken);
                    if (productResult.IsSuccess)
                    {
                        if (productResult.Data.GalleryId != seller.GalleryId)
                        {
                            return CommandResult<ProductModel>.Failure(UserMessages.YouAreNotAllowedToEditThisInformation, new());
                        }
                    }
                }
                else
                    productResult = await _unitOfWork.ProductRepository.GetByIdAsync(id, cancellationToken);

                if (productResult.IsSuccess)
                {


                    ProductModel model = new()
                    {
                        Id = productResult.Data.Id,
                        GalleryName = productResult.Data.Gallery.Name,
                        Title = productResult.Data.Title,
                        Weight = productResult.Data.Weight,
                        Wage = productResult.Data.Wage,
                        Karat = productResult.Data.Karat,
                        GalleryProfit = productResult.Data.GalleryProfit,
                        Status = productResult.Data.Status,
                        StonPrice = productResult.Data.StonPrice,
                        Description = productResult.Data.Description,
                        GalleryId = productResult.Data.GalleryId,
                        RegisterDate = productResult.Data.RegisterDate,
                        CategoryIds = productResult.Data.ProductCategories.Select(x => x.CategoryId).ToList(),

                    };
                    return CommandResult<ProductModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                }
                else
                    return CommandResult<ProductModel>.Failure(productResult.Message, new());
            }
            catch (Exception ex)
            {
                _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<ProductModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }

        public async Task<CommandResult<ProductModel>> RemoveProductAsync(long id, int? userId = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var product = await _unitOfWork.ProductRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == id)
                    .Include(x => x.ProductCategories)
                    .Include(x => x.ProductGalleries)
                    .FirstOrDefaultAsync(cancellationToken);

                ProductModel model = new ProductModel
                {
                    GalleryName = product.Gallery.Name,
                    Title = product.Title,
                };

                if (product is not null)
                {
                    if (userId.HasValue)
                    {

                        var sellerGalleryResult = await _sellerService.GetSellerGalleryAsync(userId.Value, cancellationToken);
                        if (sellerGalleryResult.IsSuccess)
                        {
                            if (sellerGalleryResult.Data.Id != product.GalleryId)
                                return CommandResult<ProductModel>.Failure(UserMessages.CantDeleteProduct, model);
                        }
                        else
                            return CommandResult<ProductModel>.Failure(string.Format(UserMessages.NotFound, Captions.Product), model);
                    }

                    #region delete product gallery files

                    if (product.ProductGalleries is not null)
                    {
                        //delete files
                        foreach (var item in product.ProductGalleries)
                            _fileService.DeleteFile(item.FileName, _filePathAddress.ProductGallery);
                    }

                    #endregion
                    _unitOfWork.Delete(product);

                    var result = await _unitOfWork.CommitChangesAsync(cancellationToken);

                    if (result.IsSuccess)
                        return CommandResult<ProductModel>.Success(UserMessages.DataDeletedSuccessfully, model);
                    else
                        return CommandResult<ProductModel>.Failure(result.Message, model);
                }
                else
                    return CommandResult<ProductModel>.Failure(string.Format(UserMessages.NotFound, Captions.Product), model);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<ProductModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }
        public async Task<CommandResult<string>> ChangeProductStatusAsync(long id, int? userId = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var product = await _unitOfWork.ProductRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (product is not null)
                {

                    if (userId.HasValue)
                    {
                        var sellerGalleryResult = await _sellerService.GetSellerGalleryAsync(userId.Value, cancellationToken);
                        if (sellerGalleryResult.IsSuccess)
                        {
                            if (sellerGalleryResult.Data.Id != product.GalleryId)
                                return CommandResult<string>.Failure(UserMessages.CantChangeProductStatus, string.Empty);
                        }
                        else
                        {
                            return CommandResult<string>.Failure(string.Format(UserMessages.NotFound, Captions.Product), string.Empty);
                        }
                    }

                    if (product.Status == ProductStatus.Active)
                        product.Status = ProductStatus.Sold;
                    else
                        product.Status = ProductStatus.Active;

                    _unitOfWork.Update(product);

                    var result = await _unitOfWork.CommitChangesAsync(cancellationToken);

                    if (result.IsSuccess)
                        return CommandResult<string>.Success(UserMessages.DataEditedSuccessfully, product.Title);
                    else
                        return CommandResult<string>.Failure(result.Message, product.Title);
                }
                else
                    return CommandResult<string>.Failure(string.Format(UserMessages.NotFound, Captions.Product), string.Empty);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, string.Empty);
            }
        }


        #endregion

        #region Product gallery
        public async Task<CommandResult<ProductGalleryModel>> GetProductGalleryByIdAsync(long id, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _unitOfWork.ProductGalleryRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Id == id)
                    .Include(x => x.Product)
                    .Select(x => new ProductGalleryModel
                    {
                        ProductGalleryId = x.Id,
                        OwnProductId = x.ProductId,
                        FileName = x.FileName,
                        FileType = x.FileType,
                        OrderNo = x.OrderNo,
                        ProductName = x.Product.Title,
                        IsThumbnail = x.IsThumbnail,
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (item is not null)
                    return CommandResult<ProductGalleryModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, item);
                else
                    return CommandResult<ProductGalleryModel>.Failure(string.Format(UserMessages.NotFound, Captions.Product), new());
            }
            catch (Exception ex)
            {
                _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<ProductGalleryModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }

        public async Task<CommandResult<List<ProductGalleryModel>>> GetAllProductGalleryByProductIdAsync(long productId, CancellationToken cancellationToken)
        {
            try
            {
                var list = await _unitOfWork.ProductGalleryRepository.GetAllAsIQueryable().Data
                    .Where(x => x.ProductId == productId)
                    .Select(x => new ProductGalleryModel
                    {
                        ProductGalleryId = x.Id,
                        OwnProductId = x.ProductId,
                        FileName = x.FileName,
                        FileType = x.FileType,
                        OrderNo = x.OrderNo,
                        ProductName = x.Product.Title,
                        IsThumbnail = x.IsThumbnail,
                    }).ToListAsync(cancellationToken);
                return CommandResult<List<ProductGalleryModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, list);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<ProductGalleryModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }

        public async Task<CommandResult<CreateProductGalleryViewModel>> CreateProductGalleryAsync(CreateProductGalleryViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                if (model.UploadedFile is null)
                {
                    return CommandResult<CreateProductGalleryViewModel>.Failure(string.Format(ValidationMessages.Required, Captions.VideoOrImageFile), model);
                }
                ProductGallery item = new() { ProductId = model.OwnProductId };

                string fileExtentin = Path.GetExtension(model.UploadedFile.FileName);
                List<string> allowedImageExtentins = new List<string>() { ".png", ".jpg", ".jpeg" };
                List<string> allowedVideoExtentins = new List<string>() { ".mp4", ".mpeg" };
                if (allowedImageExtentins.Any(x => x.ToLower() == fileExtentin.ToLower()))
                {
                    //is image
                    item.FileType = MediaFileType.Image;
                    //save gallery file
                    string fileName = Guid.NewGuid().ToString();
                    if (_fileService.SaveImageFile(model.UploadedFile, _filePathAddress.ProductGalleryThumbnailWith, _filePathAddress.ProductGalleryThumbnailHeight, _filePathAddress.ProductGalleryThumbnail, fileName) &
                        await _fileService.UploadFileAsync(model.UploadedFile, _filePathAddress.ProductGallery, fileName))
                        item.FileName = $"{fileName}{fileExtentin}";
                    else
                        return CommandResult<CreateProductGalleryViewModel>.Failure(UserMessages.ErrorInUploadFile, model);
                }
                else if (allowedVideoExtentins.Any(x => x.ToLower() == fileExtentin.ToLower()))
                {
                    item.FileType = MediaFileType.Video;
                    //save gallery file
                    string fileName = Guid.NewGuid().ToString();
                    if (await _fileService.UploadFileAsync(model.UploadedFile, _filePathAddress.ProductGallery, fileName))
                        item.FileName = $"{fileName}{fileExtentin}";
                    else
                        return CommandResult<CreateProductGalleryViewModel>.Failure(UserMessages.ErrorInUploadFile, model);
                }
                else
                {
                    return CommandResult<CreateProductGalleryViewModel>.Failure(string.Format(ValidationMessages.InvalidFormat, Captions.VideoOrImageFile), model);
                }


                var lastOrderNumber = await _unitOfWork.ProductGalleryRepository.GetAllAsIQueryable().Data
                        .Where(x => x.ProductId == model.OwnProductId)
                        .OrderByDescending(x => x.OrderNo)
                        .Select(x => x.OrderNo)
                        .FirstOrDefaultAsync(cancellationToken);

                if (lastOrderNumber > 0)
                {
                    item.OrderNo = lastOrderNumber + 1;
                    item.IsThumbnail = false;
                }
                else
                {
                    item.OrderNo = 1;
                    item.IsThumbnail = true;
                }
                // item.OrderNo= lastOrderNumber > 0 ? lastOrderNumber + 1 : 1;

                var addResult = await _unitOfWork.ProductGalleryRepository.InsertAsync(item, cancellationToken);
                if (addResult.IsSuccess)
                {
                    var saveResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (saveResult.IsSuccess)
                    {
                        string? productName = await _unitOfWork.ProductRepository.GetAllAsIQueryable().Data
                            .Where(x => x.Id == model.OwnProductId)
                            .Select(x => x.Title)
                            .FirstOrDefaultAsync(cancellationToken);
                        productName ??= string.Empty;
                        model.ProductName = productName;

                        return CommandResult<CreateProductGalleryViewModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                    }
                    else
                        return CommandResult<CreateProductGalleryViewModel>.Failure(saveResult.Message, model);
                }
                else
                    return CommandResult<CreateProductGalleryViewModel>.Failure(addResult.Message, model);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CreateProductGalleryViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
            }
        }
        public async Task<CommandResult<string>> RemoveProductGalleryAsync(long id, CancellationToken cancellationToken)
        {
            try
            {
                var itemResult = await _unitOfWork.ProductGalleryRepository.GetByIdAsync(id, cancellationToken);
                if (itemResult.IsSuccess)
                {

                    #region delete product gallery files
                    _fileService.DeleteFile(itemResult.Data.FileName, _filePathAddress.ProductGallery);
                    if (itemResult.Data.FileType == MediaFileType.Image)
                        _fileService.DeleteFile(itemResult.Data.FileName, _filePathAddress.ProductGalleryThumbnail);
                    #endregion


                    #region update thumbnail
                    if (itemResult.Data.IsThumbnail)
                    {
                        var lastItem = _unitOfWork.ProductGalleryRepository.GetAllAsIQueryable().Data
                             .Where(x => x.ProductId == itemResult.Data.ProductId & x.Id != itemResult.Data.Id)
                             .OrderByDescending(x => x.OrderNo)
                             .FirstOrDefault();
                        if (lastItem is not null)
                        {
                            lastItem.IsThumbnail = true;
                            _unitOfWork.ProductGalleryRepository.Update(lastItem);
                        }
                    }
                    #endregion
                    _unitOfWork.ProductGalleryRepository.Delete(itemResult.Data);
                    var result = await _unitOfWork.CommitChangesAsync(cancellationToken);

                    string? productName = await _unitOfWork.ProductRepository.GetAllAsIQueryable().Data
                        .Where(x => x.Id == itemResult.Data.ProductId)
                        .Select(x => x.Title)
                       .FirstOrDefaultAsync(cancellationToken);
                    productName ??= string.Empty;

                    if (result.IsSuccess)
                        return CommandResult<string>.Success(UserMessages.DataDeletedSuccessfully, productName);
                    else
                        return CommandResult<string>.Failure(result.Message, productName);
                }
                else
                    return CommandResult<string>.Failure(string.Format(UserMessages.NotFound, Captions.ProductGallery), string.Empty);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, string.Empty);
            }
        }

        public async Task<CommandResult> ChangeGalleryOrderNumberAsync(long id, bool isUp, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.ProductGalleryRepository.GetByIdAsync(id, cancellationToken);
                if (result.IsSuccess)
                {
                    if (isUp)
                    {
                        //up
                        var previousItem = _unitOfWork.ProductGalleryRepository.GetAllAsIQueryable().Data
                            .Where(x => x.ProductId == result.Data.ProductId & x.OrderNo < result.Data.OrderNo)
                            .OrderByDescending(x => x.OrderNo)
                            .FirstOrDefault();
                        if (previousItem is not null)
                        {
                            var tempItem = previousItem.OrderNo;
                            previousItem.OrderNo = result.Data.OrderNo;
                            result.Data.OrderNo = tempItem;
                            _unitOfWork.ProductGalleryRepository.Update(previousItem);
                        }
                    }
                    else
                    {
                        //down
                        var nextItem = _unitOfWork.ProductGalleryRepository.GetAllAsIQueryable().Data
                            .Where(x => x.ProductId == result.Data.ProductId & x.OrderNo > result.Data.OrderNo)
                            .OrderBy(x => x.OrderNo)
                            .FirstOrDefault();
                        if (nextItem is not null)
                        {
                            var tempItem = nextItem.OrderNo;
                            nextItem.OrderNo = result.Data.OrderNo;
                            result.Data.OrderNo = tempItem;
                            _unitOfWork.ProductGalleryRepository.Update(nextItem);
                        }
                    }
                    //update
                    _unitOfWork.ProductGalleryRepository.Update(result.Data);
                    var updateResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (updateResult.IsSuccess)
                        return CommandResult.Success(UserMessages.DataEditedSuccessfully);
                    else
                        return CommandResult.Failure(updateResult.Message);
                }
                else
                {
                    return CommandResult.Failure(result.Message);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
            }
        }
        public async Task<CommandResult> SetProductGalleryThubmnailAsync(long id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.ProductGalleryRepository.GetByIdAsync(id, cancellationToken);
                if (result.IsSuccess)
                {
                    if (result.Data.FileType != MediaFileType.Image)
                        return CommandResult.Failure(UserMessages.OnlyImagesCanSetForThumbnail);
                    var allGalleries = await _unitOfWork.ProductGalleryRepository.GetAllAsIQueryable().Data
                        .Where(x => x.ProductId == result.Data.ProductId)
                        .ToListAsync(cancellationToken);
                    if (allGalleries is null)
                        return CommandResult.Failure(string.Format(UserMessages.NotFound, Captions.Galleries));

                    foreach (var item in allGalleries)
                    {
                        if (item.Id == result.Data.Id)
                            item.IsThumbnail = true;
                        else
                            item.IsThumbnail = false;
                    }
                    //update
                    _unitOfWork.ProductGalleryRepository.UpdateRange(allGalleries);
                    var updateResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (updateResult.IsSuccess)
                        return CommandResult.Success(UserMessages.DataEditedSuccessfully);
                    else
                        return CommandResult.Failure(updateResult.Message);
                }
                else
                {
                    return CommandResult.Failure(result.Message);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
            }
        }

        public async Task<CommandResult<List<ProductForShowInSiteModel>>> GetProductsByFilterAsync(ProductsFilterModel model)
        {
            try
            {
                var query = _unitOfWork.GalleryRepository.GetAllAsIQueryable().Data
                     .Where(x => x.IsActive)
                    .Include(x => x.Products).ThenInclude(x => x.ProductGalleries)
                    .SelectMany(x => x.Products);
                // var query = _unitOfWork.ProductRepository.GetAllAsIQueryable().Data;


                if (!string.IsNullOrEmpty(model.Term))
                    query = query.Where(x => x.Title.Contains(model.Term));

                if (model.Categories is not null)
                    if (model.Categories.Any())
                        query = query.Include(x => x.ProductCategories)
                        .Where(x => x.ProductCategories.Any(x => model.Categories.Contains(x.CategoryId)));
                //query = query.OrderBy(x => x.Status);

                if (model.SortBy.HasValue)
                {
                    switch (model.SortBy.Value)
                    {
                        case SortBy.MostVisited:
                            query = query.OrderBy(x => x.Status).ThenByDescending(x => x.VisitedCount);
                            break;
                        case SortBy.LowestWage:
                            query = query.OrderBy(x => x.Status).ThenBy(x => x.Wage);
                            break;
                        case SortBy.HighestWage:
                            query = query.OrderBy(x => x.Status).ThenByDescending(x => x.Wage);
                            break;
                        case SortBy.MostExpensive:
                            query = query.OrderBy(x => x.Status).ThenByDescending(x => x.RoughPrice);
                            break;
                        case SortBy.Cheapest:
                            query = query.OrderBy(x => x.Status).ThenBy(x => x.RoughPrice);
                            break;
                        case SortBy.BiggestDiscount:
                            query = query.OrderBy(x => x.Status).ThenBy(x => x.GalleryProfit);
                            break;
                        case SortBy.Newest:
                            query = query.OrderBy(x => x.Status).ThenByDescending(x => x.RegisterDate);
                            break;
                        default:
                            query = query.OrderBy(x => x.Status).ThenByDescending(x => x.RegisterDate);
                            break;
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.Status);
                }

                var result = await query
                    //.Include(x => x.ProductGalleries)

                    .Skip(model.PageSize * (model.Page - 1))
                    .Take(model.PageSize)
                    .Select(x => new ProductForShowInSiteModel
                    {
                        Id = x.Id,
                        ImageName = x.ProductGalleries.Any() ? x.ProductGalleries.Where(x => x.IsThumbnail).FirstOrDefault().FileName : "product-logo.png",
                        IsImage = x.ProductGalleries.Any() ? x.ProductGalleries.Where(x => x.IsThumbnail).FirstOrDefault().FileType == MediaFileType.Image : false,
                        IsSold = x.Status == ProductStatus.Sold,
                        Title = x.Title,
                    })
                    .ToListAsync();
                return CommandResult<List<ProductForShowInSiteModel>>.Success(OperationResultMessage.AnErrorHasOccurredInTheSoftware, result);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<List<ProductForShowInSiteModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }

        public async Task<CommandResult<ProductInfoForShowInSiteModel>> GetProductInfoAsync(long productId, CancellationToken cancellationToken)
        {
            ProductInfoForShowInSiteModel model = new();
            try
            {
                var product = await _unitOfWork.ProductRepository.GetAllAsIQueryable().Data
                    .Include(x => x.Gallery).ThenInclude(x => x.Sellers).ThenInclude(x => x.User)
                    .Include(x => x.ProductGalleries)
                    .FirstOrDefaultAsync(x => x.Id == productId, cancellationToken);

                if (product is not null)
                {
                    var contentTypeResult = await _settingService.GetSettingAsync<string>(SettingType.SiteContent_InstallmentPurchaseOfProduct, cancellationToken);
                    if (contentTypeResult.IsSuccess)
                    {
                        model.InstallmentPurchaseOfProduct = string.IsNullOrEmpty(contentTypeResult.Data) ? string.Empty : contentTypeResult.Data;
                    }
                    model.HasInstallmentSale = product.Gallery.HasInstallmentSale;
                    model.Id = product.Id;
                    model.Title = product.Title;
                    model.Description = string.IsNullOrEmpty(product.Description) ? string.Empty : product.Description;
                    var goldPriceResult = await _settingService.GetGoldPriceInfoAsync(cancellationToken);

                    if (goldPriceResult.IsSuccess)
                    {
                        long karat1Amount = goldPriceResult.Data.Karat18 / 18;
                        decimal goldPrice = 0;
                        if (product.Karat == 18)
                        {
                            goldPrice = _settingService.CalcGoldPrice(goldPriceResult.Data.Karat18, product.Weight);
                        }
                        else
                        {
                            goldPrice = _settingService.CalcGoldPrice((karat1Amount * product.Karat), product.Weight);
                        }
                        model.GoldPrice = NumberTools.RoundUpNumber(Math.Ceiling(goldPrice).ToString(), 4);
                        model.StonePrice = product.StonPrice;
                        var wageAmount = _settingService.CalcWageAmount(goldPrice, product.Wage);
                        model.Wage = product.Wage;
                        model.WageAmount = NumberTools.RoundUpNumber(Math.Ceiling(wageAmount).ToString(), 4);
                        var galleryProfitAmount = _settingService.CalcGalleryProfitAmount(model.StonePrice, model.WageAmount, model.GoldPrice, product.GalleryProfit);
                        model.GalleryProfitAmount = NumberTools.RoundUpNumber(Math.Ceiling(galleryProfitAmount).ToString(), 4);
                        model.GalleryProfit = product.GalleryProfit;
                        var loanSettingsResult = await _settingService.GetSettingAsync<LoanSettingsViewModel>(SettingType.LoanSetting, cancellationToken);
                        if (loanSettingsResult.IsSuccess)
                        {
                            if (decimal.TryParse(loanSettingsResult.Data.MaxProfitGallery, out decimal galleryMaxProfit))
                            {
                                model.Discount = Math.Abs(galleryMaxProfit - product.GalleryProfit);
                                if (model.Discount > 0)
                                {
                                    model.DiscountedPrice = NumberTools.RoundUpNumber(Math.Ceiling(model.GoldPrice - (model.Discount * model.GoldPrice)).ToString(), 4);
                                    model.DiscountedPrice = Math.Abs(model.DiscountedPrice);
                                    if (model.DiscountedPrice == model.GoldPrice)
                                    {
                                        model.DiscountedPrice = 0;
                                    }
                                    model.HasDiscount = true;
                                }
                                else
                                {
                                    model.DiscountedPrice = 0;
                                    model.HasDiscount = false;
                                }
                            }

                            if (loanSettingsResult.Data.Tax.HasValue)
                            {
                                var taxAmount = _settingService.CalcTax(model.WageAmount, model.GalleryProfitAmount, loanSettingsResult.Data.Tax.Value);

                                model.Tax = loanSettingsResult.Data.Tax.Value;
                                model.TaxAmount = NumberTools.RoundUpNumber(Math.Ceiling(taxAmount).ToString(), 4);

                                var finalPrice = _settingService.InvoceTotalAmount(model.GoldPrice, model.WageAmount, model.GalleryProfitAmount, model.TaxAmount, model.StonePrice);
                                model.FinalPrice = NumberTools.RoundUpNumber(Math.Ceiling(finalPrice).ToString(), 4);
                            }

                        }


                    }
                    model.Weight = product.Weight.ToString()/*.Replace('.', '/')*/;

                    #region  تصاویر و ویدیو های محصول
                    if (product.ProductGalleries.Any())
                    {
                        model.GalleryFiles = product.ProductGalleries.OrderBy(x => x.OrderNo).Select(x => new ProductGalleryForShowInSiteModel
                        {
                            IsImage = x.FileType == MediaFileType.Image,
                            FileName = x.FileName,
                            ProductId = x.ProductId
                        }).ToList();
                    }
                    #endregion

                    #region اطلاعات گالری محصول
                    if (product.Gallery is not null)
                    {
                        model.GalleryTitle = product.Gallery.Name;
                        model.GalleryPhone = string.IsNullOrEmpty(product.Gallery.Tel) ? string.Empty : product.Gallery.Tel;
                        model.GalleryDescription = string.IsNullOrEmpty(product.Gallery.PurchaseDescription) ? string.Empty : product.Gallery.PurchaseDescription;
                        model.GalleryAddress = string.IsNullOrEmpty(product.Gallery.Address) ? string.Empty : product.Gallery.Address;
                        if (product.Gallery.Sellers.Any())
                        {
                            model.GallerySellers = product.Gallery.Sellers.Select(x => new GallerySellerForShowInSiteModel
                            {
                                Name = x.User.FullName,
                                Phone = x.User.Mobile,
                                ProfileImage = _fileService.IsExist(x.ImageName, _filePathAddress.SellerProfileImage) ? x.ImageName : string.Empty,
                            }).ToList();
                        }
                    }
                    #endregion

                    #region مبلغ پیش پرداخت خرید اقساطی

                    var prePaymentCalcResult = PrepaymentCalculation(model.GalleryProfitAmount, model.TaxAmount, model.WageAmount, product.Weight, model.GoldPrice);
                    if (prePaymentCalcResult.IsSuccess)
                        model.DefaultPrePayment = prePaymentCalcResult.Data;
                    #endregion
                }

                return CommandResult<ProductInfoForShowInSiteModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<ProductInfoForShowInSiteModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
            }
        }

        /// <summary>
        /// محاسبه خرید قسطی محصول
        /// </summary>
        /// <returns>مبلغ نهایی فاکور - مبلغ قسط</returns>
        public async Task<CommandResult<InstallmentPurchaseModel>> InstallmentPurchaseAsync(InstallmentPurchaseInputDataModel model, CancellationToken cancellationToken)
        {
            try
            {
                InstallmentPurchaseModel installmentPurchaseModel = new();
                var loanSettingsResult = await _settingService.GetSettingAsync<LoanSettingsViewModel>(SettingType.LoanSetting, cancellationToken);
                if (loanSettingsResult.IsSuccess)
                {
                    if (!string.IsNullOrEmpty(loanSettingsResult.Data.MonthlyProfitPercentage))
                    {
                        if (decimal.TryParse(loanSettingsResult.Data.MonthlyProfitPercentage, out decimal monthlyProfitPercentage))
                        {
                            var product = await _unitOfWork.ProductRepository.GetAllAsIQueryable().Data
                                                                             .Include(x => x.Gallery)
                                                                             .FirstOrDefaultAsync(x => x.Id == model.ProductId, cancellationToken);

                            model.PrePayment = NumberTools.RoundUpNumber(model.PrePayment.ToString(), 5);
                            if (product is not null)
                            {
                                //برای اقساطی کردن محصول باید چک شود که گالری محصول این امکان را دارد یا نه
                                if (!product.Gallery.HasInstallmentSale)
                                    return CommandResult<InstallmentPurchaseModel>.Failure(UserMessages.IsNotPossibleToSellThisProductInInstallments, new());

                                long goldPrice = 0;
                                var goldPriceResult = await _settingService.GetGoldPriceInfoAsync(cancellationToken);
                                if (goldPriceResult.IsSuccess)
                                    goldPrice = NumberTools.RoundUpNumber(Math.Ceiling(_settingService.CalcGoldPrice(((goldPriceResult.Data.Karat18 / 18) * product.Karat), product.Weight)).ToString(), 4);


                                long wageAmount = NumberTools.RoundUpNumber(Math.Ceiling(_settingService.CalcWageAmount(goldPrice, product.Wage)).ToString(), 4);
                                long galleryProfitAmount = NumberTools.RoundUpNumber(Math.Ceiling(_settingService.CalcGalleryProfitAmount(product.StonPrice, wageAmount, goldPrice, product.GalleryProfit)).ToString(), 4);
                                long taxAmount = NumberTools.RoundUpNumber(Math.Ceiling(_settingService.CalcTax(wageAmount, galleryProfitAmount, loanSettingsResult.Data.Tax.Value)).ToString(), 4);

                                var invoceTotalAmount = _settingService.InvoceTotalAmount(goldPrice, wageAmount, galleryProfitAmount, taxAmount, product.StonPrice);
                                long remainAmount = NumberTools.RoundUpNumber(Math.Ceiling(invoceTotalAmount).ToString(), 4) - model.PrePayment;
                                remainAmount = Math.Abs(remainAmount);

                                //decimal installmentAmount = (remainAmount + (model.InstallmentCount * monthlyProfitPercentage * remainAmount ))/ model.InstallmentCount;
                                var installmentAmountResult = await _customerService.InstallmentAmountCalculator(remainAmount, model.InstallmentCount, cancellationToken); ;

                                //installmentPurchaseModel.InstallmentAmount = NumberTools.RoundUpNumber(Math.Ceiling(installmentAmount).ToString(), 4);
                                installmentPurchaseModel.InstallmentAmount = installmentAmountResult.Data;
                                installmentPurchaseModel.InvoiceAmount = NumberTools.RoundUpNumber(goldPrice.ToString(), 4);
                                installmentPurchaseModel.RemainAmount = NumberTools.RoundUpNumber(remainAmount.ToString(), 4);
                            }
                        }
                    }
                }

                return CommandResult<InstallmentPurchaseModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, installmentPurchaseModel);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<InstallmentPurchaseModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }
        /// <summary>
        /// محاسبه مبلغ پیش پرداخت پیشفرض برای خرید قسطی محصول
        /// </summary>
        /// <param name="galleryProfitAmount">مبلغ سود گالری</param>
        /// <param name="taxAmount">مبلغ مالیات</param>
        /// <param name="wageAmount">مبلغ اجرت</param>
        /// <param name="weight">وزن محصول</param>
        /// <param name="karat">عیار</param>
        /// <returns>مبلغ پیش پرداخت محاسبه شده</returns>
        public CommandResult<long> PrepaymentCalculation(long galleryProfitAmount, long taxAmount, long wageAmount, decimal weight, long productAmount)
        {
            try
            {
                decimal weightValue = (1 / 10) * weight;
                var result = galleryProfitAmount + taxAmount + wageAmount + (productAmount * weightValue);
                long prePaymentValue = NumberTools.RoundUpNumber(Math.Ceiling(result).ToString(), 5);
                return CommandResult<long>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, prePaymentValue);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<long>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, 0);
            }
        }

        #endregion
        public async Task<CommandResult<string>> IsVisitedAsync(long productId, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _unitOfWork.ProductRepository.GetAllAsIQueryable().Data
                        .Where(x => x.Id == productId)
                        .FirstOrDefaultAsync(cancellationToken);
                if (product is not null)
                {
                    product.VisitedCount += 1;
                    _unitOfWork.ProductRepository.Update(product);
                    var result = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (result.IsSuccess)
                        return CommandResult<string>.Success(string.Format(UserMessages.TheProductIsVisited, product.Title), product.Title);
                    else
                        return CommandResult<string>.Failure(result.Message, product.Title);
                }
                else
                {
                    return CommandResult<string>.Failure(string.Format(OperationResultMessage.NotFound, Captions.Product), string.Empty);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, string.Empty);
            }
        }
    }
}
