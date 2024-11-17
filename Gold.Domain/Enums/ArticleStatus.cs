using Gold.Resources;
using System.ComponentModel.DataAnnotations;

namespace Gold.Domain.Enums
{
    public enum ArticleStatus:byte
    {
        [Display(Name = "Active", ResourceType = typeof(Captions))]
        Active=1,
        [Display(Name = "DeActive", ResourceType = typeof(Captions))]
        Deactive=2
    } 
}
