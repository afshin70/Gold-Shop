using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gold.Infrastracture.TagHelpers.SmartTags
{
    [HtmlTargetElement("smart-panel-header")]
    public class SmartPanelHeaderTagHelper : TagHelper
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


            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("class", "panel-heading");

            output.PreContent.AppendHtml($@"<div class='page-header-content'>
                                                    <h5 class='page-title'>
                                                        {Title}
                                                    </h5>
                                                    <div class='heading-elements'>
                                                        <ul class='icons-list'>
                                                            <li><a data-action='collapse'></a></li>
                                                        </ul>
                                                    </div>
                                                </div>");


            //output.PostElement.AppendHtml($@"<div class='panel-footer'>
            //                                    <div class='text-right'>
            //                                        <button class='btn btn-success btn-labeled legitRipple' type='submit'><b><i class='fa fa-search'></i></b>جستجو</button>&nbsp;<button class='btn bg-slate btn-labeled legitRipple' onclick='RedirectTo('/Students')' type='button'><b><i class='fa fa-backspace'></i></b>انصراف</button></div></div>");

            base.Process(context, output);
        }
    }
}
