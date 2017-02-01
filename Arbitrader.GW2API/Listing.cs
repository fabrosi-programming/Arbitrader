using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrader.GW2API
{
    public class Listing
    {
        int listings { get; set; }
        int unit_price { get; set; }
        int quantity { get; set; }

        public static Listing FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Listing>(json);
        }

        public override string ToString()
        {
            return $"{listings}::{unit_price}::{quantity}";
        }
    }
}
