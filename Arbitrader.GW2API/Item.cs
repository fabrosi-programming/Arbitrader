using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace Arbitrader.GW2API
{
    public class Item
    {
        static readonly string Controller = "items";

        public string name { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public int level { get; set; }
        public string rarity { get; set; }
        public int vendor_value { get; set; }
        public List<string> game_types { get; set; }
        public List<string> flags { get; set; }
        public List<string> restrictions { get; set; }
        public int id { get; set; }
        public string chat_link { get; set; }
        public string icon { get; set; }
        
        //ItemDetails Details { get; set; }

        public static Item FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Item>(json);
        }

        public override string ToString()
        {
            return name;
        }
    }
}