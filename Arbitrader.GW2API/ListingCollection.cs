using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrader.GW2API
{
    public class ListingCollection
    {
        public int id { get; set; }
        public List<Listing> buys { get; set; }
        public List<Listing> sells { get; set; }

        public static ListingCollection FromJson(string json)
        {
            return JsonConvert.DeserializeObject<ListingCollection>(json);
        }

        public override string ToString()
        {
            return $"ID-{id}::Buys-{buys.Count}::Sells-{sells.Count}";
        }
    }
}
