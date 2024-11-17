using Gold.Domain.Contract.IRepositories;
using Gold.Domain.Contract.IRepositories.Base;
using Gold.Domain.Contract.IRepositories.Logging;
using Gold.Domain.Entities;
using Gold.Domain.Entities.Base;
using Gold.SharedKernel.DTO.OperationResult;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.Infrastracture.Repositories.UOW
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        ICustomerRepository CustomerRepository { get; }
        ICityRepository CityRepository { get; }
        IEssentialTelRepository EssentialNumberRepository { get; }
        ISettingRepository SettingRepository { get; }
        ISocialNetworkRepository SocialNetworkRepository { get; }
        IGalleryRepository GalleryRepository { get; }
        ISellerRepository SellerRepository { get; }
        IDocumentRepository DocumentRepository { get; }
        ICollateralRepository CollateralRepository { get; }
        ICollateralTypeRepository CollateralTypeRepository { get; }
        IInstallmentRepository InstallmentRepository { get; }
        ICustomerMessageRepository CustomerMessageRepository { get; }
        ICustomerPaymentRepository CustomerPaymentRepository { get; }
        IPaymentRepository PaymentRepository { get; }

        IManagerRepository ManagerRepository { get; }
        IPermissionAccessRepository PermissionAccessRepository { get; }
        IPermissionRepository PermissionRepository { get; }
        IAdminMenuGroupsRepository AdminMenuGroupsRepository { get; }
        IAdminMenuRepository AdminMenuRepository { get; }
        IAdminActivityRepository AdminActivityRepository { get; }
        IEditInformationRequestRepository EditInformationRequestRepository { get; }
        ISendSmsTempRepository SendSmsTempRepository { get; }
        IGoldPriceRepository GoldPriceRepository { get; }

        IBankCardNoRepository BankCardNoRepository { get; }
        IProfileImageRepository ProfileImageRepository { get; }
        IFAQCategoryRepository FAQCategoryRepository { get; }
        IFAQRepository FAQRepository { get; }
        IArticleRepository ArticleRepository { get; }
        IProductRepository ProductRepository { get; }
        IProductGalleryRepository ProductGalleryRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IProductCategoryRepository ProductCategoryRepository { get; }
        IFavoritProductRepository FavoritProductRepository { get; }
        IContactUsRepository ContactUsRepository { get; }

        CommandResult Add(IEntity entity);
        Task<CommandResult> AddAsync(IEntity entity, CancellationToken cancellationToken);

        IDbContextTransaction BeginTransaction();
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
        CommandResult<int> CommitChanges();
        Task<CommandResult<int>> CommitChangesAsync(CancellationToken cancellationToken);
        CommandResult Delete(IEntity entity);

        CommandResult CommitTransaction();
        Task<CommandResult> CommitTransactionAsync(CancellationToken cancellationToken);
        void Dispose();
        void DisposeAsync();
        void RejectChanges();
        CommandResult Update(IEntity entity);
        CommandResult Rollback();
        Task<CommandResult> RollbackAsync(CancellationToken cancellationToken);
		IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel);
		Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken);
	}
}
