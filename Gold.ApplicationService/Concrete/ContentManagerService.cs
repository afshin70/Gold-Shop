using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.ArticleModels;
using Gold.ApplicationService.Contract.DTOs.Models.FAQModels;
using Gold.ApplicationService.Contract.DTOs.Models.SiteContent;
using Gold.ApplicationService.Contract.DTOs.ViewModels.ArticleViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.FAQViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.SellerViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.SiteContent;
using Gold.Domain.Entities;
using Gold.Domain.Enums;
using Gold.Infrastracture.ExternalService;
using Gold.Infrastracture.Repositories;
using Gold.Infrastracture.Repositories.UOW;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.FileAddress;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.Enums;
using Gold.SharedKernel.ExtentionMethods;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Concrete
{
    public class ContentManagerService : IContentManagerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogManager _logManager;
        private readonly IFileService _fileService;
        private readonly FilePathAddress _filePathAddress;

        public ContentManagerService(IUnitOfWork unitOfWork, ILogManager logManager, IFileService fileService, IOptions<FilePathAddress> filePathAddressOptions)
        {
            _unitOfWork = unitOfWork;
            _logManager = logManager;
            _fileService = fileService;
            _filePathAddress = filePathAddressOptions.Value;
        }

        #region Article
        public async Task<CommandResult> ChangeArticleOrderNumberAsync(int id, bool isUp, CancellationToken cancellationToken)
        {
            try
            {
                var articleResult = await _unitOfWork.ArticleRepository.GetByIdAsync(id, cancellationToken);

                if (articleResult.IsSuccess)
                {


                    if (isUp)
                    {
                        //up
                        var previousItem = _unitOfWork.ArticleRepository.GetAllAsIQueryable().Data
                            .Where(x => x.OrderNo > articleResult.Data.OrderNo & x.Id != articleResult.Data.Id)
                            .OrderBy(x => x.OrderNo).FirstOrDefault();
                        if (previousItem is not null)
                        {
                            var tempItem = previousItem.OrderNo;
                            previousItem.OrderNo = articleResult.Data.OrderNo;
                            articleResult.Data.OrderNo = tempItem;
                            _unitOfWork.ArticleRepository.Update(previousItem);
                        }
                    }
                    else
                    {
                        //down
                        var nextItem = _unitOfWork.ArticleRepository.GetAllAsIQueryable().Data
                            .Where(x => x.OrderNo < articleResult.Data.OrderNo & x.Id != articleResult.Data.Id)
                            .OrderByDescending(x => x.OrderNo).FirstOrDefault();
                        if (nextItem is not null)
                        {
                            var tempItem = nextItem.OrderNo;
                            nextItem.OrderNo = articleResult.Data.OrderNo;
                            articleResult.Data.OrderNo = tempItem;
                            _unitOfWork.ArticleRepository.Update(nextItem);
                        }
                    }
                    //update
                    _unitOfWork.ArticleRepository.Update(articleResult.Data);
                    var updateResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (updateResult.IsSuccess)
                        return CommandResult.Success(UserMessages.DataEditedSuccessfully);
                    else
                        return CommandResult.Failure(updateResult.Message);

                }
                else
                {
                    return CommandResult.Failure(articleResult.Message);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
            }
        }

        public async Task<CommandResult<string>> ChangeArticleStatusAsync(int id, CancellationToken cancellationToken)
        {
            string title = string.Empty;
            try
            {
                var articleResult = await _unitOfWork.ArticleRepository.GetByIdAsync(id, cancellationToken);

                if (articleResult.IsSuccess)
                {
                    articleResult.Data.Status = !articleResult.Data.Status;
                    title = articleResult.Data.Title;
                    //update
                    _unitOfWork.ArticleRepository.Update(articleResult.Data);
                    var updateResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (updateResult.IsSuccess)
                        return CommandResult<string>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, title);
                    else
                        return CommandResult<string>.Failure(updateResult.Message, title);
                }
                else
                    return CommandResult<string>.Failure(articleResult.Message, title);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, title);
            }
        }

        public async Task<CommandResult<CreateOrEditArticleViewModel>> CreateOrEditArticleAsync(CreateOrEditArticleViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                string imageName = string.Empty;
                string videoName = string.Empty;
                if (model.Id == null)
                {
                    //add 
                    var lastOrderNumber = await _unitOfWork.ArticleRepository.GetAllAsIQueryable().Data
                        .OrderByDescending(x => x.OrderNo)
                        .Select(x => x.OrderNo)
                        .FirstOrDefaultAsync(cancellationToken);

                    var item = new Article
                    {
                        Title = model.Title,
                        Status = true,
                        RegisterDate = DateTime.Now,
                        Description = model.Description,
                        OrderNo = lastOrderNumber > 0 ? lastOrderNumber + 1 : 1
                    };

                    var addResult = await _unitOfWork.ArticleRepository.InsertAsync(item, cancellationToken);

                    if (addResult.IsSuccess)
                    {
                        var saveResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                        if (saveResult.IsSuccess)
                            return CommandResult<CreateOrEditArticleViewModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                        else
                            return CommandResult<CreateOrEditArticleViewModel>.Failure(saveResult.Message, model);
                    }
                    else
                        return CommandResult<CreateOrEditArticleViewModel>.Failure(addResult.Message, model);
                }
                else
                {
                    //edit
                    var result = await _unitOfWork.ArticleRepository.GetByIdAsync(model.Id.Value, cancellationToken);
                    if (result.IsSuccess)
                    {
                        result.Data.Title = model.Title;
                        result.Data.Description = model.Description;
                        var updateResult = _unitOfWork.ArticleRepository.Update(result.Data);
                        if (updateResult.IsSuccess)
                        {
                            var saveResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                            if (saveResult.IsSuccess)
                                return CommandResult<CreateOrEditArticleViewModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                            else
                                return CommandResult<CreateOrEditArticleViewModel>.Failure(saveResult.Message, model);
                        }
                        else
                            return CommandResult<CreateOrEditArticleViewModel>.Failure(updateResult.Message, model);
                    }
                    else
                        return CommandResult<CreateOrEditArticleViewModel>.Failure(result.Message, model);
                }

            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<CreateOrEditArticleViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
            }
        }


        public async Task<CommandResult<CreateOrEditArticleMediaViewModel>> ChangeImageOrVideoAsync(CreateOrEditArticleMediaViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.ArticleRepository.GetByIdAsync(model.Id.Value, cancellationToken);
                if (result.IsSuccess)
                {
                    if (model.IsVideo)
                    {
                        if (model.VideoFile is null)
                            return CommandResult<CreateOrEditArticleMediaViewModel>.Failure(string.Format(ValidationMessages.Required, Captions.Video), model);

                        #region Update or Delete Video File
                        string newVideoName = Guid.NewGuid().ToString();
                        if (model.VideoFile is not null)
                        {
                            //string path = _filePathAddress.ArticleImages;
                            string lastVideoName = string.IsNullOrEmpty(result.Data.VideoFileName) ? string.Empty : result.Data.VideoFileName;
                            if (!await _fileService.UpdateFileAsync(model.VideoFile, newVideoName, lastVideoName, _filePathAddress.ArticleVideos))
                                return CommandResult<CreateOrEditArticleMediaViewModel>.Failure(UserMessages.ErrorInUpdateFile, model);

                            result.Data.VideoFileName = $"{newVideoName}{Path.GetExtension(model.VideoFile.FileName)}";
                            model.FileName = result.Data.VideoFileName;
                        }
                        #endregion


                    }
                    else
                    {
                        if (model.ImageFile is null)
                            return CommandResult<CreateOrEditArticleMediaViewModel>.Failure(string.Format(ValidationMessages.Required, Captions.Image), model);

                        string lastImageName = string.IsNullOrEmpty(result.Data.ImageFileName) ? string.Empty : result.Data.ImageFileName;
                        //delete last image
                        #region Save Image File
                        string imageName = Guid.NewGuid().ToString();
                        if (model.ImageFile is not null)
                        {
                            _fileService.DeleteFile(lastImageName, _filePathAddress.ArticleImages);
                            _fileService.DeleteFile(lastImageName, _filePathAddress.ArticleThumbnailImages);
                            //save thumbnail image
                            if (!_fileService.SaveImageFile(model.ImageFile, _filePathAddress.ArticleThumbnailImageWith, _filePathAddress.ArticleThumbnailImageHeight, _filePathAddress.ArticleThumbnailImages, imageName))
                                return CommandResult<CreateOrEditArticleMediaViewModel>.Failure(UserMessages.ErrorInUploadFile, model);

                            if (!await _fileService.UploadFileAsync(model.ImageFile, _filePathAddress.ArticleImages, imageName))
                                return CommandResult<CreateOrEditArticleMediaViewModel>.Failure(UserMessages.ErrorInUploadFile, model);

                            result.Data.ImageFileName = imageName + Path.GetExtension(model.ImageFile.FileName);
                            model.FileName = result.Data.ImageFileName;
                        }

                        #endregion
                    }
                    var updateResult = _unitOfWork.ArticleRepository.Update(result.Data);
                    if (updateResult.IsSuccess)
                    {
                        var saveResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                        if (saveResult.IsSuccess)
                            return CommandResult<CreateOrEditArticleMediaViewModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                        else
                            return CommandResult<CreateOrEditArticleMediaViewModel>.Failure(saveResult.Message, model);
                    }
                    else
                        return CommandResult<CreateOrEditArticleMediaViewModel>.Failure(updateResult.Message, model);

                }
                else
                {
                    return CommandResult<CreateOrEditArticleMediaViewModel>.Failure(result.Message, model);
                }

            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<CreateOrEditArticleMediaViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
            }
        }
        public async Task<CommandResult> RemoveImageOrVideoAsync(int id, bool isVideo, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.ArticleRepository.GetByIdAsync(id, cancellationToken);
                if (result.IsSuccess)
                {
                    if (isVideo)
                    {
                        #region Delete Video File
                        if (!string.IsNullOrEmpty(result.Data.VideoFileName))
                        {
                            if (!_fileService.DeleteFile(result.Data.VideoFileName, _filePathAddress.ArticleVideos))
                                return CommandResult.Failure(UserMessages.ErrorInDeleteFile);

                            result.Data.VideoFileName = null;
                        }
                        #endregion


                    }
                    else
                    {
                        #region Delete Image File
                        if (!string.IsNullOrEmpty(result.Data.ImageFileName))
                        {
                            if (_fileService.DeleteFile(result.Data.ImageFileName, _filePathAddress.ArticleImages) |
                                _fileService.DeleteFile(result.Data.ImageFileName, _filePathAddress.ArticleThumbnailImages))
                            {
                                result.Data.ImageFileName = null;
                            }
                        }

                        #endregion
                    }
                    var updateResult = _unitOfWork.ArticleRepository.Update(result.Data);
                    if (updateResult.IsSuccess)
                    {
                        var saveResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                        if (saveResult.IsSuccess)
                            return CommandResult.Success(OperationResultMessage.OperationIsSuccessfullyCompleted);
                        else
                            return CommandResult.Failure(saveResult.Message);
                    }
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
                _logManager.RaiseLog(ex);
                return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
            }
        }


        public CommandResult<IQueryable<ArticleModel>> GetArticleAsQuerable()
        {
            try
            {
                var result = _unitOfWork.ArticleRepository.GetAllAsIQueryable();
                if (result.IsSuccess)
                {
                    var iQuerableData = result.Data.Select(x => new ArticleModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        OrderNo = x.OrderNo,
                        Status = x.Status,
                        StatusTitle = x.Status ? Captions.Active : Captions.DeActive,
                        RegisterDate = x.RegisterDate,
                        RegisterDatePersian = x.RegisterDate.GeorgianToPersian(ShowMode.OnlyDateAndTime),
                    });
                    return CommandResult<IQueryable<ArticleModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, iQuerableData);
                }
                else
                {
                    return CommandResult<IQueryable<ArticleModel>>.Failure(result.Message, null);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<ArticleModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult<ArticleModel>> GetArticleByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var findItemResult = await _unitOfWork.ArticleRepository.GetByIdAsync(id, cancellationToken);
                if (findItemResult.IsSuccess)
                {
                    ArticleModel model = new()
                    {
                        Id = findItemResult.Data.Id,
                        Description = findItemResult.Data.Description,
                        ImageFileName = findItemResult.Data.ImageFileName,
                        OrderNo = findItemResult.Data.OrderNo,
                        RegisterDate = findItemResult.Data.RegisterDate,
                        Status = findItemResult.Data.Status,
                        Title = findItemResult.Data.Title,
                        VideoFileName = findItemResult.Data.VideoFileName,
                    };
                    return CommandResult<ArticleModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                }
                else
                    return CommandResult<ArticleModel>.Failure(UserMessages.UserNotFound, new());
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<ArticleModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }

        public async Task<CommandResult<string>> RemoveArticleAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var itemResult = await _unitOfWork.ArticleRepository.GetByIdAsync(id, cancellationToken);
                if (itemResult.IsSuccess)
                {
                    #region delete article image and video files
                    //remove image
                    if (!string.IsNullOrEmpty(itemResult.Data.ImageFileName))
                    {
                        _fileService.DeleteFile(itemResult.Data.ImageFileName, _filePathAddress.ArticleImages);
                        _fileService.DeleteFile(itemResult.Data.ImageFileName, _filePathAddress.ArticleThumbnailImages);
                    }
                    //remove video
                    if (!string.IsNullOrEmpty(itemResult.Data.VideoFileName))
                        _fileService.DeleteFile(itemResult.Data.VideoFileName, _filePathAddress.ArticleVideos);
                    #endregion

                    _unitOfWork.ArticleRepository.Delete(itemResult.Data);
                    var result = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (result.IsSuccess)
                        return CommandResult<string>.Success(UserMessages.DataDeletedSuccessfully, itemResult.Data.Title);
                    else
                        return CommandResult<string>.Failure(result.Message, itemResult.Data.Title);
                }
                else
                    return CommandResult<string>.Failure(itemResult.Message, string.Empty);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, string.Empty);
            }
        }

        public async Task<CommandResult<List<ArticleForShowInSiteModel>>> GetArticlesForShoInSiteAsync(int page, int pageSize, string searchTerm, CancellationToken cancellationToken)
        {
            try
            {
                var query = _unitOfWork.ArticleRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Status == true);
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(x => x.Title.Contains(searchTerm));
                }

                var list = await query
                      .OrderByDescending(x => x.OrderNo)
                      .Skip(pageSize * (page - 1))
                      .Take(pageSize)
                      .Select(x => new ArticleForShowInSiteModel
                      {
                          Id = x.Id,
                          //Description = string.IsNullOrEmpty(x.Description) ? string.Empty : x.Description,
                          ImageFileName = string.IsNullOrEmpty(x.ImageFileName) ? "default.png" : x.ImageFileName,
                          Title = x.Title,
                          HasVideo = !string.IsNullOrEmpty(x.VideoFileName),
                          VideoFileName = x.VideoFileName,
                      }).ToListAsync(cancellationToken);
                return CommandResult<List<ArticleForShowInSiteModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, list);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<List<ArticleForShowInSiteModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }
        public async Task<CommandResult<ArticleForShowInSiteModel>> GetArticleInfoForShoInSiteAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _unitOfWork.ArticleRepository.GetAllAsIQueryable().Data
                    .Where(x => x.Status == true & x.Id == id)
                      .Select(x => new ArticleForShowInSiteModel
                      {
                          Id = x.Id,
                          Description = string.IsNullOrEmpty(x.Description) ? string.Empty : x.Description,
                          ImageFileName = x.ImageFileName,
                          Title = x.Title,
                          VideoFileName = x.VideoFileName,
                          HasVideo = !string.IsNullOrEmpty(x.VideoFileName),
                      }).FirstOrDefaultAsync(cancellationToken);
                if (item is not null)
                    return CommandResult<ArticleForShowInSiteModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, item);
                else
                    return CommandResult<ArticleForShowInSiteModel>.Failure(string.Format(OperationResultMessage.NotFound, Captions.Article), new());
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<ArticleForShowInSiteModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }
        #endregion

        #region Faq And Faq Category
        public async Task<CommandResult<FAQCategoryModel>> GetFAQCategoryByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.FAQCategoryRepository.GetByIdAsync(id, cancellationToken);
                if (result.IsSuccess)
                {
                    var model = new FAQCategoryModel
                    {
                        Id = result.Data.Id,
                        OrderNo = result.Data.OrderNo,
                        Title = result.Data.Title,
                    };
                    return CommandResult<FAQCategoryModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                }
                else
                    return CommandResult<FAQCategoryModel>.Failure(string.Format(OperationResultMessage.NotFound, Captions.FAQCategory), new());

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<FAQCategoryModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }
        public async Task<CommandResult<FAQModel>> GetFAQByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.FAQRepository.GetByIdAsync(id, cancellationToken);
                if (result.IsSuccess)
                {
                    var model = new FAQModel
                    {
                        Id = result.Data.Id,
                        OrderNo = result.Data.OrderNo,
                        Question = result.Data.Question,
                        Answer = result.Data.Answer,
                        CategoryId = result.Data.CategoryId
                    };
                    return CommandResult<FAQModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                }
                else
                    return CommandResult<FAQModel>.Failure(string.Format(OperationResultMessage.NotFound, Captions.FAQCategory), new());

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<FAQModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }
        public CommandResult<IQueryable<FAQModel>> GetFAQAsQuerable()
        {
            try
            {
                var result = _unitOfWork.FAQRepository.GetAllAsIQueryable();
                if (result.IsSuccess)
                {
                    var iQuerableData = result.Data
                        .Include(x => x.Category)
                        .OrderBy(x => x.Category.Title)
                        //.OrderBy(x=>x.Question)
                        .ThenBy(x => x.OrderNo)
                        .Select(x => new FAQModel
                        {
                            Id = x.Id,
                            Question = x.Question,
                            OrderNo = x.OrderNo,
                            Answer = x.Answer,
                            CategoryId = x.CategoryId,
                            Category = x.Category.Title
                        });
                    return CommandResult<IQueryable<FAQModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, iQuerableData);
                }
                else
                {
                    return CommandResult<IQueryable<FAQModel>>.Failure(result.Message, null);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<FAQModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }
        public CommandResult<List<FAQModel>> GetFAQAsList()
        {
            List<FAQModel> resultList = new();
            try
            {
                var faqCategory = _unitOfWork.FAQCategoryRepository.GetAllAsIQueryable();
                if (faqCategory.IsSuccess)
                {
                    var list = faqCategory.Data
                        .Include(x => x.FAQs)
                        .ToList();
                    foreach (var item in list.OrderBy(x=>x.OrderNo))
                    {
                        foreach (var question in item.FAQs.OrderBy(x=>x.OrderNo).ThenBy(x=>x.Question))
                        {
                            resultList.Add(new FAQModel
                            {
                                Id = question.Id,
                                Answer = question.Answer,
                                CategoryId = item.Id,
                                Question=question.Question,
                                Category=item.Title,
                                OrderNo=question.OrderNo,
                            });
                        }
                    }
                }
                return CommandResult<List<FAQModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, resultList);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<List<FAQModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, resultList);
            }
        }

        public CommandResult<IQueryable<FAQCategoryModel>> GetFAQCategoryAsQuerable()
        {
            try
            {
                var result = _unitOfWork.FAQCategoryRepository.GetAllAsIQueryable();
                if (result.IsSuccess)
                {
                    var iQuerableData = result.Data.Select(x => new FAQCategoryModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        OrderNo = x.OrderNo
                    });
                    return CommandResult<IQueryable<FAQCategoryModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, iQuerableData);
                }
                else
                {
                    return CommandResult<IQueryable<FAQCategoryModel>>.Failure(result.Message, null);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<FAQCategoryModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult<CreateOrEditFAQCategoryViewModel>> CreateOrEditFAQCategoryAsync(CreateOrEditFAQCategoryViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                if (model.Id == null)
                {
                    //add 
                    bool isDuoplicate = await _unitOfWork.FAQCategoryRepository.GetAllAsIQueryable().Data
                           .AnyAsync(x => x.Title == model.Title, cancellationToken);
                    if (isDuoplicate)
                        return CommandResult<CreateOrEditFAQCategoryViewModel>.Failure(string.Format(UserMessages.IsDuplicated, Captions.Title), model);

                    var lastOrderNumber = await _unitOfWork.FAQCategoryRepository.GetAllAsIQueryable().Data
                        .OrderByDescending(x => x.OrderNo)
                        .Select(x => x.OrderNo)
                        .FirstOrDefaultAsync(cancellationToken);

                    var item = new FAQCategory
                    {
                        Title = model.Title,
                        OrderNo = lastOrderNumber > 0 ? lastOrderNumber + 1 : 1
                    };
                    var addResult = await _unitOfWork.FAQCategoryRepository.InsertAsync(item, cancellationToken);
                    if (addResult.IsSuccess)
                    {
                        var saveResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                        if (saveResult.IsSuccess)
                            return CommandResult<CreateOrEditFAQCategoryViewModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                        else
                            return CommandResult<CreateOrEditFAQCategoryViewModel>.Failure(saveResult.Message, model);
                    }
                    else
                        return CommandResult<CreateOrEditFAQCategoryViewModel>.Failure(addResult.Message, model);
                }
                else
                {
                    //edit
                    var result = await _unitOfWork.FAQCategoryRepository.GetByIdAsync(model.Id.Value, cancellationToken);
                    if (result.IsSuccess)
                    {
                        bool isDuoplicate = await _unitOfWork.FAQCategoryRepository.GetAllAsIQueryable().Data
                            .AnyAsync(x => x.Id != result.Data.Id & x.Title == model.Title, cancellationToken);
                        if (isDuoplicate)
                            return CommandResult<CreateOrEditFAQCategoryViewModel>.Failure(string.Format(UserMessages.IsDuplicated, Captions.Title), model);
                        result.Data.Title = model.Title;
                        var updateResult = _unitOfWork.FAQCategoryRepository.Update(result.Data);
                        if (updateResult.IsSuccess)
                        {
                            var saveResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                            if (saveResult.IsSuccess)
                                return CommandResult<CreateOrEditFAQCategoryViewModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                            else
                                return CommandResult<CreateOrEditFAQCategoryViewModel>.Failure(saveResult.Message, model);
                        }
                        else
                            return CommandResult<CreateOrEditFAQCategoryViewModel>.Failure(updateResult.Message, model);
                    }
                    else
                        return CommandResult<CreateOrEditFAQCategoryViewModel>.Failure(result.Message, model);
                }

            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<CreateOrEditFAQCategoryViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
            }
        }
        public async Task<CommandResult<CreateOrEditFAQViewModel>> CreateOrEditFAQAsync(CreateOrEditFAQViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Question))
                    return CommandResult<CreateOrEditFAQViewModel>.Failure(string.Format(ValidationMessages.Required, Captions.Question), model);
                if (string.IsNullOrEmpty(model.Answer))
                    return CommandResult<CreateOrEditFAQViewModel>.Failure(string.Format(ValidationMessages.Required, Captions.Answer), model);
                if (!model.FaqCategoryId.HasValue)
                    return CommandResult<CreateOrEditFAQViewModel>.Failure(string.Format(ValidationMessages.Required, Captions.Category), model);

                if (model.Id == null)
                {
                    //add 
                    var lastOrderNumber = await _unitOfWork.FAQRepository.GetAllAsIQueryable().Data
                        .OrderByDescending(x => x.OrderNo)
                        .Select(x => x.OrderNo)
                        .FirstOrDefaultAsync(cancellationToken);
                    var item = new FAQ
                    {
                        Question = model.Question,
                        Answer = model.Answer,
                        CategoryId = model.FaqCategoryId.Value,
                        OrderNo = lastOrderNumber > 0 ? lastOrderNumber + 1 : 1
                    };
                    var addResult = await _unitOfWork.FAQRepository.InsertAsync(item, cancellationToken);
                    if (addResult.IsSuccess)
                    {
                        var saveResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                        if (saveResult.IsSuccess)
                            return CommandResult<CreateOrEditFAQViewModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                        else
                            return CommandResult<CreateOrEditFAQViewModel>.Failure(saveResult.Message, model);
                    }
                    else
                        return CommandResult<CreateOrEditFAQViewModel>.Failure(addResult.Message, model);
                }
                else
                {
                    //edit
                    var result = await _unitOfWork.FAQRepository.GetByIdAsync(model.Id.Value, cancellationToken);
                    if (result.IsSuccess)
                    {
                        result.Data.Question = model.Question;
                        result.Data.Answer = model.Answer;
                        result.Data.CategoryId = model.FaqCategoryId.Value;
                        var updateResult = _unitOfWork.FAQRepository.Update(result.Data);
                        if (updateResult.IsSuccess)
                        {
                            var saveResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                            if (saveResult.IsSuccess)
                                return CommandResult<CreateOrEditFAQViewModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                            else
                                return CommandResult<CreateOrEditFAQViewModel>.Failure(saveResult.Message, model);
                        }
                        else
                            return CommandResult<CreateOrEditFAQViewModel>.Failure(updateResult.Message, model);
                    }
                    else
                        return CommandResult<CreateOrEditFAQViewModel>.Failure(result.Message, model);
                }

            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<CreateOrEditFAQViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
            }
        }

        public async Task<CommandResult<string>> RemoveFAQCategoryAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var fAQCategoryResult = await _unitOfWork.FAQCategoryRepository.GetByIdAsync(id, cancellationToken);
                if (fAQCategoryResult.IsSuccess)
                {
                    _unitOfWork.FAQCategoryRepository.Delete(fAQCategoryResult.Data);

                    var result = await _unitOfWork.CommitChangesAsync(cancellationToken);

                    if (result.IsSuccess)
                        return CommandResult<string>.Success(UserMessages.DataDeletedSuccessfully, fAQCategoryResult.Data.Title);/*string.Format(UserMessages.Gallery0IsDeleted, galleryResult.Data.Name)*/
                    else
                        return CommandResult<string>.Failure(result.Message, fAQCategoryResult.Data.Title);
                }
                else
                    return CommandResult<string>.Failure(fAQCategoryResult.Message, string.Empty);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, string.Empty);
            }
        }
        public async Task<CommandResult<string>> RemoveFAQAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var fAQResult = await _unitOfWork.FAQRepository.GetByIdAsync(id, cancellationToken);
                if (fAQResult.IsSuccess)
                {
                    _unitOfWork.FAQRepository.Delete(fAQResult.Data);
                    var result = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (result.IsSuccess)
                        return CommandResult<string>.Success(UserMessages.DataDeletedSuccessfully, fAQResult.Data.Question);
                    else
                        return CommandResult<string>.Failure(result.Message, fAQResult.Data.Question);
                }
                else
                    return CommandResult<string>.Failure(fAQResult.Message, string.Empty);

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, string.Empty);
            }
        }
        public async Task<CommandResult> ChangeFAQOrderNumberAsync(int id, bool isUp, CancellationToken cancellationToken)
        {
            try
            {
                var fAQResult = await _unitOfWork.FAQRepository.GetByIdAsync(id, cancellationToken);

                if (fAQResult.IsSuccess)
                {
                    if (isUp)
                    {
                        //up
                        var previousItem = _unitOfWork.FAQRepository.GetAllAsIQueryable().Data
                            .Where(x => x.OrderNo < fAQResult.Data.OrderNo & x.CategoryId == fAQResult.Data.CategoryId)
                            .OrderByDescending(x => x.OrderNo)
                            .FirstOrDefault();
                        if (previousItem is not null)
                        {
                            var tempItem = previousItem.OrderNo;
                            previousItem.OrderNo = fAQResult.Data.OrderNo;
                            fAQResult.Data.OrderNo = tempItem;
                            _unitOfWork.FAQRepository.Update(previousItem);
                        }
                    }
                    else
                    {
                        //down
                        var nextItem = _unitOfWork.FAQRepository.GetAllAsIQueryable().Data
                            .Where(x => x.OrderNo > fAQResult.Data.OrderNo & x.CategoryId == fAQResult.Data.CategoryId)
                            .OrderBy(x => x.OrderNo)
                            .FirstOrDefault();

                        if (nextItem is not null)
                        {
                            var tempItem = nextItem.OrderNo;
                            nextItem.OrderNo = fAQResult.Data.OrderNo;
                            fAQResult.Data.OrderNo = tempItem;
                            _unitOfWork.FAQRepository.Update(nextItem);
                        }
                    }
                    //update
                    _unitOfWork.FAQRepository.Update(fAQResult.Data);
                    var updateResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (updateResult.IsSuccess)
                        return CommandResult.Success(UserMessages.DataEditedSuccessfully);
                    else
                        return CommandResult.Failure(updateResult.Message);

                }
                else
                {
                    return CommandResult.Failure(fAQResult.Message);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
            }
        }
        public async Task<CommandResult> ChangeFAQCategoryOrderNumberAsync(int id, bool isUp, CancellationToken cancellationToken)
        {
            try
            {
                var fAQCategoryResult = await _unitOfWork.FAQCategoryRepository.GetByIdAsync(id, cancellationToken);

                if (fAQCategoryResult.IsSuccess)
                {
                    if (isUp)
                    {
                        //up
                        var previousItem = _unitOfWork.FAQCategoryRepository.GetAllAsIQueryable().Data
                            .Where(x => x.OrderNo < fAQCategoryResult.Data.OrderNo)
                            .OrderByDescending(x => x.OrderNo)
                            .FirstOrDefault();
                        if (previousItem is not null)
                        {
                            var tempItem = previousItem.OrderNo;
                            previousItem.OrderNo = fAQCategoryResult.Data.OrderNo;
                            fAQCategoryResult.Data.OrderNo = tempItem;
                            _unitOfWork.FAQCategoryRepository.Update(previousItem);
                        }
                    }
                    else
                    {
                        //down
                        var nextItem = _unitOfWork.FAQCategoryRepository.GetAllAsIQueryable().Data
                            .Where(x => x.OrderNo > fAQCategoryResult.Data.OrderNo)
                            .OrderBy(x => x.OrderNo)
                            .FirstOrDefault();

                        if (nextItem is not null)
                        {
                            var tempItem = nextItem.OrderNo;
                            nextItem.OrderNo = fAQCategoryResult.Data.OrderNo;
                            fAQCategoryResult.Data.OrderNo = tempItem;
                            _unitOfWork.FAQCategoryRepository.Update(nextItem);
                        }
                    }
                    //update
                    _unitOfWork.FAQCategoryRepository.Update(fAQCategoryResult.Data);
                    var updateResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (updateResult.IsSuccess)
                        return CommandResult.Success(UserMessages.DataEditedSuccessfully);
                    else
                        return CommandResult.Failure(updateResult.Message);

                }
                else
                {
                    return CommandResult.Failure(fAQCategoryResult.Message);
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware);
            }
        }
        #endregion

        #region Contact Us
        public async Task<CommandResult<ContactUsViweModel>> CreateContactUsMessageAsync(ContactUsViweModel model, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(model.FullName))
                    return CommandResult<ContactUsViweModel>.Failure(string.Format(ValidationMessages.Required, Captions.FullName), model);
                if (string.IsNullOrEmpty(model.Message))
                    return CommandResult<ContactUsViweModel>.Failure(string.Format(ValidationMessages.Required, Captions.Message), model);

                if (!string.IsNullOrEmpty(model.Phone))
                    model.Phone = model.Phone.ToEnglishNumbers();
                var item = new ContactUs
                {
                    FullName = model.FullName,
                    Phone = model.Phone,
                    Message = model.Message,
                    CreateDate = DateTime.Now,
                };
                var addResult = await _unitOfWork.ContactUsRepository.InsertAsync(item, cancellationToken);
                if (addResult.IsSuccess)
                {
                    var saveResult = await _unitOfWork.CommitChangesAsync(cancellationToken);
                    if (saveResult.IsSuccess)
                        return CommandResult<ContactUsViweModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, model);
                    else
                        return CommandResult<ContactUsViweModel>.Failure(saveResult.Message, model);
                }
                else
                    return CommandResult<ContactUsViweModel>.Failure(addResult.Message, model);

            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<ContactUsViweModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, model);
            }
        }

        public CommandResult<IQueryable<ContactUsMessageModel>> GetContactUsMessagesAsQuerable()
        {
            try
            {
                var result = _unitOfWork.ContactUsRepository.GetAllAsIQueryable();
                if (result.IsSuccess)
                {
                    var iQuerableData = result.Data.Select(x => new ContactUsMessageModel
                    {
                        Id = x.Id,
                        FullName = x.FullName,
                        Message = x.Message,
                        Phone = x.Phone,
                        SendDate = x.CreateDate,
                        SendDatePersian = x.CreateDate.GeorgianToPersian(ShowMode.OnlyDateAndTime),
                    });
                    return CommandResult<IQueryable<ContactUsMessageModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, iQuerableData);
                }
                else
                {
                    return CommandResult<IQueryable<ContactUsMessageModel>>.Failure(result.Message, null);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<ContactUsMessageModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }


        public async Task<CommandResult<ContactUsMessageModel>> RemoveMessageAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var itemResult = await _unitOfWork.ContactUsRepository.GetByIdAsync(id, cancellationToken);
                if (itemResult.IsSuccess)
                {
                    ContactUsMessageModel resultModel = new()
                    {
                        FullName = itemResult.Data.FullName,
                        SendDate = itemResult.Data.CreateDate,
                        SendDatePersian = itemResult.Data.CreateDate.GeorgianToPersian(ShowMode.OnlyDateAndTime)
                    };
                    //remove user and customer and essentialTels
                    _unitOfWork.ContactUsRepository.Delete(itemResult.Data);

                    var result = await _unitOfWork.CommitChangesAsync(cancellationToken);

                    if (result.IsSuccess)
                        return CommandResult<ContactUsMessageModel>.Success(UserMessages.DataDeletedSuccessfully, resultModel);
                    else
                        return CommandResult<ContactUsMessageModel>.Failure(result.Message, resultModel);
                }
                else
                {
                    return CommandResult<ContactUsMessageModel>.Failure(itemResult.Message, new());
                }

            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<ContactUsMessageModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }
        #endregion
    }
}
