namespace Arbitrader.GW2API.Results
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Arbitrader.GW2API.Entities;

    /// <summary>
    /// A result from an Item query to the GW2 API.
    /// </summary>
    public class ItemResult : APIDataResult
    {
        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Gets or sets the address of the image used to represent the item in the GW2 UI.
        /// </summary>
        public string icon { get; set; }

        /// <summary>
        /// Gets or sets the type of the item.
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// Gets or sets the rarity of the item.
        /// </summary>
        public string rarity { get; set; }
        
        /// <summary>
        /// Gets or sets the level required to use the item.
        /// </summary>
        public int? level { get; set; }

        /// <summary>
        /// Gets or sets the value of the item if it were to be sold to an NPC vendor.
        /// </summary>
        public int? vendor_value { get; set; }

        /// <summary>
        /// Gets or sets the list of flags assigned to the item.
        /// </summary>
        public IList<ItemFlagResult> flags { get; set; } = new List<ItemFlagResult>();

        /// <summary>
        /// Returns a <see cref="ItemEntity"/> that contains the data from the <see cref="ItemResult"/>.
        /// </summary>
        /// <returns>A <see cref="ItemEntity"/> that contains the data from the <see cref="ItemResult"/>.</returns>
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