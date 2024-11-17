using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.Infrastracture.TagHelpers.SmartTags
{
    [HtmlTargetElement("smart-row")]
    public class RowTagHelper : TagHelper
    {
        private const string VisibleAttributeName = "asp-visible";
        private const string CSSClassAttributeName = "class";

        [HtmlAttributeName(VisibleAttributeName)]
        public bool Visible { get; set; } = true;

        [HtmlAttributeName(CSSClassAttributeName)]
        public string CSSClass { get; set; }

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

            if (!Visible)
            {
                output.TagName = "";
                output.Content.SetHtmlContent("");
                return;
            }

            string css = "section row";
            if (!string.IsNullOrEmpty(CSSClass))
            {
                css = $"{css} {CSSClass}";
            }

            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("class", css);

            //output.PreElement.AppendHtml($@"<div class='section row'>");

            //output.PostElement.AppendHtml($@"</div>");

            base.Process(context, output);
        }
    }
}
