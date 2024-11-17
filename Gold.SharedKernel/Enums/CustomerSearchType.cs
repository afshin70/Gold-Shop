using System.ComponentModel.DataAnnotations;
using Gold.Resources;

namespace Gold.SharedKernel.Enums
{
    public enum CustomerSearchType : byte
    {
        [Display(Name = "PhoneNumber", ResourceType = typeof(Captions))]
        ByPhoneNumber = 1,
        [Display(Name = "BankCardNumber", ResourceType = typeof(Captions))]
        ByBankCardNumber=2
    }
}
