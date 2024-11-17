using Gold.Domain.Entities.AuthEntities;
using Gold.Domain.Entities.Base;
using Gold.Resources;
using Gold.SharedKernel.Attributes.Validation;
using Gold.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Gold.Domain.Entities
{
    public class Customer : IEntity<int>
    {
        public int Id { get; set; }

        public NationalityType? Nationality { get; set; }
        public GenderType? Gender { get; set; }
        public string NationalCode { get; set; }
        public string? FatherName { get; set; }
        public string? PostalCode { get; set; }
        public string? Address { get; set; }

		public string? JobTitle { get; set; }
		public DateTime? BirthDate { get; set; }

        public int? BirthDateYear { get; set; }
        public byte? BirthDateMonth { get; set; }
        public byte? BirthDateDay { get; set; }

        public int?   CityId { get; set; }
        public City? City{ get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public string? SanaCode { get; set; }

        public ICollection<EssentialTel> EssentialTels { get; set; }
        public ICollection<BankCardNo> BankCardNo { get; set; }
        public ICollection<Document> Documents { get; set; }
        public ICollection<EditInformationRequest> EditInformationRequests { get; set; }
        public ICollection<CustomerMessage> CustomerMessages { get; set; }
        public ICollection<ProfileImage> ProfileImages { get; set; }
        public ICollection<FavoritProduct> FavoritProducts { get; set; }

    }
}
