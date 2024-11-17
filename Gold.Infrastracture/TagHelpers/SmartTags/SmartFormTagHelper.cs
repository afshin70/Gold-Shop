using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using Gold.Resources;

namespace Gold.Infrastracture.TagHelpers.SmartTags
{
    [HtmlTargetElement("smart-form")]
    public class SmartFormTagHelper : FormTagHelper
    {
        private const string BackRouteValuesAttributeName = "asp-back-route-values";
        private const string RemoveButtonAttributeName = "asp-remove-btn";
        private const string RemoveButtonSubmitAttributeName = "asp-remove-btn-submit";
        private const string BackToControllerAttributeName = "asp-back-to-controller";
        private const string BackToActionAttributeName = "asp-back-to-action";
        private const string BackToLinkTextAttributeName = "asp-back-to-text";
        private const string SubmitBtnTextAttributeName = "asp-submit-btn-text";
        private const string PartialViewAttributeName = "asp-partial-view";
        private const string TitlePartialViewAttributeName = "asp-title-partial-view";
        private const string TitleAttributeName = "asp-title";
        private const string SubmitButtonJsAttributeName = "asp-submit-btn-js";
        private const string SearchButtonJsAttributeName = "asp-search-btn-js";
        private const string CancelButtonJsAttributeName = "asp-cancel-btn-js";
        private const string ExcelUrlAttributeName = "asp-excel-url";
        private const string ExcelAttributeName = "asp-excel-button";

        private const string FormIdAttributeName = "id";

        ///// <summary>
        ///// An expression to be evaluated against the current model
        ///// </summary>
        //[HtmlAttributeName(ForAttributeName)]
        //public ModelExpression For { get; set; }

        [HtmlAttributeName(BackRouteValuesAttributeName)]
        public object BackRouteValues { get; set; }

        [HtmlAttributeName(RemoveButtonAttributeName)]
        public bool RemoveButton { get; set; }

        [HtmlAttributeName(ExcelAttributeName)]
        public bool ExcelButton { get; set; }

        [HtmlAttributeName(ExcelUrlAttributeName)]
        public string ExcelUrl { get; set; }

        [HtmlAttributeName(RemoveButtonSubmitAttributeName)]
        public bool RemoveButtonSubmit { get; set; }

        [HtmlAttributeName(BackToControllerAttributeName)]
        public string ControllerName { get; set; }

        [HtmlAttributeName(BackToActionAttributeName)]
        public string ActionName { get; set; } = "Index";

        [HtmlAttributeName(BackToLinkTextAttributeName)]
        public string BackToLinkText { get; set; } = Captions.Cancel;

        [HtmlAttributeName(SubmitBtnTextAttributeName)]
        public string SubmitBtnText { get; set; } = Captions.SubmitInformation;

        [HtmlAttributeName(TitlePartialViewAttributeName)]
        public string TitlePartialView { get; set; }

        [HtmlAttributeName(PartialViewAttributeName)]
        public bool IsPartialView { get; set; }


        [HtmlAttributeName(TitleAttributeName)]
        public string Title { get; set; }

        [HtmlAttributeName(SubmitButtonJsAttributeName)]
        public string SubmitButtonJs { get; set; }


        [HtmlAttributeName(SearchButtonJsAttributeName)]
        public string SearchButtonJs { get; set; }

        [HtmlAttributeName(CancelButtonJsAttributeName)]
        public string CancelButtonJs { get; set; }

        [HtmlAttributeName(FormIdAttributeName)]
        public string FormId { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="generator">HTML generator</param>
        public SmartFormTagHelper(IHtmlGenerator generator) : base(generator)
        {
        }

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

            output.TagName = "form";
            output.TagMode = TagMode.StartTagAndEndTag;

            //var x = Generator.GenerateValidationMessage(ViewContext, For.ModelExplorer, For.Name, For.Metadata.me)

            if (!string.IsNullOrEmpty(FormId))
            {
                output.Attributes.Add("id", FormId);
            }

            if (!IsPartialView)
            {
                //open panel tag
                output.PreContent.AppendHtml("<div class='panel panel-flat'>");


                //open & close head tag
                if (!string.IsNullOrEmpty(Title))
                    output.PreContent.AppendHtml($@"<div class='panel-heading'>
                                                    <h6 class='panel-title'>{Title}<a class='heading-elements-toggle'><i class='icon-more'></i></a></h6>
                                                    <div class='heading-elements'>
                                                        <ul class='icons-list'>
                                                            <li><a data-action='collapse'></a></li>
                                                        </ul>
                                                    </div>
                                                </div>");

                //open body tag
                output.PreContent.AppendHtml("<div class='panel-body'>");
                //close body tag
                output.PostContent.AppendHtml("</div>");

                var builder = new HtmlContentBuilder();
                //open footer tag
                string submitBtn = null;
                string searchBtn = null;
                string cancelBtn = null;
                string excelBtn = null;
                builder.AppendHtml($"<div class='panel-footer text-right'>");

                if (!RemoveButtonSubmit)
                {
                    submitBtn = string.IsNullOrEmpty(SubmitButtonJs) ? $"<button type='submit' class='btn btn-success btn-labeled'>{SubmitBtnText} <b><i class='fa fa-check-circle'></i></b></button>" : $"<button type='button' id='submit-{FormId}' onclick='{SubmitButtonJs};' class='btn btn-success btn-labeled'><span>{SubmitBtnText}</span> <b><i class='fa fa-check-circle'></i></b></button>";
                    builder.AppendHtml($"{submitBtn}&nbsp;");
                }

                if (RemoveButtonSubmit && !string.IsNullOrEmpty(SearchButtonJs))
                {
                    searchBtn = $"<button type='button' id='submit-{FormId}' onclick='{SearchButtonJs};' class='btn btn-success btn-labeled'><span>جستجو</span> <b><i class='fa fa-check-circle'></i></b></button>";
                    builder.AppendHtml($"{searchBtn}&nbsp;");
                }

                if (ExcelButton)
                {
                    excelBtn = $@"<button type = 'button' class='btn btn-info btn-labeled' onclick='javascript: GoToReport(""{ExcelUrl}"", ""#frm-search"")'><span>خروجی اکسل</span><b><i class='fa fa-print'></i></b></button>";
                    builder.AppendHtml($"{excelBtn}&nbsp;");
                }
                if (!RemoveButton)
                {
                    //var actionAnchor = Generator.GenerateActionLink(
                    //                    ViewContext,
                    //                    linkText: BackToLinkText,
                    //                    actionName: ActionName,
                    //                    controllerName: ControllerName,
                    //                    fragment: null,
                    //                    hostname: null,
                    //                    htmlAttributes: new Dictionary<string, object> { { "class", "btn bg-slate btn-labeled" } },
                    //                    //htmlAttributes: new Dictionary<string, object> { { "class", "btn bg-slate" } },
                    //                    protocol: null,
                    //                    routeValues: BackRouteValues
                    //                    );

                    //actionAnchor.InnerHtml.AppendHtml("<b><i class='fa fa-times-circle'></i></b>");
                    var parameters = string.Empty;
                    var temp = $"{{";

                    if (!string.IsNullOrEmpty(CancelButtonJs))
                    {
                        string[] inputParameters = CancelButtonJs.Split(",");
                        var i = 1;
                        foreach (var item in inputParameters)
                        {
                            temp += $"{item}:$('#{item}').val()";
                            if (inputParameters.Length != i)
                            {
                                temp += ", ";
                            }
                            i++;
                        }
                    }
                    parameters = temp + $"}}";

                    cancelBtn = $"<button type='button' onclick=\"OpenWindowCreate('/{ControllerName}/{ActionName}',{parameters},'{ControllerName}');\" class='btn bg-slate btn-labeled legitRipple'><span>{Captions.Cancel}</span> <b><i class='fa fa-times-circle'></i></b></button>";

                    builder.AppendHtml(cancelBtn);
                }
                else
                {
                    var actionAnchor = Generator.GenerateActionLink(
                                        ViewContext,
                                        linkText: BackToLinkText,
                                        actionName: ActionName,
                                        controllerName: ControllerName,
                                        fragment: null,
                                        hostname: null,
                                        htmlAttributes: new Dictionary<string, object> { { "class", "btn bg-slate btn-labeled" } },
                                        //htmlAttributes: new Dictionary<string, object> { { "class", "btn bg-slate" } },
                                        protocol: null,
                                        routeValues: BackRouteValues
                                        );

                    actionAnchor.InnerHtml.AppendHtml("<b><i class='fa fa-times-circle'></i></b>");
                    builder.AppendHtml(actionAnchor);

                }
                //close footer tag
                builder.AppendHtml("</div>");
                output.PostContent.AppendHtml(builder);
                //close panel tag
                output.PostContent.AppendHtml("</div>");
            }
            else if (IsPartialView)
            {
                output.PreElement.AppendHtml("<div class='panel-body'>");
                output.PostElement.AppendHtml("</div>");

                output.PreContent.AppendHtml($@"<div class='panel panel-flat'>
                    <div class='panel-heading'>
                        <h5 class='panel-title'>{TitlePartialView}</h5>
                        <div class='heading-elements'>
                            <ul class='icons-list'>
                                <li><a data-action='collapse'></a></li>
                            </ul>
                        </div>
                    </div><div class='panel-body'>");

                var builder = new HtmlContentBuilder();
                output.PostContent.AppendHtml($@"</div><div class='panel-footer'>
                    <div class='text-right'><button type = 'submit' class='btn btn-success btn-labeled'>ثبت اطلاعات <b><i class='fa fa-check-circle'></i></b></button>
                </div></div></div>");
            }

            base.Process(context, output);

            //string tag = this.CreateHtmlTag(inputRender);
            //output.Content.Clear();
            //output.Content.SetHtmlContent(tag);
        }
    }
}
