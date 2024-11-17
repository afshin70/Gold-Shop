using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;
using System.Xml.Linq;

namespace Gold.ApplicationService.Contract.DTOs.Models.CustomerModels
{
    public class CustomerProfileModel
    {
        public string? FullName { get; set; }
        public string? NationalCode { get; set; }
        public string? FatherName { get; set; }
        public string? Mobile { get; set; }

        public int? CityId { get; set; }
        public int? ProvinceId { get; set; }
        public string? PostalCode { get; set; }
        public string? Address { get; set; }
        public string? SanaCode { get; set; }
        public string? JobTitle { get; set; }
        public string? BirthDate { get; set; }
        public string? ProfileImage { get; set; }
    }
}
