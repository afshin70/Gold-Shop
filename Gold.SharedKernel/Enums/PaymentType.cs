using System.ComponentModel.DataAnnotations;
using Gold.Resources;

namespace Gold.SharedKernel.Enums
{
    public enum PaymentType : byte
    {
        /// <summary>
        /// پرداخت کامل
        /// </summary>
        [Display(Name = "FullPayment", ResourceType = typeof(Captions))]
        FullPayment = 1,

        /// <summary>
        /// پرداخت با اضافه
        /// </summary>
        [Display(Name = "OverPayment", ResourceType = typeof(Captions))]
        OverPayment = 2,

        /// <summary>
        /// پرداخت با کسری
        /// </summary>
        [Display(Name = "DeficitPayment", ResourceType = typeof(Captions))]
        DeficitPayment = 3
    }
}
