using Gold.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Gold.SharedKernel.Enums
{
    /// <summary>
    /// permission of manager user type
    /// </summary>
    public enum ManagerPermission:byte
    {
		/// <summary>
		/// گزارش پرداخنت
		/// </summary>
		[Display(Name = "PaymentReport", ResourceType = typeof(Captions))]
		PaymentReport = 1,
		/// <summary>
		/// گزارش فعالیت مدیران
		/// </summary>
		[Display(Name = "ManagersActivityReport", ResourceType = typeof(Captions))]
		ManagersActivityReport = 2,
		/// <summary>
		/// سند جدید
		/// </summary>
		[Display(Name = "NewDocument", ResourceType = typeof(Captions))]
		NewDocument = 3,
		/// <summary>
		/// لیست اسناد
		/// </summary>
		[Display(Name = "DocumentsList", ResourceType = typeof(Captions))]
		DocumentsList = 4,
		/// <summary>
		/// پرداخت های در انتظار تایید
		/// </summary>
		[Display(Name = "PaymentsInPendingConfirmation", ResourceType = typeof(Captions))]
		PaymentsInPendingConfirmation = 5,
		/// <summary>
		/// درخواست ویرایش اطلاعات
		/// </summary>
		[Display(Name = "RequestsToEditInformation", ResourceType = typeof(Captions))]
		RequestsToEditInformation = 6,
		/// <summary>
		/// مشتری ها
		/// </summary>
		[Display(Name = "Customers", ResourceType = typeof(Captions))]
		Customers = 7,
		/// <summary>
		/// گالری ها
		/// </summary>
		[Display(Name = "Galleries", ResourceType = typeof(Captions))]
		Galleries = 8,
		/// <summary>
		/// مدیران
		/// </summary>
		[Display(Name = "Managers", ResourceType = typeof(Captions))]
		Managers = 9,
		/// <summary>
		/// سطح دسترسی
		/// </summary>
		[Display(Name = "AccessLevels", ResourceType = typeof(Captions))]
		AccessLevels = 10,
		/// <summary>
		/// تنظیمات
		/// </summary>
		[Display(Name = "Setting", ResourceType = typeof(Captions))]
		Setting = 11,
        /// <summary>
        /// شبکه های اجتماعی
        /// </summary>
        [Display(Name = "SocialNetworks", ResourceType = typeof(Captions))]
        SocialNetwork=12,
		/// <summary>
		///   ارسال پیام
		/// </summary>
        [Display(Name = "SendMessage", ResourceType = typeof(Captions))]
        SendMessage =13,
        [Display(Name = "GoldRate", ResourceType = typeof(Captions))]
        GoldPrice =14,
        /// <summary>
        /// مطالب سایت
        /// </summary>
        [Display(Name = "SiteContent", ResourceType = typeof(Captions))]
        SiteContent =15,
		/// <summary>
		/// سوالات متداول
		/// </summary>
        [Display(Name = "ManageFAQ", ResourceType = typeof(Captions))]
        ManageFAQ = 16,
        /// <summary>
        /// مقالات آموزشی
        /// </summary>
        [Display(Name = "LearningArticle", ResourceType = typeof(Captions))]
        Article = 17,
        /// <summary>
        /// مدیریت محصولات
        /// </summary>
        [Display(Name = "ProductManagement", ResourceType = typeof(Captions))]
        ProductManagement = 18,
		/// <summary>
		/// مدیریت پیامهای تماس با ما
		/// </summary>
        [Display(Name = "ContactUsMessagesManagement", ResourceType = typeof(Captions))]
        ContactUsMessagesManagement =19,
    }
}
