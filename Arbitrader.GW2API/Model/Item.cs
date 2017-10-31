using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Arbitrader.GW2API.Entities;
using Arbitrader.GW2API.Results;

namespace Arbitrader.GW2API.Model
{
    /// <summary>
    /// Represents a single item in GW2 and all of its upstream and downstream recipe dependencies.
    /// </summary>
    internal class Item
    {
        /// <summary>
        /// The address of the icon used to represent the item in the GW2 UI.
        /// </summary>
        private Uri _icon;
        
        /// <summary>
        /// The type of the item.
        /// </summary>
        private ItemType _type;

        /// <summary>
        /// The rarity of the item.
        /// </summary>
        private Rarity _rarity;

        /// <summary>
        /// The level required to use the item.
        /// </summary>
        private int? _level;

        /// <summary>
        /// The value of the item if it were to be sold to an NPC vendor.
        /// </summary>
        private int? _vendor_value;

        /// <summary>
        /// The list of flags assigned to the item.
        /// </summary>
        internal List<Flag> Flags { get; set; } = new List<Flag>();

        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier in the GW2 API for the result associated with the entity.
        /// </summary>
        internal int ID { get; set; }

        /// <summary>
        /// Gets or sets the list of recipes that require the item in order to be crafted.
        /// </summary>
        internal List<Recipe> DependentRecipes { get; set; } = new List<Recipe>();

        /// <summary>
        /// Gets or sets the list of recipes for which the item is the output item.
        /// </summary>
        internal List<Recipe> GeneratingRecipes { get; set; } = new List<Recipe>();

        internal Listings Listings { get; set; } = new Listings();

        /// <summary>
        /// Initializes a new instance of <see cref="Item"/> from an existing entity.
        /// </summary>
        /// <param name="itemEntity">The entity containing the descriptors for the item.</param>
        internal Item(ItemEntity itemEntity)
        {
            this._icon = new Uri(itemEntity.Icon);
            this._type = Enum.TryParse(itemEntity.Type, out ItemType type) ? type : ItemType.Unknown;
            this._rarity = Enum.TryParse(itemEntity.Rarity, out Rarity rarity) ? rarity : Rarity.Unknown;
            this._level = itemEntity.Level;
            this._vendor_value = itemEntity.VendorValue;

            this.Name = itemEntity.Name;
            this.ID = itemEntity.APIID;

            foreach (var flagEntity in itemEntity.Flags)
                this.Flags.Add((Flag)Enum.Parse(typeof(Flag), flagEntity.Name));
        }

        internal int GetBestPrice(int count)
        {
            var marketPrice = this.GetMarketPrice(count);
            var craftPrice = this.GeneratingRecipes.Min(r => r.GetPrice(count));
            return Math.Min(marketPrice, craftPrice);
        }

        internal int GetMarketPrice(int count)
        {
            var buyListings = new Queue<Listing>(Listings.Where(l => l.Direction == Direction.Buy)
                                                         .OrderBy(l => l.UnitPrice));

            if (buyListings.Sum(l => l.Quantity) < count)
                throw new InvalidOperationException($"Insufficient listings in the market to allow {count} of the item to be purchased.");

            var price = 0;
            var remaining = count;
            var exhaustedListings = new List<Listing>();

            while (remaining > 0)
            {
                var bestListing = buyListings.Dequeue();
                price += bestListing.UnitPrice * Math.Min(bestListing.Quantity, remaining);
                remaining -= bestListing.Quantity;
            }

            return price;
        }

        /// <summary>
        /// Returns a string representation of the <see cref="Item"/>.
        /// </summary>
        /// <returns>A string representation of the <see cref="Item"/>.</returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}