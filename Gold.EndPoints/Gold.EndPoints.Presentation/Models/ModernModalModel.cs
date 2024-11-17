namespace Gold.EndPoints.Presentation.Models
{
	public class ModernModalModel
	{
		public bool IsShowCloseIcon { get; set; } = true;
		//public bool IsShowCloseButtonInFooter { get; set; }
		//public string CloseButtonName { get; set; } = string.Empty;
		//public string CloseButtonClass { get; set; } = "btn btn-secondary";
		public string Id { get; set; } = string.Empty;
		public string ExtraClass { get; set; } = string.Empty;
		public string ShareUrlTitle { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public string BodyContent { get; set; } = string.Empty;
		//public string HeaderContent { get; set; } = string.Empty;
		//public string FooterContent { get; set; } = string.Empty;
		//public bool IsShowFooter { get; set; }
		public bool IsShowHeader { get; set; } = true;
		public string HeaderHtmlContent { get; set; }
		public bool IsHtmlHeader { get; set; }
		public bool IsShowBookmarkButtonInHeader { get; set; } = false;
		public string BookmarkButtonJsFn { get; set; } = string.Empty;
		public bool IsShowShareButtonInHeader { get; set; } = false;
		public string ShareUrl { get; set; } = string.Empty;
        //public ModalVertically Vertically { get; set; } = ModalVertically.Default;
        public ModernModalSize Size { get; set; } = ModernModalSize.Lg;
		//public string Modal_Dialog_Class { get; set; } = string.Empty;
		public bool DisableClickOutside { get; set; }
	}

    //public enum ModalVertically
    //{
    //	Default,
    //	Centered
    //}

    public enum ModernModalSize
	{
		Sm,
		Md,
		Lg,
		Xl,
	}
}
