namespace Arbitrader.GW2API.Results
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Arbitrader.GW2API.Entities;

    public class ItemResult : APIDataResult
    {
        public string name { get; set; }
        public string icon { get; set; }
        public string type { get; set; }
        public string rarity { get; set; }
        public int? level { get; set; }
        public int? vendor_value { get; set; }
        public IList<ItemFlagResult> flags { get; set; } = new List<ItemFlagResult>();

        public override Entity ToEntity()
        {
            return new ItemEntity()
            {
                APIID = this.id,
                LoadDate = this.LoadDate,
                Name = this.name,
                Icon = this.icon,
                Type = this.type,
                Rarity = this.rarity,
                Level = this.level,
                VendorValue = this.vendor_value,
                Flags = this.flags.Select(f => (ItemFlagEntity)f.ToEntity()).ToList()
            };
        }
    }
}