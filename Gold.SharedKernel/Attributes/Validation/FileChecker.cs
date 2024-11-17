using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Gold.SharedKernel.Attributes.Validation
{
    /// <summary>
    /// split content type by '|'
    /// </summary>
    public class FileChecker : ValidationAttribute
    {
        private string _acceptContentTypes;
        private string _acceptExtentions;

        public FileChecker(string acceptExtentions, string acceptContentTypes)
        {
            _acceptContentTypes = acceptContentTypes.ToLower();
            _acceptExtentions = acceptExtentions.ToLower();

        }

        public override bool IsValid(object? value)
        {
            if (value != null)
            {
                IFormFile file = (IFormFile)value;
                if (!_acceptContentTypes.Split('|').Any(x => x == file.ContentType))
                {
                    return false;
                }
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
