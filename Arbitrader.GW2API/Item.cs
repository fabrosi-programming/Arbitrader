using System.Collections.Generic;

namespace Arbitrader.GW2API
{
    internal class Item
    {
        public string name { get; set; }
        public string type { get; set; }
        public int level { get; set; }
        public string rarity { get; set; }
        public int? vendor_value { get; set; }
        public List<string> flags { get; set; }
        public int id { get; set; }
        public string icon { get; set; }
    }
}