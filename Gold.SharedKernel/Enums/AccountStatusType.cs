using System.ComponentModel.DataAnnotations;
using Gold.Resources;

namespace Gold.SharedKernel.Enums
{
    public enum AccountStatusType : byte
    {
        [Display(Name = "GoodPay", ResourceType = typeof(Captions))]
        GoodPay = 1,
        [Display(Name = "DeadBeat", ResourceType = typeof(Captions))]
        DeadBeat = 2,
    }
}
