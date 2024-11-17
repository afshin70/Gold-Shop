using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.SharedKernel.Attributes.Validation
{
    /// <summary>
    /// split extension by '|'
    /// </summary>
    public class FileExtentionChecker : ValidationAttribute
    {
        private string _acceptExtentions;
        public FileExtentionChecker(string acceptExtentions)
        {
            _acceptExtentions = acceptExtentions.ToLower();
        }
        public override bool IsValid(object? value)
        {
            if (value != null)
            {
                IFormFile file = (IFormFile)value;
                string fileExtention = Path.GetExtension(file.FileName).ToLower();
                if (!_acceptExtentions.Split('|').Any(x => x == fileExtention))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
