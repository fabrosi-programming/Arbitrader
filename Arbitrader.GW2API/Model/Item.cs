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
    public class Item
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
        /// Gets <see cref="true"/> if the <see cref="Item"/> can be bought and sold either
        /// on the market or to a vendor and <see cref="false"/> otherwise.
        /// </summary>
        public bool IsBuyable
        {
            get
            {
                return !(this.Flags.Contains(Flag.NoSell)
                         || this.Flags.Contains(Flag.AccountBound)
                         || this.Flags.Contains(Flag.MonsterOnly)
                         || this.Flags.Contains(Flag.SoulbindOnAcquire));
            }
        }

        /// <summary>
        /// Gets <see cref="true"/> if there are crafting recipes for which the <see cref="Item"/> is the output and
        /// <see cref="false"/> otherwise.
        /// </summary>
        public bool IsCraftable
        {
            get
            {
                return this.GeneratingRecipes.Count > 0;
            }
        }

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

        public AcquisitionStep GetBestSteps(int count)
        {
            return this.GetAcquisitionSteps(count).MinimizeOn(s => s.GetBestPrice());
        }

        public AcquisitionStep WithCostLessThan(int cost)
        {
            var count = 1;
            var steps = GetBestSteps(count);
            var price = steps.GetBestPrice();

            while (price <= cost * count)
            {
                count += 1;
                try
                {
                    steps = GetBestSteps(count);
                }
                catch (InsufficientListingsException)
                {
                    break;
                }

                price = steps.GetBestPrice();
            }

            // step back to before the last iteration and return the plan to execute at the limit price
            return GetBestSteps(count - 1);
        }

        public IEnumerable<AcquisitionStep> GetAcquisitionSteps(int count)
        {
            if (!this.IsBuyable && !this.IsCraftable)
                yield return new AcquireStep(this, count);

            if (this.IsBuyable)
                yield return new BuyStep(this, count, (c) => this.GetMarketPrice(c, Direction.Buy));

            if (this.IsCraftable)
                foreach (var recipe in this.GeneratingRecipes)
                    yield return new CraftStep(recipe, count);
        }

        /// <summary>
        /// Returns the best price that can be paid on the market to buy or sell the specified number
        /// of the item.
        /// </summary>
        /// <param name="count">The number of the item to be priced.</param>
        /// <param name="direction"><see cref="Direction.Buy"/> if the caller intends to buy the item; 
        /// <see cref="Direction.Sell"/> if the caller intendes to sell the item.</param>
        /// <returns>The lowest price that can be paid on the market to obtain the specified number
        /// of the item.</returns>
        internal int GetMarketPrice(int count, Direction direction)
        {
            // invert buy/sell direction to translate the caller's request to the market's language
            var marketDirection = direction == Direction.Buy ? Direction.Sell : Direction.Buy;

            var listings = Listings.Where(l => l.Direction == marketDirection);
            var orderedListings = new Queue<Listing>(marketDirection == Direction.Sell ? listings.OrderBy(l => l.UnitPrice)
                                                                                       : listings.OrderByDescending(l => l.UnitPrice));

            if (orderedListings.Sum(l => l.Quantity) < count)
                throw new InsufficientListingsException(this, count);

            var price = 0;
            var remaining = count;

            while (remaining > 0)
            {
                var bestListing = orderedListings.Dequeue();
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