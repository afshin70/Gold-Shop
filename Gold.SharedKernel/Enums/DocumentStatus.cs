using System.ComponentModel.DataAnnotations;
using Gold.Resources;

namespace Gold.SharedKernel.Enums
{
    public enum DocumentStatus : byte
    {
        /// <summary>
        /// فعال
        /// </summary>
        [Display(Name = "Active", ResourceType = typeof(Captions))]
        Active = 1,

        /// <summary>
        /// تسویه شده
        /// </summary>
        [Display(Name = "Settled", ResourceType = typeof(Captions))]
        Settled = 2,

        /// <summary>
        /// حذف شده
        /// </summary>
        [Display(Name = "Deleted", ResourceType = typeof(Captions))]
        Deleted = 3
    }
}
