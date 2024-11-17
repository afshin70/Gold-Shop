using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Gold.Resources;
using System.Security.AccessControl;
using System.Xml.Linq;

namespace Gold.SharedKernel.Enums
{
	public enum UserType : byte
    {
        [Display(Name = "Customer", ResourceType = typeof(Captions))]
        Customer = 1,
        [Display(Name = "Seller", ResourceType = typeof(Captions))]
        Seller = 2,
        [Display(Name = "Admin", ResourceType = typeof(Captions))]
        Admin = 3,
        [Display(Name = "Manager", ResourceType = typeof(Captions))]
        Manager = 4
    }

    public enum CustomerMessageType : byte
    {
        Custom=1,
        /// <summary>
        /// ثبت سند
        /// </summary>
        RegisterDocument=2,
        /// <summary>
        /// تسویه سند
        /// </summary>
        SettleDocument=3,
        /// <summary>
        /// یادآوری پرداخت
        /// </summary>
        PaymentReminder=4,
        /// <summary>
        /// یادآوری پرداخت 7 روز قبل از
        /// </summary>
        Reminder7Days=5,
        /// <summary>
        /// تبریک تولد
        /// </summary>
        HappyBirthday=6,
        /// <summary>
        /// پرداخت با اضافه
        /// </summary>
        OverPayment=7,
        /// <summary>
        /// پرداخت با کسری
        /// </summary>
        DeficitPayment=8,
        /// <summary>
        /// پرداخت کامل
        /// </summary>
        FullPayment=9
    }
}
