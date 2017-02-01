using System.Collections.Generic;

namespace Arbitrader.GW2API
{
    internal class Item
    {
        int? id { get; set; }
        string name { get; set; }
        string icon { get; set; }
        string type { get; set; }
        string rarity { get; set; }
        int? level { get; set; }
        int? vendor_value { get; set; }
        List<string> flags { get; set; }
        int? size { get; set; }
    }
}