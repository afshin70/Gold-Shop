using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.Infrastracture.TagHelpers.SmartTags
{
    public class SmartInputTagHelper : InputTagHelper
    {
        private const string SkinAttributeName = "asp-skin";


        [HtmlAttributeName(SkinAttributeName)]
        public SmartEditorSkins Skin { get; set; } = SmartEditorSkins.Default;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="generator">HTML generator</param>
        public SmartInputTagHelper(IHtmlGenerator generator) : base(generator)
        {
        }
    }



    public enum SmartEditorSkins
    {
        Default
    }
}
