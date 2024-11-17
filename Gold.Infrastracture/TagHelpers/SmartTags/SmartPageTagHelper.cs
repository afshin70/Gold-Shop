using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.Infrastracture.TagHelpers.SmartTags
{
    [HtmlTargetElement("smart-page")]
    public class SmartPageTagHelper : TagHelper
    {
        private const string TitleAttributeName = "asp-title";

        [HtmlAttributeName(TitleAttributeName)]
        public string Title { get; set; }

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


            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("class", "content");

            output.PreElement.AppendHtml($@"<div class='page-header'>
                                                 <div class='page-header-content'>
                                                    <div class='page-title'>
                                                        <h4><i class='icon-plus-circle2'></i> <span class='text-semibold'>{Title}</span></h4>
                                                    </div>
                                                </div>
                                            </div>");


            base.Process(context, output);
        }
    }
}
