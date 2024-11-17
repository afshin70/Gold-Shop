using Gold.Domain.Contract.DTOs;
using Gold.Domain.Contract.IRepositories;
using Gold.Domain.Contract.IRepositories.Logging;
using Gold.Domain.Entities;
using Gold.Domain.Entities.Base;
using Gold.Infrastracture.EFCoreContext;
using Gold.Infrastracture.Repositories.UOW;
using Gold.Resources;
using Gold.SharedKernel.Contract;
using Gold.SharedKernel.DTO.OperationResult;
using Gold.SharedKernel.ExtentionMethods;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.Infrastracture.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationContext _context;
        private readonly ILogManager _logManager;
        private IDbContextTransaction _transaction;

        public UnitOfWork(ApplicationContext context, ILogManager logManager)
        {
            _context = context;
            _logManager = logManager;
        }
        private UserRepository _userRepository;
        public IUserRepository UserRepository => _userRepository is null ? new UserRepository(_context, _logManager) : _userRepository;

        private CustomerRepository _customerRepository;
        public ICustomerRepository CustomerRepository => _customerRepository is null ? new CustomerRepository(_context, _logManager) : _customerRepository;

        private CityRepository _cityRepository;
        public ICityRepository CityRepository => _cityRepository is null ? new CityRepository(_context, _logManager) : _cityRepository;

        private EssentialTelRepository _essentialNumberRepository;
        public IEssentialTelRepository EssentialNumberRepository => _essentialNumberRepository is null ? new EssentialTelRepository(_context, _logManager) : _essentialNumberRepository;
        private SettingRepository _settingRepository;
        public ISettingRepository SettingRepository => _settingRepository is null ? new SettingRepository(_context, _logManager) : _settingRepository;

        private SocialNetworkRepository _socialNetworkRepository;
        public ISocialNetworkRepository SocialNetworkRepository => _socialNetworkRepository is null ? new SocialNetworkRepository(_context, _logManager) : _socialNetworkRepository;

        private GalleryRepository _galleryRepository;
        public IGalleryRepository GalleryRepository => _galleryRepository is null ? new GalleryRepository(_context, _logManager) : _galleryRepository;

        private SellerRepository _sellerRepository;
        public ISellerRepository SellerRepository => _sellerRepository is null ? new SellerRepository(_context, _logManager) : _sellerRepository;

        private DocumentRepository _documentRepository;
        public IDocumentRepository DocumentRepository => _documentRepository is null ? new DocumentRepository(_context, _logManager) : _documentRepository;

        private CollateralRepository _collateralRepository;
        public ICollateralRepository CollateralRepository => _collateralRepository is null ? new CollateralRepository(_context, _logManager) : _collateralRepository;

        private CollateralTypeRepository _collateralTypeRepository;
        public ICollateralTypeRepository CollateralTypeRepository => _collateralTypeRepository is null ? new CollateralTypeRepository(_context, _logManager) : _collateralTypeRepository;

        private InstallmentRepository _installmentRepository;
        public IInstallmentRepository InstallmentRepository => _installmentRepository is null ? new InstallmentRepository(_context, _logManager) : _installmentRepository;

        private PaymentRepository _paymentRepository;
        public IPaymentRepository PaymentRepository => _paymentRepository is null ? new PaymentRepository(_context, _logManager) : _paymentRepository;

        private CustomerMessageRepository _customerMessageRepository;
        public ICustomerMessageRepository CustomerMessageRepository => _customerMessageRepository is null ? new CustomerMessageRepository(_context, _logManager) : _customerMessageRepository;

        private CustomerPaymentRepository _customerPaymentRepository;
        public ICustomerPaymentRepository CustomerPaymentRepository => _customerPaymentRepository is null ? new CustomerPaymentRepository(_context, _logManager) : _customerPaymentRepository;

        private ManagerRepository _managerRepository;
        public IManagerRepository ManagerRepository => _managerRepository is null ? new ManagerRepository(_context, _logManager) : _managerRepository;

        private PermissionAccessRepository _permissionAccessRepository;
        public IPermissionAccessRepository PermissionAccessRepository => _permissionAccessRepository is null ? new PermissionAccessRepository(_context, _logManager) : _permissionAccessRepository;

        private PermissionRepository _permissionRepository;
        public IPermissionRepository PermissionRepository => _permissionRepository is null ? new PermissionRepository(_context, _logManager) : _permissionRepository;

        private AdminMenuGroupsRepository _adminMenuGroupsRepository;
        public IAdminMenuGroupsRepository AdminMenuGroupsRepository => _adminMenuGroupsRepository is null ? new AdminMenuGroupsRepository(_context, _logManager) : _adminMenuGroupsRepository;

        private AdminMenuRepository _adminMenuRepository;
        public IAdminMenuRepository AdminMenuRepository => _adminMenuRepository is null ? new AdminMenuRepository(_context, _logManager) : _adminMenuRepository;

        private EditInformationRequestRepository _editInformationRequestRepository;
        public IEditInformationRequestRepository EditInformationRequestRepository => _editInformationRequestRepository is null ? new EditInformationRequestRepository(_context, _logManager) : _editInformationRequestRepository;

        private AdminActivityRepository _adminActivityRepository;
        public IAdminActivityRepository AdminActivityRepository => _adminActivityRepository is null ? new AdminActivityRepository(_context, _logManager) : _adminActivityRepository;


        private SendSmsTempRepository _passwordRecoveryRepository;
        public ISendSmsTempRepository SendSmsTempRepository => _passwordRecoveryRepository is null ? new SendSmsTempRepository(_context, _logManager) : _passwordRecoveryRepository;

        private BankCardNoRepository _bankCardNoRepository;
        public IBankCardNoRepository BankCardNoRepository => _bankCardNoRepository is null ? new BankCardNoRepository(_context, _logManager) : _bankCardNoRepository;

        private ProfileImageRepository _profileImageRepository;
        public IProfileImageRepository ProfileImageRepository => _profileImageRepository is null ? new ProfileImageRepository(_context, _logManager) : _profileImageRepository;

        private GoldPriceRepository _goldPriceRepository;
        public IGoldPriceRepository GoldPriceRepository => _goldPriceRepository is null ? new GoldPriceRepository(_context, _logManager) : _goldPriceRepository;


        private FAQCategoryRepository _fAQCategoryRepository;
        public IFAQCategoryRepository FAQCategoryRepository=>_fAQCategoryRepository is null?new FAQCategoryRepository(_context,_logManager):_fAQCategoryRepository;
        
        private FAQRepository _fAQRepository;
        public IFAQRepository FAQRepository=>_fAQRepository is null?new FAQRepository(_context,_logManager):_fAQRepository;
        
        private ArticleRepository _articleRepository;
        public IArticleRepository ArticleRepository => _articleRepository is null?new ArticleRepository(_context,_logManager):_articleRepository;

        private ProductRepository _productRepository;
        public IProductRepository ProductRepository => _productRepository is null?new ProductRepository(_context,_logManager): _productRepository;

        private ProductGalleryRepository _productGalleryRepository;
        public IProductGalleryRepository ProductGalleryRepository => _productGalleryRepository is null ? new ProductGalleryRepository(_context, _logManager) : _productGalleryRepository;

        private CategoryRepository _categoryRepository;
        public ICategoryRepository CategoryRepository => _categoryRepository is null ? new CategoryRepository(_context, _logManager) : _categoryRepository;


        private ProductCategoryRepository _productCategoryRepository;
        public IProductCategoryRepository ProductCategoryRepository => _productCategoryRepository is null ? new ProductCategoryRepository(_context, _logManager) : _productCategoryRepository;
        
        private FavoritProductRepository _favoritProductRepository;
        public IFavoritProductRepository FavoritProductRepository => _favoritProductRepository is null ? new FavoritProductRepository(_context, _logManager) : _favoritProductRepository;
        
        private ContactUsRepository _contactUsRepository;
        public IContactUsRepository ContactUsRepository => _contactUsRepository is null ? new ContactUsRepository(_context, _logManager) : _contactUsRepository;

        public void RejectChanges()
        {
            foreach (var entry in _context.ChangeTracker.Entries()
                  .Where(e => e.State != EntityState.Unchanged))
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                }
            }
        }
        public CommandResult<int> CommitChanges()
        {
            try
            {
                int result = _context.SaveChanges();
                return CommandResult<int>.Success(DBOperationMessages.AllDataChangesAreSaved, result);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult<int>.FailureInSaveData();
            }
        }
        public async Task<CommandResult<int>> CommitChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                int result = await _context.SaveChangesAsync(cancellationToken);
                return CommandResult<int>.Success(DBOperationMessages.AllDataChangesAreSaved, result);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null)
                    if (((SqlException)ex.InnerException).Number == 547)
                        return CommandResult<int>.FailureInDeleteDataUseElsewhere();
                return CommandResult<int>.FailureInOperation();
            }

            catch (Exception ex)
            {
                await _logManager.RaiseLogAsync(ex, cancellationToken);
                return CommandResult<int>.FailureInOperation();
            }
        }

        public CommandResult Add(IEntity entity)
        {
            try
            {
                _context.Add(entity);
                return CommandResult.Success(DBOperationMessages.DataAddedCorrectly);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult.Failure(DBOperationMessages.AnErrorOccurredWhileAddingData);
            }
        }

        public async Task<CommandResult> AddAsync(IEntity entity, CancellationToken cancellationToken)
        {
            try
            {
                await _context.AddAsync(entity, cancellationToken);
                return CommandResult.Success(DBOperationMessages.DataAddedCorrectly);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult.Failure(DBOperationMessages.AnErrorOccurredWhileAddingData);
            }
        }


        public CommandResult Update(IEntity entity)
        {
            try
            {
                _context.Update(entity);
                return CommandResult.Success(DBOperationMessages.DataEditedCorrectly);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult.Failure(DBOperationMessages.AnErrorOccurredWhileEditingData);
            }
        }


        public CommandResult Delete(IEntity entity)
        {
            try
            {
                _context.Remove(entity);
                return CommandResult.Success(DBOperationMessages.DataDeletedCorrectly);
            }
            catch (Exception ex)
            {
                _logManager.RaiseLog(ex);
                return CommandResult.Failure(DBOperationMessages.AnErrorOccurredWhileRemovingData);
            }
        }








        public void Dispose()
        {
            _context.Dispose();
        }
        public async void DisposeAsync()
        {
            await _context.DisposeAsync();
        }
        public IDbContextTransaction BeginTransaction(System.Data.IsolationLevel isolationLevel)
        {
            _transaction = _context.Database.BeginTransaction(isolationLevel);
            return _transaction;
        }
        public IDbContextTransaction BeginTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
            return _transaction;
        }


        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        {
            return await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(System.Data.IsolationLevel isolationLevel, CancellationToken cancellationToken)
        {
            return await _context.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
        }
        public CommandResult Rollback()
        {
            try
            {
                if (_transaction is not null)
                {
                    _transaction.Rollback();
                }

                return CommandResult.Success(DBOperationMessages.AllDataChangesAreRollBacked);
            }
            catch (Exception)
            {
                //throw new NullReferenceException("The transaction instance is null. Please call BeginTransaction() or BeginTransactionAsync() before use  Rollback() or RollbackAsync().",ex);
                return CommandResult.Failure(DBOperationMessages.AnErrorOccurredWhileRollBackingData);

            }
        }
        public async Task<CommandResult> RollbackAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (_transaction is not null)
                    await _transaction.RollbackAsync(cancellationToken);
                return CommandResult.Success(DBOperationMessages.AllDataChangesAreRollBacked);
            }
            catch (Exception)
            {
                // throw new NullReferenceException("The transaction instance is null. Please call BeginTransaction() or BeginTransactionAsync() before use  Rollback() or RollbackAsync().",ex);
                return CommandResult.Failure(DBOperationMessages.AnErrorOccurredWhileRollBackingData);
            }
        }
        public async Task<CommandResult> CommitTransactionAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (_transaction is not null)
                    await _transaction.CommitAsync(cancellationToken);
                return CommandResult.Success(DBOperationMessages.AllDataChangesAreSaved);
            }
            catch (Exception)
            {
                return CommandResult.Failure(DBOperationMessages.AnErrorOccurredWhileSavingData);
            }
        }
        public CommandResult CommitTransaction()
        {
            try
            {
                if (_transaction is not null)
                    _transaction.Commit();
                return CommandResult.Success(DBOperationMessages.AllDataChangesAreSaved);
            }
            catch (Exception)
            {
                return CommandResult.Failure(DBOperationMessages.AnErrorOccurredWhileSavingData);
            }
        }
    }

}
