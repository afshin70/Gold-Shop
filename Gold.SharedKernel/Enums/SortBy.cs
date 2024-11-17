using Gold.Resources;
using System.ComponentModel.DataAnnotations;

namespace Gold.SharedKernel.Enums
{
    public enum SortBy:byte
    {
        /// <summary>
        /// جدیدترین
        /// </summary>
        [Display(Name = "Newest", ResourceType = typeof(Captions))]
        Newest = 1,

        /// <summary>
        /// پر بازدیدترین
        /// </summary>
        [Display(Name = "MostVisited", ResourceType = typeof(Captions))]
        MostVisited = 2,

        /// <summary>
        /// کمترین اجرت
        /// </summary>
        [Display(Name = "LowestWage", ResourceType = typeof(Captions))]
        LowestWage =3,

        /// <summary>
        /// بیشترین اجرت
        /// </summary>
        [Display(Name = "HighestWage", ResourceType = typeof(Captions))]
        HighestWage =4,

        /// <summary>
        /// گران‌ترین
        /// </summary>
        [Display(Name = "MostExpensive", ResourceType = typeof(Captions))]
        MostExpensive =5,

        /// <summary>
        /// ارزان‌ترین
        /// </summary>
        [Display(Name = "Cheapest", ResourceType = typeof(Captions))]
        Cheapest =6,

        /// <summary>
        /// بیش‌ترین تخفیف‌
        /// </summary>
        [Display(Name = "BiggestDiscount", ResourceType = typeof(Captions))]
        BiggestDiscount =7,

    }

}
