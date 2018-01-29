using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arbitrader.GW2API.Entities;

namespace Arbitrader.GW2API.Model
{
    internal class Items : IEnumerable<Item>
    {
        /// <summary>
        /// The internal collection of items.
        /// </summary>
        private List<Item> _items = new List<Item>();

        /// <summary>
        /// Gets the number of items in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return this._items.Count;
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Items"/>.
        /// </summary>
        public Items()
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="Items"/> with the given collection of items.
        /// </summary>
        /// <param name="items">The collection of items to initialize the internal collection with.</param>
        public Items(IEnumerable<Item> items)
        {
            foreach (var item in items)
                this._items.Add(item);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Items"/> with <see cref="Item"/>s taken from the given
        /// collection of <see cref="ItemEntity"/>.
        /// </summary>
        /// <param name="entities">The collection of entities used to generate the <see cref="Item"/>s to initialize
        /// the collection with.</param>
        public Items(IEnumerable<ItemEntity> entities)
            : this(entities.Select(e => new Item(e)))
        { }

        /// <summary>
        /// Loads individual listing data into the <see cref="Item"/>s within the collection.
        /// </summary>
        /// <param name="listings">The listings to be loaded.</param>
        public void AttachListings(IEnumerable<ListingEntity> listings)
        {
            foreach (var item in this._items)
            {
                var listing = listings.Where(l => l.APIID == item.ID).FirstOrDefault();

                if (listing == null)
                    continue;

                foreach (var individualListing in listing.IndividualListings)
                    item.Listings.Merge(new Listing(individualListing));
            }
        }

        /// <summary>
        /// Associates all of the <see cref="Item"/>s in the collection with any recipes that output
        /// those items.
        /// </summary>
        /// <param name="getGeneratingRecipes">A function to get the generating recipes for an item.</param>
        public void AttachGeneratingRecipes(Func<Item, IEnumerable<Recipe>> getGeneratingRecipes)
        {
            var items = this._items.Where(i => i.GeneratingRecipes?.Count == 0).ToList();

            foreach (var item in items)
                item.GeneratingRecipes = getGeneratingRecipes(item).ToList();
        }

        /// <summary>
        /// Returns a new <see cref="Items"/> collection that includes the contents of the current <see cref="Items"/> collection
        /// except that it excludes those items that cannot be traded on the trading post.
        /// </summary>
        /// <returns>A new <see cref="Items"/> collection that includes the contents of the current <see cref="Items"/> collection
        /// except that it excludes those items that cannot be traded on the trading post.</returns>
        public Items ExcludeNonSellable()
        {
            return new Items(this._items.Where(i => i.IsBuyable));
        }

        /// <summary>
        /// Adds a single <see cref="Item"/> to the end of the collection.
        /// </summary>
        /// <param name="item"></param>
        public void Add(Item item)
        {
            if (!this._items.Any(i => i.ID == item.ID))
                this._items.Add(item);
        }

        #region IEnumerable<Item> Support
        public IEnumerator<Item> GetEnumerator()
        {
            return this._items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion
    }
}
