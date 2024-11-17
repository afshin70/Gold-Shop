using System.ComponentModel.DataAnnotations;
using Gold.Resources;

namespace Gold.SharedKernel.Enums
{
    public enum NationalityType : byte
    {
        [Display(Name = "Iranian", ResourceType = typeof(Captions))]
        Iranian = 1,
        [Display(Name = "IranianWithOutNationalCode", ResourceType = typeof(Captions))]
        IranianWithOutNationalCode = 2,
        [Display(Name = "Foreigners", ResourceType = typeof(Captions))]
        Foreigners = 3
    } 
}
