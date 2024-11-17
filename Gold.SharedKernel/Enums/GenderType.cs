using System.ComponentModel.DataAnnotations;
using Gold.Resources;

namespace Gold.SharedKernel.Enums
{
    public enum GenderType : byte
    {
        [Display(Name = "Men", ResourceType = typeof(Captions))]
        Men = 1,
        [Display(Name = "Women", ResourceType = typeof(Captions))]
        Women = 2,
    }
}
