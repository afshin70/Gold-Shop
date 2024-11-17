using Gold.Domain.Enums;
using Gold.Resources;
using System.ComponentModel.DataAnnotations;

namespace Gold.Domain.Enums
{
    public enum ProductStatus:byte
    {
        [Display(Name = "Active", ResourceType = typeof(Captions))]
        Active = 1,
        [Display(Name = "Sold", ResourceType = typeof(Captions))]
        Sold = 2,
    }
}
