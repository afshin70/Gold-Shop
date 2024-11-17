using Gold.ApplicationService.Contract.Abstractions;
using Gold.ApplicationService.Contract.DTOs.Models.AccessLevelModels;
using Gold.ApplicationService.Contract.DTOs.Models.ManagerModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.AccessLevelViewModels;
using Gold.ApplicationService.Contract.DTOs.ViewModels.ManagerViewModels;
using Gold.Domain.Entities;
using Gold.Domain.Entities.AuthEntities;
using Gold.Infrastracture.Repositories.UOW;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.Enums;
using Gold.SharedKernel.ExtentionMethods;
using Gold.SharedKernel.Security;
using Microsoft.EntityFrameworkCore;

namespace Gold.ApplicationService.Concrete
{
    public class ManagerService : IManagerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogManager _logManager;

        public ManagerService(IUnitOfWork unitOfWork, ILogManager logManager)
        {
            _unitOfWork = unitOfWork;
            this._logManager = logManager;
        }

        public CommandResult<IQueryable<ManagerUserModel>> GetManagerListAsQuerable()
        {
            try
            {
                var result = _unitOfWork.UserRepository.GetAllAsIQueryable();
                if (result.IsSuccess)
                {
                    IQueryable<ManagerUserModel> finallQuery = result.Data
                        .Where(x=>x.UserType==UserType.Manager)
                        .Include(x=>x.Manager).ThenInclude(x=>x.Permission)
                        .Select(x => new ManagerUserModel
                    {
                        ManagerUserId=x.Id,
                        AccessLevelTitle=(x.Manager.Permission==null)?string.Empty: x.Manager.Permission.Title,
                        FullName=x.FullName,
                        IsActive=x.IsActive,
                        Mobile=x.Mobile,
                        UserName=x.UserName,
                    }).AsQueryable();

                    return CommandResult<IQueryable<ManagerUserModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, finallQuery);
                }
                else
                {
                    return CommandResult<IQueryable<ManagerUserModel>>.Failure(result.Message, null);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<ManagerUserModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }
        
        
        public CommandResult<IQueryable<ManagerUserReportModel>> GetManagerReportListAsQuerable()
        {
            try
            {
                var result = _unitOfWork.UserRepository.GetAllAsIQueryable();
                if (result.IsSuccess)
                {
                    IQueryable<ManagerUserReportModel> finallQuery = result.Data
                        .Where(x=>x.UserType==UserType.Manager)
                        .Include(x=>x.Manager).ThenInclude(x=>x.Permission)
                        .Select(x => new ManagerUserReportModel
                        {
                        ManagerUserId=x.Id,
                        AccessLevelTitle=(x.Manager.Permission==null)?string.Empty: x.Manager.Permission.Title,
                        FullName=x.FullName,
                        IsActive=x.IsActive,
                        Mobile=x.Mobile,
                        UserName=x.UserName,
                    }).AsQueryable();

                    return CommandResult<IQueryable<ManagerUserReportModel>>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, finallQuery);
                }
                else
                {
                    return CommandResult<IQueryable<ManagerUserReportModel>>.Failure(result.Message, null);
                }
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<IQueryable<ManagerUserReportModel>>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, null);
            }
        }

        public async Task<CommandResult<CreateOrEditManagerUserViewModel>> GetManagerUserForEditAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                int userId = id;
                CommandResult<User> managerUserResult =await _unitOfWork.UserRepository.GetManagerUserAsync(userId, cancellationToken);
                if (managerUserResult.IsSuccess)
                {
                    CreateOrEditManagerUserViewModel user = new()
                    {
                        ManagerUserId= managerUserResult.Data.Id,
                        AccessLevelId= managerUserResult.Data.Manager.PermissionId,
                        AccessLevels=new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>(),
                        ConfirmPassword=string.Empty,
                        FullName= managerUserResult.Data.FullName,
                        IsActive= managerUserResult.Data.IsActive,
                        Mobile= managerUserResult.Data.Mobile,
                        Password=string.Empty,
                        UserName= managerUserResult.Data.UserName,
                    };
                    return CommandResult<CreateOrEditManagerUserViewModel>.Success(OperationResultMessage.OperationIsSuccessfullyCompleted, user);
                }
                else
                {
                    return CommandResult<CreateOrEditManagerUserViewModel>.Failure(managerUserResult.Message, new());
                }
            }
            catch (Exception ex)
            {
              await  _logManager.RaiseLogAsync(ex,cancellationToken);
                return CommandResult<CreateOrEditManagerUserViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }

        public async Task<CommandResult<CreateOrEditManagerUserViewModel>> AddOrEditManagerUserAsync(CreateOrEditManagerUserViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                if (model.ManagerUserId<=0)
                {
                    //add
                    if (string.IsNullOrEmpty(model.Password))
                        return CommandResult<CreateOrEditManagerUserViewModel>.Failure(string.Format(ValidationMessages.Required, Captions.Password), model);

                    bool isDuplicateUserName=_unitOfWork.UserRepository.GetAllAsIQueryable().Data.Any(x=>x.UserName==model.UserName);
                    if(isDuplicateUserName)
                        return CommandResult<CreateOrEditManagerUserViewModel>.Failure(string.Format(UserMessages.IsDuplicated, Captions.UserName), model);

                    bool isDuplicateFullName = _unitOfWork.UserRepository.GetAllAsIQueryable().Data.Any(x => x.FullName == model.FullName);
                    if (isDuplicateFullName)
                        return CommandResult<CreateOrEditManagerUserViewModel>.Failure(string.Format(UserMessages.IsDuplicated, Captions.FullName), model);

                    model.AccessLevelId ??= 0;
					var permissionResult = await _unitOfWork.PermissionRepository.GetByIdAsync(model.AccessLevelId.Value, cancellationToken);
                    if (!permissionResult.IsSuccess)
                        return CommandResult<CreateOrEditManagerUserViewModel>.Failure(string.Format(ValidationMessages.Invalid, Captions.AccessLevel), model);
                   
                    string passwordSalt = Guid.NewGuid().ToString();

                    User newManagerUser = new User
                    {
                        FullName= model.FullName,
                        IsActive=model.IsActive,
                        Mobile= model.Mobile,
                        SecurityStamp=Guid.NewGuid(),
                        UserName= model.UserName,
                        UserType=UserType.Manager,
                        RegisterDate=DateTime.Now,
                        PasswordSalt= passwordSalt,
                        PasswordHash= Encryptor.Encrypt(model.Password,passwordSalt),
                        Manager=new Manager
                        {
                            PermissionId= permissionResult.Data.Id,
                        }
                    };

                    await _unitOfWork.AddAsync(newManagerUser, cancellationToken);
                    
                }
                else
                {
                    //edit
                    CommandResult<User> managerUserResult = await _unitOfWork.UserRepository.GetManagerUserAsync(model.ManagerUserId, cancellationToken);
                    if (managerUserResult.IsSuccess)
                    {
                        //change UserName
                        if (managerUserResult.Data.UserName!=model.UserName)
                        {
                            bool isDuplicateUserName = _unitOfWork.UserRepository.GetAllAsIQueryable().Data.Any(x => x.UserName == model.UserName);
                            if (isDuplicateUserName)
                                return CommandResult<CreateOrEditManagerUserViewModel>.Failure(string.Format(UserMessages.IsDuplicated, Captions.UserName), model);
                            managerUserResult.Data.UserName=model.UserName;
                        }

                        //change fullName
                        if (managerUserResult.Data.FullName != model.FullName)
                        {
                            bool isDuplicateFullName = _unitOfWork.UserRepository.GetAllAsIQueryable().Data.Any(x => x.FullName == model.FullName);
                            if (isDuplicateFullName)
                                return CommandResult<CreateOrEditManagerUserViewModel>.Failure(string.Format(UserMessages.IsDuplicated, Captions.FullName), model);
                            managerUserResult.Data.FullName = model.FullName;
                        }

                        //change PermissionId
                        if (managerUserResult.Data.Manager.PermissionId!=model.AccessLevelId)
                        {
                            model.AccessLevelId ??= 0;
							var permissionResult = await _unitOfWork.PermissionRepository.GetByIdAsync(model.AccessLevelId.Value, cancellationToken);
                            if (!permissionResult.IsSuccess)
                                return CommandResult<CreateOrEditManagerUserViewModel>.Failure(string.Format(ValidationMessages.Invalid, Captions.AccessLevel), model);
                            managerUserResult.Data.Manager.PermissionId = permissionResult.Data.Id;
                        }
                        //change Password
                        

						if (!string.IsNullOrEmpty(model.Password))
                        {
                            managerUserResult.Data.PasswordHash = Encryptor.Encrypt(model.Password, managerUserResult.Data.PasswordSalt);
                            managerUserResult.Data.LastPasswordChangeDate= DateTime.Now;
                            managerUserResult.Data.WrongPasswordCount= 0;
                        }

                        //reset securityStamp if manager info is changed
                        managerUserResult.Data.SecurityStamp = Guid.NewGuid();

                        managerUserResult.Data.FullName = model.FullName;
                        managerUserResult.Data.Mobile=model.Mobile;
                        managerUserResult.Data.IsActive=model.IsActive;
                        _unitOfWork.Update(managerUserResult.Data);
                    }
                    else
                    {
                        return CommandResult<CreateOrEditManagerUserViewModel>.Failure(managerUserResult.Message, model);
                    } 
                }

                var result = await _unitOfWork.CommitChangesAsync(cancellationToken);
                if (result.IsSuccess)
                    return CommandResult<CreateOrEditManagerUserViewModel>.Success(result.Message, model);
                else
                    return CommandResult<CreateOrEditManagerUserViewModel>.Failure(result.Message, model);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<CreateOrEditManagerUserViewModel>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, new());
            }
        }

        public async Task<CommandResult<string>> RemoveManagerAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetAllAsIQueryable()
                    .Data.Where(x=>x.Id==id)
                    .Include(x=>x.Manager)
                    .SingleOrDefaultAsync(cancellationToken);
                if (user is null)
                    return CommandResult<string>.Failure(string.Format(UserMessages.NotFound,Captions.User),string.Empty);
               
                _unitOfWork.UserRepository.Delete(user);
                var removeResult = await _unitOfWork.CommitChangesAsync(cancellationToken);

                if (removeResult.IsSuccess)
                    return CommandResult<string>.Success(UserMessages.DataDeletedSuccessfully, user.FullName);
                else
                    return CommandResult<string>.Failure(removeResult.Message, user.FullName);
            }
            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<string>.Failure(OperationResultMessage.AnErrorHasOccurredInTheSoftware, string.Empty);
            }
        }
    }
}
