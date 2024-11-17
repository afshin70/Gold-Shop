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
    [HtmlTargetElement("smart-editor", Attributes = ForAttributeName)]
    public class TextEditorTagHelper : SmartInputTagHelper
    {
        private const string ForAttributeName = "asp-for";
        private const string SkinAttributeName = "asp-skin";
        private const string CSSClassAttributeName = "class";
        private const string NameAttributeName = "asp-name";
        private const string CSSClassInputAttributeName = "asp-input-class";
        private const string FaIconAttributeName = "asp-fa-icon";
        private const string AutocompleteOffAttributeName = "asp-autocomplete-off";
        private const string VisibleAttributeName = "asp-visible";
        private const string SizeAttributeName = "asp-col-size";
        private const string PlaceHolderAttributeName = "asp-placeholder";
        private const string PlaceHolderTextAttributeName = "asp-placeholder-text";
        private const string ReadonlyAttributeName = "asp-readonly";
        private const string DisplayNameAttributeName = "asp-displayname";
        private const string TabIndexName = "asp-tab";
        ///// <summary>
        ///// An expression to be evaluated against the current model
        ///// </summary>
        //[HtmlAttributeName(ForAttributeName)]
        //public ModelExpression For { get; set; }

        [HtmlAttributeName(FaIconAttributeName)]
        public string FaIcon { get; set; }

        [HtmlAttributeName(TabIndexName)]
        public int TabIndex { get; set; } = 1000;

        [HtmlAttributeName(AutocompleteOffAttributeName)]
        public bool AutocompleteOff { get; set; }

        [HtmlAttributeName(VisibleAttributeName)]
        public bool Visible { get; set; } = true;

        [HtmlAttributeName(SizeAttributeName)]
        public int ColSize { get; set; } = 4;

        [HtmlAttributeName(PlaceHolderAttributeName)]
        public bool Placeholder { get; set; } = true;

        [HtmlAttributeName(PlaceHolderTextAttributeName)]
        public string PlaceholderText { get; set; }

        [HtmlAttributeName(DisplayNameAttributeName)]
        public string DisplayName { get; set; }

        [HtmlAttributeName(CSSClassAttributeName)]
        public string CSSClass { get; set; }

        [HtmlAttributeName(NameAttributeName)]
        public string Name { get; set; }

        [HtmlAttributeName(CSSClassInputAttributeName)]
        public string InputCSSClass { get; set; }

        [HtmlAttributeName(ReadonlyAttributeName)]
        public bool Readonly { get; set; }


        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="generator">HTML generator</param>
        public TextEditorTagHelper(IHtmlGenerator generator) : base(generator)
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

            if (!Visible)
            {
                output.TagName = "";
                output.Content.SetHtmlContent("");
                return;
            }

            string inputCSSClass = string.Empty;
            if (Skin == SmartEditorSkins.Default)
            {
                inputCSSClass = "form-control";
            }

            if (!string.IsNullOrEmpty(InputCSSClass))
            {
                inputCSSClass = $"{inputCSSClass} {InputCSSClass}";
            }

            if (!string.IsNullOrEmpty(inputCSSClass))
            {
                output.Attributes.Add("class", inputCSSClass);
            }

            if (AutocompleteOff)
            {
                output.Attributes.Add("autocomplete", "off");
            }

            if (Readonly)
            {
                output.Attributes.Add("readonly", true);
            }

            output.Attributes.Add("data-tab", TabIndex);

            output.SuppressOutput();

            //var anchorTagHelper = new AnchorTagHelper(Generator);
            //var anchorOutput = new TagHelperOutput("input", output.Attributes, (useCachedResult, encoder) => output.GetChildContentAsync());
            //var anchorContext = new TagHelperContext(output.Attributes, context.Items, context.UniqueId);
            //anchorTagHelper.Process(anchorContext, anchorOutput);

            //var htmlAttributes = new Dictionary<string, object>
            //{
            //    { "type", InputTypeName }
            //};

            output.TagName = "input";

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

            if (string.IsNullOrEmpty(DisplayName))
            {
                DisplayName = For.Metadata.DisplayName;
            }

            //output.TagMode = TagMode.StartTagAndEndTag;

            string colSize = SmartMainHelper.GetColSize(ColSize);

            if (!string.IsNullOrEmpty(CSSClass))
            {
                colSize = $"{colSize} {CSSClass}";
            }

            string name = For.Name;
            if (!string.IsNullOrEmpty(Name))
            {
                name = Name;
            }

            if (string.IsNullOrEmpty(FaIcon))
            {
                output.PreElement.SetHtmlContent($@"<div class='{colSize} form-group'>
                        <label >
                            {DisplayName}
                        </label>");
                output.PostElement.SetHtmlContent($@"<label lass='validation-error-label' data-valmsg-replace='true' data-valmsg-for='{name}'></label>
                    </div>");
            }
            else
            {
                output.PreElement.SetHtmlContent($@"<div class='{colSize} form-group'>
                        <label>
                            {DisplayName}
                        </label><div class='input-group'>");
                output.PostElement.SetHtmlContent($@"  <span class='input-group-addon'><i class='fa fa-{(string.IsNullOrEmpty(FaIcon) ? "angle-double-left" : FaIcon)}'></i></span></div>
                       <small class='validation-error-label' data-valmsg-replace='true' data-valmsg-for='{name}'></small>
                    </div>");
            }


            base.Process(context, output);

            //string tag = this.CreateHtmlTag(inputRender);
            //output.Content.Clear();
            //output.Content.SetHtmlContent(tag);
        }

        //private string CreateHtmlTag(string inputRender)
        //{
        //    string tag = $@"<div class='col-lg-4 col-md-4 col-sm-6 col-xs-12 pull-right mb20'>
        //                <small class='col-lg-12 no-padding mb5'>
        //                    <label asp-for='{For.Name}'></label>
        //                </small>
        //                <label for='MinScore' class='field prepend-icon'>
        //                    {inputRender}
        //                    <span class='col-lg-12 text-danger mt5 no-padding field-validation-valid' data-valmsg-replace='true' data-valmsg-for='{For.Name}'></span>
        //                    <label for='{For.Name}' class='field-icon'>
        //                        <i class='fa fa-{(!string.IsNullOrEmpty(FaIcon) ? "angle-double-left" : FaIcon)}'></i>
        //                    </label>
        //                </label>
        //            </div>";

        //    return tag;
        //}
    }
}
