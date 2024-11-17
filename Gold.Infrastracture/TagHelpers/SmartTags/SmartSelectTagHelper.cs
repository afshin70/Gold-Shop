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
    [HtmlTargetElement("smart-select", Attributes = ForAttributeName)]
    public class SmartSelectTagHelper : SelectTagHelper
    {
        private const string ForAttributeName = "asp-for";
        //private const string SkinAttributeName = "asp-skin";
        private const string SizeAttributeName = "asp-col-size";
        private const string VisibleAttributeName = "asp-visible";
        private const string DisabledAttributeName = "asp-disabled";
        private const string IDAttributeName = "asp-id";
        private const string MultipleAttributeName = "asp-multiple";
        private const string TabIndexName = "asp-tab";
        ///// <summary>
        ///// An expression to be evaluated against the current model
        ///// </summary>
        //[HtmlAttributeName(ForAttributeName)]
        //public ModelExpression For { get; set; }

        [HtmlAttributeName(SizeAttributeName)]
        public int ColSize { get; set; } = 4;

        [HtmlAttributeName(TabIndexName)]
        public int TabIndex { get; set; } = 1000;

        [HtmlAttributeName(VisibleAttributeName)]
        public bool Visible { get; set; } = true;

        [HtmlAttributeName(MultipleAttributeName)]
        public bool Multiple { get; set; }

        [HtmlAttributeName(DisabledAttributeName)]
        public bool Disabled { get; set; }
        [HtmlAttributeName(IDAttributeName)]
        public string ID { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="generator">HTML generator</param>
        public SmartSelectTagHelper(IHtmlGenerator generator) : base(generator)
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

            if (!string.IsNullOrEmpty(ID))
            {
                output.Attributes.Add("ID", ID);
            }
            output.Attributes.Add("class", "form-control");
            output.Attributes.Add("data-tab", TabIndex);

            output.TagName = "select";
            output.TagMode = TagMode.StartTagAndEndTag;

            string colSize = SmartMainHelper.GetColSize(ColSize);

            //// < div class="btn-group bootstrap-select dropup" style="width: 100%;"><button type = "button" class="btn dropdown-toggle btn-default" data-toggle="dropdown" role="button" title="Alaska" aria-expanded="false"><span class="filter-option pull-left">Alaska</span>&nbsp;<span class="bs-caret"><span class="caret"></span></span></button><div class="dropdown-menu open" role="combobox" style="max-height: 181.083px; overflow: hidden; min-height: 126px;"><ul class="dropdown-menu inner" role="listbox" aria-expanded="false" style="max-height: 165.083px; overflow-y: auto; min-height: 110px;"><li data-original-index="0" class="selected"><a tabindex = "0" class="" data-tokens="null" role="option" aria-disabled="false" aria-selected="true"><span class="text">Alaska</span><span class=" icon-checkmark3 check-mark"></span></a></li><li data-original-index="1"><a tabindex = "0" class="" data-tokens="null" role="option" aria-disabled="false" aria-selected="false"><span class="text">Hawaii</span><span class=" icon-checkmark3 check-mark"></span></a></li><li data-original-index="2"><a tabindex = "0" class="" data-tokens="null" role="option" aria-disabled="false" aria-selected="false"><span class="text">California</span><span class=" icon-checkmark3 check-mark"></span></a></li><li data-original-index="3"><a tabindex = "0" class="" data-tokens="null" role="option" aria-disabled="false" aria-selected="false"><span class="text">Nevada</span><span class=" icon-checkmark3 check-mark"></span></a></li><li data-original-index="4"><a tabindex = "0" class="" data-tokens="null" role="option" aria-disabled="false" aria-selected="false"><span class="text">Oregon</span><span class=" icon-checkmark3 check-mark"></span></a></li></ul></div><select class="bootstrap-select" data-width="100%" tabindex="-98">
            ////<option value = "AK" > Alaska </ option >

            ////                                 < option value="HI">Hawaii</option>
            ////<option value = "CA" > California </ option >

            ////                                 < option value="NV">Nevada</option>
            ////<option value = "OR" > Oregon </ option >

            ////        
            output.PreElement.AppendHtml($@"<div class='{colSize} form-group'>
                        <label>
                            {For.Metadata.DisplayName}
                        </label>
                       ");

            output.PostElement.AppendHtml($@"
                     <small class='validation-error-label' data-valmsg-replace='true' data-valmsg-for='{For.Name}'></small>
                    </div>");

            if (Multiple)
            {
                output.Attributes.Add("multiple", "multiple");
            }

            if (Disabled)
            {
                //output.Attributes.Add(new TagHelperAttribute("disabled", null, HtmlAttributeValueStyle.NoQuotes));
                output.Attributes.Add("disabled", true);
                output.PreElement.AppendHtml($"<input type='hidden' name='{For.Name}' value='{For.Model.ToString()}' />");
            }

            base.Process(context, output);
        }
    }
}
