using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buss.Layer.DTOs
{
    public class BrandDTO
    {
        public int BrandID { get; set; }
        public string BrandName { get; set; }
        public string? Image { get; set; }
        public List<int> CategoryIDs {  get; set; }
    }
}
