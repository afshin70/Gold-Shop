using System.ComponentModel.DataAnnotations;
using Gold.Resources;

namespace Gold.SharedKernel.Enums
{
    public enum AdminActivityType : byte
    {
        [Display(Name = "Insert", ResourceType = typeof(Captions))]
        Insert = 1,

        [Display(Name = "Edit", ResourceType = typeof(Captions))]
        Edit = 2,

        [Display(Name = "Delete", ResourceType = typeof(Captions))]
        Delete = 3,

        //[Display(Name = "Confirm", ResourceType = typeof(Captions))]
        //Confirm = 4,

        //[Display(Name = "Cancel", ResourceType = typeof(Captions))]
        //Cancel = 5,

        //[Display(Name = "Settle", ResourceType = typeof(Captions))]
        //Settle = 6,
    }
}
