using Gold.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.Domain.Enums
{
    public enum SettingType
    {
        Email=1,
        Sms=2,
        /// <summary>
        /// تنظیمات وام
        /// </summary>
        LoanSetting=3,
        ScriptSetting=4,

        /// <summary>
        /// ثبت سند
        /// </summary>
        [Display(Name = "MessageType_DocumentRegistration", ResourceType = typeof(Captions))]
        MessageType_DocumentRegistration = 5,
        /// <summary>
        /// یادآوری روز قبل از تاریخ قسط
        /// </summary>
        [Display(Name = "MessageType_ReminderOfTheDayBeforeTheInstallmentDate", ResourceType = typeof(Captions))]
        MessageType_ReminderOfTheDayBeforeTheInstallmentDate = 6,
        /// <summary>
        /// یادآوری 7 روز بعد از تاریخ قسط پرداخت نشده
        /// </summary>
        [Display(Name = "MessageType_Reminder7DaysAfterTheUnpaidInstallmentDate", ResourceType = typeof(Captions))]
        MessageType_Reminder7DaysAfterTheUnpaidInstallmentDate = 7,
        /// <summary>
        /// پرداخت با کسری
        /// </summary>
        [Display(Name = "MessageType_DeficitPayment", ResourceType = typeof(Captions))]
        MessageType_DeficitPayment = 8,
        /// <summary>
        /// پرداخت
        /// </summary>
        [Display(Name = "MessageType_Payment", ResourceType = typeof(Captions))]
        MessageType_Payment = 9,
        /// <summary>
        /// پرداخت با اضافه
        /// </summary>
        [Display(Name = "MessageType_OverPayment", ResourceType = typeof(Captions))]
        MessageType_OverPayment = 10,
        /// <summary>
        /// تسویه
        /// </summary>
        [Display(Name = "MessageType_SettleDocument", ResourceType = typeof(Captions))]
        MessageType_SettleDocument = 11,

        [Display(Name = "MessageType_HappyBirthday", ResourceType = typeof(Captions))]
        MessageType_HappyBirthday=12,

        /// <summary>
        /// خرید قسطی محصول
        /// </summary>
        [Display(Name = "SiteContent_InstallmentPurchaseOfProduct", ResourceType = typeof(Captions))]
        SiteContent_InstallmentPurchaseOfProduct = 13,
        [Display(Name = "SiteContent_ContactUs", ResourceType = typeof(Captions))]
        SiteContent_ContactUs =14,
        [Display(Name = "SiteContent_AboutUs", ResourceType = typeof(Captions))]
        SiteContent_AboutUs =15,
        /// <summary>
        /// لیست شعب
        /// </summary>
        [Display(Name = "SiteContent_BrancheList", ResourceType = typeof(Captions))]
        SiteContent_BrancheList = 16,
    }
}
