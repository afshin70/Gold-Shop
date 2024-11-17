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
    [HtmlTargetElement("smart-checkbox", Attributes = ForAttributeName)]
    public class CheckBoxTagHelper : InputTagHelper
    {
        private const string ForAttributeName = "asp-for";
        private const string TextAttributeName = "asp-text";
        private const string CheckedAttributeName = "asp-checked";
        private const string SizeAttributeName = "asp-col-size";
        private const string CSSClassAttributeName = "class";
        private const string NameAttributeName = "name";
        private const string DisabledName = "asp-disabled";

        ///// <summary>
        ///// An expression to be evaluated against the current model
        ///// </summary>
        //[HtmlAttributeName(ForAttributeName)]
        //public ModelExpression For { get; set; }

        [HtmlAttributeName(TextAttributeName)]
        public string Text { get; set; }

        [HtmlAttributeName(NameAttributeName)]
        public string Name { get; set; }

        [HtmlAttributeName(DisabledName)]
        public bool IsDisabled { get; set; }

        [HtmlAttributeName(CheckedAttributeName)]
        public bool? Checked { get; set; }

        [HtmlAttributeName(SizeAttributeName)]
        public int ColSize { get; set; } = 4;

        [HtmlAttributeName(CSSClassAttributeName)]
        public string CSSClass { get; set; }


        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="generator">HTML generator</param>
        public CheckBoxTagHelper(IHtmlGenerator generator) : base(generator)
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

            output.SuppressOutput();

            //var anchorTagHelper = new AnchorTagHelper(Generator);
            //var anchorOutput = new TagHelperOutput("input", output.Attributes, (useCachedResult, encoder) => output.GetChildContentAsync());
            //var anchorContext = new TagHelperContext(output.Attributes, context.Items, context.UniqueId);
            //anchorTagHelper.Process(anchorContext, anchorOutput);

            //var htmlAttributes = new Dictionary<string, object>
            //{
            //    { "type", InputTypeName }
            //};

            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            string colSize = "col-lg-4 col-md-4 col-sm-6 col-xs-12";
            if (ColSize == 2)
            {
                colSize = "col-lg-2 col-md-2 col-sm-6 col-xs-12";
            }
            else if (ColSize == 3)
            {
                colSize = "col-lg-3 col-md-3 col-sm-6 col-xs-12";
            }
            else if (ColSize == 6)
            {
                colSize = "col-lg-6 col-md-6 col-sm-6 col-xs-12";
            }

            if (!string.IsNullOrEmpty(CSSClass))
            {
                colSize = $"{colSize} {CSSClass}";
            }

            string name = For.Name;
            if (!string.IsNullOrEmpty(Name))
            {
                name = Name;
            }

            string text;
            if (!string.IsNullOrEmpty(Text))
            {
                text = Text;
            }
            else
            {
                text = For.Metadata.DisplayName;
            }

            string val;
            if (string.IsNullOrEmpty(Value))
            {
                val = "true";
            }
            else
            {
                val = Value;
            }

            var inputChecked = Checked.HasValue ? Checked.Value ? " checked='checked'" : null : For.Model != null && (bool)For.Model ? " checked='checked'" : null;
            var isDisabled = IsDisabled ? "disabled" : "";
            output.Attributes.Add("class", colSize);
            output.PostContent.AppendHtml($@"<div class='checkbox checkbox-switchery'><label>
                                            <input type='checkbox' class='switchery' name='{name}' {inputChecked} {isDisabled} id='{name}' value='{val}' />
                                            {text}
                                           </label></div>");

            //var x = Generator.GenerateValidationMessage(ViewContext, For.ModelExplorer, For.Name, For.Metadata.me)

            //output.PreElement.SetHtmlContent($@"<div class='col-lg-4 col-md-4 col-sm-6 col-xs-12 pull-right mb20'>
            //            <small class='col-lg-12 no-padding mb5'>
            //                {For.Metadata.DisplayName}
            //            </small>
            //            <label for='{For.Name}' class='field prepend-icon'>
            //                <span class='col-lg-12 text-danger mt5 no-padding field-validation-valid' data-valmsg-replace='true' data-valmsg-for='{For.Name}'></span>
            //                <label class='p5 pt10'>");

            //output.PostElement.SetHtmlContent($@" {Text} </label></label></div>");

            base.Process(context, output);

            //string tag = this.CreateHtmlTag(inputRender);
            //output.Content.Clear();
            //output.Content.SetHtmlContent(tag);
        }
    }
}
