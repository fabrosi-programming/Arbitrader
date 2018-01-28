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
        private List<Item> _items = new List<Item>();

        public int Count
        {
            get
            {
                return this._items.Count;
            }
        }

        public IEnumerable<Recipe> GeneratingRecipes
        {
            get
            {
                return this._items.SelectMany(i => i.GeneratingRecipes);
            }
        }

        public Items()
        { }

        public Items(IEnumerable<Item> items)
        {
            foreach (var item in items)
                this._items.Add(item);
        }

        public Items(IEnumerable<ItemEntity> entities)
            : this(entities.Select(e => new Item(e)))
        { }

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
        /// Returns a new <see cref="Items"/> collection that includes the contents of the current <see cref="Items"/> collection
        /// except that it excludes those items that cannot be traded on the trading post.
        /// </summary>
        /// <returns>A new <see cref="Items"/> collection that includes the contents of the current <see cref="Items"/> collection
        /// except that it excludes those items that cannot be traded on the trading post.</returns>
        public Items ExcludeNonSellable()
        {
            return new Items(this._items.Where(i => i.IsSellable));
        }

        public void Add(Item item)
        {
            if (!this._items.Any(i => i.ID == item.ID))
                this._items.Add(item);
        }

        public void Add(IEnumerable<Item> items)
        {
            foreach (var item in items)
                this.Add(item);
        }

        public void Add(ItemEntity entity)
        {
            this.Add(new Item(entity));
        }

        public void Add(IEnumerable<ItemEntity> entities)
        {
            foreach (var entity in entities)
                this.Add(entity);
        }

        public bool ContainsAllIngredients(IEnumerable<Recipe> recipes)
        {
            var ingredients = recipes.SelectMany(r => r.Ingredients);
            return ingredients.All(i => this._items.Contains(i.Key));
        }

        public void FillGeneratingRecipes(Func<Item, IEnumerable<Recipe>> getGeneratingRecipes)
        {
            var items = this._items.Where(i => i.GeneratingRecipes?.Count == 0).ToList();

            foreach (var item in items)
                item.GeneratingRecipes = getGeneratingRecipes(item).ToList();
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
