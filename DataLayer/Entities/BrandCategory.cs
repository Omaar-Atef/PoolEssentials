using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class BrandCategory
    {
        public int BrandID { get; set; }

        [JsonIgnore]
        public Brand Brand { get; set; }

        public int CategoryID { get; set; }

        [JsonIgnore]
        public Category Category { get; set; }
    }
}
