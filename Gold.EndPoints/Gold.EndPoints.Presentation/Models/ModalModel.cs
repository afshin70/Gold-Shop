using Gold.Resources;

namespace Gold.EndPoints.Presentation.Models
{
    public class ModalModel
    {
        public string ExtraClass { get; set; } = string.Empty;
        public bool IsShowCloseIcon { get; set; }
        public bool IsShowCloseButtonInFooter { get; set; } 
        public string CloseButtonName { get; set; }=string.Empty;
        public string CloseButtonClass { get; set; }= "btn btn-secondary";
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string BodyContent { get; set; } = string.Empty;
        //public string HeaderContent { get; set; } = string.Empty;
        public bool IsHtmlHeader { get; set; }
        public bool HeaderHtmlContent { get; set; }
        public string FooterContent { get; set; } = string.Empty;
        public bool IsShowFooter { get; set; }
        public bool IsShowHeader { get; set; } = true;
        public ModalVertically Vertically { get; set; } = ModalVertically.Default;
        public ModalSize Size { get; set; } = ModalSize.Small;
        public string Modal_Dialog_Class { get; set; } = string.Empty;
        public bool DisableClickOutside { get; set; }

    }

    public enum ModalVertically
    {
        Default,
        Centered
    }

    public enum ModalSize
    {
       Small,
       Larg,
       None
    }
}
