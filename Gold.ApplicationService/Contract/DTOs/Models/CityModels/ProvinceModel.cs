using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gold.ApplicationService.Contract.DTOs.Models.CityModels
{
    public class ProvinceModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<CityModel> Cities { get; set; } = new();
    }
}
