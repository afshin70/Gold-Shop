using Gold.Resources;
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
    [HtmlTargetElement("smart-show")]

    public class SmartShowTagHelper : TagHelper
    {
        private const string RedirectToAttributeName = "asp-redirect-to";
        private const string EditUrlAttributeName = "asp-edit-url";
        private const string DeleteUrlAttributeName = "asp-delete-url";
        private const string EditAttributeName = "asp-edit-button";
        private const string DeleteAttributeName = "asp-delete-button";
        private const string SMSAttributeName = "asp-SMS-button";
        private const string SMSUrlAttributeName = "asp-SMS-url";
        private const string FormTitle = "asp-title";

        [HtmlAttributeName(RedirectToAttributeName)]
        public string RedirectTo { get; set; }
        [HtmlAttributeName(EditAttributeName)]
        public bool EditButton { get; set; }


        [HtmlAttributeName(DeleteAttributeName)]
        public bool DeleteButton { get; set; }
        [HtmlAttributeName(SMSAttributeName)]
        public bool SMSButton { get; set; }
        [HtmlAttributeName(EditUrlAttributeName)]
        public string EditUrl { get; set; }

        [HtmlAttributeName(DeleteUrlAttributeName)]
        public string DeleteUrl { get; set; }

        [HtmlAttributeName(SMSUrlAttributeName)]
        public string SMSUrl { get; set; }
        [HtmlAttributeName(FormTitle)]
        public string FormName { get; set; }

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
            output.Attributes.Add("class", "panel panel-flat");

            output.PreContent.AppendHtml($@"
            <div class='panel-heading'>
                <h5 class='panel-title'>{FormName}</h5>
                <div class='heading-elements'>
                    <ul class='icons-list'>
                        <li><a data-action='collapse'></a></li>
                    </ul>
                </div>
            </div><div class='panel-body'>");

            var builder = new HtmlContentBuilder();
            var editStr = "";
            var deleteStr = "";
            if (EditButton)
            {
                editStr = $@"&nbsp;<button type = 'button' class='btn btn-success btn-labeled' onclick='RedirectTo(""{EditUrl}"")'><b><i class='fa fa-pen'></i></b>" + Captions.Edit + "</button>";
            }
            if (DeleteButton)
            {
                deleteStr = $@"&nbsp;<button type = 'button' class='btn bg-slate btn-labeled' onclick=""{DeleteUrl}""><b><i class='fa fa-backspace'></i></b>" + Captions.Delete + "</button>";
            }
            var smsStr = "";
            if (SMSButton)
            {
                smsStr = $@"&nbsp;<button class='btn btn-danger btn-sm ph25 btn-labeled' type='button' onclick='javascript: GoToSendSMS(""/SendSMS/Preview"", ""#frm-search"",""{SMSUrl}"")'><b><i class='fa fa-sms'></i></b>ارسال پیامک</button>";
            }

            output.PostContent.AppendHtml($@"</div><div class='panel-footer'>
            <div class='text-right'>{editStr}{smsStr}&nbsp;<button type='button' class='btn btn-info btn-labeled' onclick='RedirectTo(""{RedirectTo}"")'><b><i class='fa fa-print'></i></b>" + Captions.Print + "</button>" + deleteStr + "</div></div>");

            base.Process(context, output);

            //string tag = this.CreateHtmlTag(inputRender);
            //output.Content.Clear();
            //output.Content.SetHtmlContent(tag);
        }
    }
}
