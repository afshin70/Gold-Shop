using System.ComponentModel.DataAnnotations;
using Gold.Resources;

namespace Gold.SharedKernel.Enums
{
	public enum ConfirmStatusType : byte
    {
        [Display(Name = "Pending", ResourceType = typeof(Captions))]
        Pending = 1,
        [Display(Name = "Rejection", ResourceType = typeof(Captions))]
        Rejection = 2,
        [Display(Name = "Confirmation", ResourceType = typeof(Captions))]
        Confirmation = 3
    }
}
