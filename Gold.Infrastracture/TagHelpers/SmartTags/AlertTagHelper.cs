using Gold.Infrastracture.TagHelpers.Alert;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.Infrastracture.TagHelpers.SmartTags
{
    [HtmlTargetElement("smart-alert")]
    public class AlertTagHelper : TagHelper
    {
        //private const string ForAttributeName = "asp-for";
        //private const string SkinAttributeName = "asp-skin";
        //private const string SizeAttributeName = "asp-col-size";
        private const string VisibleAttributeName = "asp-visible";
        private const string TitleAttributeName = "asp-title";
        private const string TextAttributeName = "asp-text";
        private const string ListAttributeName = "asp-list";
        private const string ShowBigSizeAttributeName = "asp-show-big-size";
        private const string ButtonTextAttributeName = "asp-button-text";
        private const string CloseButtonAttributeName = "asp-close-button";
        private const string AlertTypeAttributeName = "asp-type";
        private const string FaIconAttributeName = "asp-fa-icon";
        private const string ButtonJsOnClickAttributeName = "asp-onclick";
        private const string ButtonUrlAttributeName = "asp-url";
        private const string ViewDataAttributeName = "asp-view-data";
        private const string BindViewDataAttributeName = "asp-bind-view-data";

        //[HtmlAttributeName(ForAttributeName)]
        //public string For { get; set; }

        //[HtmlAttributeName(SizeAttributeName)]
        //public int ColSize { get; set; } = 4;

        [HtmlAttributeName(VisibleAttributeName)]
        public bool Visible { get; set; } = true;

        [HtmlAttributeName(TitleAttributeName)]
        public string Title { get; set; }

        [HtmlAttributeName(TextAttributeName)]
        public string Text { get; set; }

        [HtmlAttributeName(ListAttributeName)]
        public string[] List { get; set; }

        [HtmlAttributeName(ShowBigSizeAttributeName)]
        public bool ShowBigSize { get; set; }

        [HtmlAttributeName(CloseButtonAttributeName)]
        public bool CloseButton { get; set; }

        [HtmlAttributeName(ButtonTextAttributeName)]
        public string ButtonText { get; set; }

        [HtmlAttributeName(ButtonJsOnClickAttributeName)]
        public string ButtonJsOnClick { get; set; }

        [HtmlAttributeName(ButtonUrlAttributeName)]
        public string ButtonUrl { get; set; }

        [HtmlAttributeName(AlertTypeAttributeName)]
        public AlertType AlertType { get; set; }

        [HtmlAttributeName(FaIconAttributeName)]
        public string FaIcon { get; set; }

        [HtmlAttributeName(ViewDataAttributeName)]
        public dynamic ViewData { get; set; }

        [HtmlAttributeName(BindViewDataAttributeName)]
        public bool BindViewData { get; set; }

        /// <summary>
        /// Process
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="output">Output</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            //output.SuppressOutput();

            if (BindViewData && ViewData != null)
            {
                var viewDataModel = (AlertModel)ViewData;
                Visible = true;
                Title = viewDataModel.Title;
                Text = viewDataModel.Text;
                List = viewDataModel.List;
                ShowBigSize = viewDataModel.ShowBigSize;
                CloseButton = viewDataModel.CloseButton.GetValueOrDefault(true);
                ButtonText = viewDataModel.ButtonText;
                ButtonJsOnClick = viewDataModel.ButtonJsOnClick;
                ButtonUrl = viewDataModel.ButtonUrl;
                AlertType = (AlertType)Enum.ToObject(typeof(AlertType), (int)viewDataModel.Type);
                FaIcon = viewDataModel.IconCssClass;
            }
            else if (BindViewData && ViewData == null)
            {
                Visible = false;
            }

            if (!Visible || !(!string.IsNullOrEmpty(Text) || (List?.Any() ?? false)))
            {
                output.TagName = "";
                output.Content.SetHtmlContent("");
                return;
            }

            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;

            string closeButton = null;
            if (CloseButton)
            {
                closeButton = "<button class='close' aria-hidden='true' type='button' data-dismiss='alert'>×</button>";
            }

            string messageColorCssClass = null;
            switch (AlertType)
            {
                case AlertType.Success:
                    messageColorCssClass = "alert-success";
                    break;
                case AlertType.Danger:
                    messageColorCssClass = "alert-danger";
                    break;
                case AlertType.Warning:
                    messageColorCssClass = "alert-warning";
                    break;
                case AlertType.Info:
                    messageColorCssClass = "alert-info";
                    break;
                default:
                    messageColorCssClass = "alert-info";
                    break;
            }

            string messageIconCssClass = FaIcon;
            if (string.IsNullOrEmpty(messageIconCssClass))
            {
                switch (AlertType)
                {
                    case AlertType.Success:
                        messageIconCssClass = "fa-check-circle";
                        break;
                    case AlertType.Danger:
                        messageIconCssClass = "fa-times-circle";
                        break;
                    case AlertType.Warning:
                        messageIconCssClass = "fa-warning";
                        break;
                    case AlertType.Info:
                        messageIconCssClass = "fa-info-circle";
                        break;
                    default:
                        messageIconCssClass = "fa-info-circle";
                        break;
                }
            }

            if (ShowBigSize || !string.IsNullOrEmpty(ButtonText) || (List?.Any() ?? false))
            {
                var textBody = new StringBuilder();
                if (List?.Any() ?? false)
                {
                    textBody.Append("<p><ul>").Append(Text);
                    foreach (var item in List)
                    {
                        textBody.Append("<li>").Append(item).Append("</li>");
                    }
                    textBody.Append("</ul></p>");
                }
                else
                {
                    textBody.Append("<p>").Append(Text).Append("</p>");
                }

                var btnBody = new StringBuilder();
                if (!string.IsNullOrEmpty(ButtonText))
                {
                    btnBody.Append("<br />")
                        .Append("<p class='text-left'>");
                    if (!string.IsNullOrEmpty(ButtonJsOnClick))
                    {
                        btnBody.Append("<span class='btn btn-default cursor-pointer' onclick='").Append(ButtonJsOnClick).Append("'>")
                            .Append(ButtonText).Append("</span>");
                    }
                    else if (ButtonUrl.StartsWith("#"))
                    {
                        btnBody.Append("<a class='btn btn-default' data-toggle='modal' data-target='").Append(ButtonUrl).Append("'>")
                            .Append(ButtonText)
                            .Append("</a>");
                    }
                    else
                    {
                        btnBody.Append("<a class='btn btn-default' href='").Append(ButtonUrl).Append("'>")
                            .Append(ButtonText)
                            .Append("</a>");
                    }
                    btnBody.Append("</p>");
                }

                //output.Content.SetHtmlContent($@"<div class='alert {messageColorCssClass} alert-dismissable mb30 pr15 alert-border-right'>
                //                                    {closeButton}
                //                                    <h3 class='mt5'>
                //                                        <i class='fa {messageIconCssClass} pl10'></i>
                //                                        {(string.IsNullOrEmpty(Title) ? "توجه!" : Title)}
                //                                    </h3>
                //                                    {textBody.ToString()}
                //                                    {btnBody.ToString()}</div>");
                output.Content.SetHtmlContent($@"<div class='alert {messageColorCssClass} alert-dismissable mb30 pr15 alert-styled-left'>
                                                    {closeButton}
                                                    <h3 class='mt5'>
                                                        {(string.IsNullOrEmpty(Title) ? "توجه!" : Title)}
                                                    </h3>
                                                    {textBody.ToString()}
                                                    {btnBody.ToString()}</div>");
            }
            else
            {
                //output.Content.SetHtmlContent($@"<div class='alert {messageColorCssClass} alert-border-right alert-dismissable'>
                //    {closeButton}<i class='fa {messageIconCssClass} pr10 pl5'></i>
                //    {Text}
                //    </div>");
                output.Content.SetHtmlContent($@"<div class='alert {messageColorCssClass} alert-styled-left'>
                    {closeButton}
                    {Text}
                    </div>");
            }

            base.Process(context, output);
        }
    }
}
