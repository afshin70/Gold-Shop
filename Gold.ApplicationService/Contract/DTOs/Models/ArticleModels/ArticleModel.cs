using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.Models.ArticleModels
{
    public class ArticleModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? ImageFileName { get; set; }
        public string? VideoFileName { get; set; }
        public bool HasVideo => !string.IsNullOrEmpty(VideoFileName);
        public bool HasImage => !string.IsNullOrEmpty(ImageFileName);
        public string? Description { get; set; }
        public bool Status { get; set; }
        public string StatusTitle { get; set; }
        public DateTime RegisterDate { get; set; }
        public string RegisterDatePersian { get; set; }
        public int OrderNo { get; set; }
    }
}
