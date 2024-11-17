using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Gold.Infrastracture.TagHelpers.SmartTags
{
    [HtmlTargetElement("smart-textarea", Attributes = ForAttributeName)]
    public class SmartTextAreaTagHelper : TextAreaTagHelper
    {
        private const string ForAttributeName = "asp-for";
        private const string DisplayNameAttributeName = "asp-displayname";
        private const string SizeAttributeName = "asp-col-size";
        private const string CSSClassAttributeName = "class";
        private const string VisibleAttributeName = "asp-visible";
        private const string PlaceHolderAttributeName = "asp-placeholder";
        private const string PlaceHolderTextAttributeName = "asp-placeholder-text";
        private const string ReadonlyAttributeName = "asp-readonly";
        private const string RowsName = "asp-rows";


        /// <summary>
        /// An expression to be evaluated against the current model
        /// </summary>
        [HtmlAttributeName(DisplayNameAttributeName)]
        public string DisplayName { get; set; }

        [HtmlAttributeName(CSSClassAttributeName)]
        public string CSSClass { get; set; }

        [HtmlAttributeName(SizeAttributeName)]
        public int ColSize { get; set; } = 4;
        [HtmlAttributeName(VisibleAttributeName)]
        public bool Visible { get; set; } = true;

        [HtmlAttributeName(PlaceHolderAttributeName)]
        public bool Placeholder { get; set; } = true;

        [HtmlAttributeName(PlaceHolderTextAttributeName)]
        public string PlaceholderText { get; set; }

        [HtmlAttributeName(ReadonlyAttributeName)]
        public bool Readonly { get; set; }

        [HtmlAttributeName(RowsName)]
        public int Rows { get; set; }
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="generator">HTML generator</param>
        public SmartTextAreaTagHelper(IHtmlGenerator generator) : base(generator)
        {
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="output">Output</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!Visible)
            {
                output.TagName = "";
                output.Content.SetHtmlContent("");
                return;
            }

            if (Placeholder)
            {
                if (string.IsNullOrEmpty(PlaceholderText))
                {
                    output.Attributes.Add("placeholder", For.Metadata.DisplayName);
                }
                else
                {
                    output.Attributes.Add("placeholder", PlaceholderText);
                }
            }

            string colSize = SmartMainHelper.GetColSize(ColSize);

            if (!string.IsNullOrEmpty(CSSClass))
            {
                colSize = $"{colSize} {CSSClass}";
            }
            output.SuppressOutput();
            output.TagName = "textarea";
            output.TagMode = TagMode.StartTagAndEndTag;

            if (string.IsNullOrEmpty(DisplayName))
            {
                DisplayName = For.Metadata.DisplayName;
            }
            if (Readonly)
            {
                output.Attributes.Add("readonly", true);
            }

            if (!string.IsNullOrEmpty(RowsName))
            {
                output.Attributes.Add("rows", Rows);
            }

            output.Attributes.Add(new TagHelperAttribute("class", "form-control"));

            output.PreElement.SetHtmlContent($@"<div class='{colSize} form-group'>
                        <label>
                            {DisplayName}
                        </label><div>");
            output.PostElement.SetHtmlContent($@"</div>
                       <small class='validation-error-label' data-valmsg-replace='true' data-valmsg-for='{For.Name}'></small>
                    </div>");

            base.Process(context, output);
        }
    }
}
