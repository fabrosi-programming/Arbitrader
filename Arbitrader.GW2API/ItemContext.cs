using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Arbitrader.GW2API.Properties;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Arbitrader.GW2API.Results;
using System.Collections.ObjectModel;
using Arbitrader.GW2API.Model;
using Arbitrader.GW2API.Entities;
using System.Diagnostics;

namespace Arbitrader.GW2API
{
    /// <summary>
    /// Contains descriptive data about items and recipes obtained from the GW2 API. Handles
    /// interactions with the SQL database adn allows replacement of or appending to existing
    /// data.
    /// </summary>
    public class ItemContext : IDisposable
    {
        /// <summary>
        /// True if the item/recipe mode has been built; false if not.
        /// </summary>
        private bool _isModelBuilt = false;

        /// <summary>
        /// The interface to the GW2 API.
        /// </summary>
        private IAPI _api;

        /// <summary>
        /// The set of items contained by the context.
        /// </summary>
        internal Items Items = new Items();

        /// <summary>
        /// The entities used for maintaining persistent data in the SQL database.
        /// </summary>
        private ArbitraderEntities _entities = new ArbitraderEntities();

        /// <summary>
        /// Initializes a new instance of <see cref="ItemContext"/>.
        /// </summary>
        public ItemContext()
        {
            if (!this._entities.Loaded)
                this._entities.Load();

            this._api = new API(this._entities); //TODO: extract dependency
        }

        /// <summary>
        /// Downloads data from the GW2 API for the specified resource with the option
        /// to append to or replace existing data for that resource.
        /// </summary>
        /// <param name="resource">The API resource for which to download data.</param>
        /// <param name="replace">True if existing data is to be replace; false otherwise.</param>
        public void Load(APIResource resource, bool replace)
        {
            if (replace)
                this._entities.Delete(resource);

            switch (resource)
            {
                case APIResource.Items:
                    this._isModelBuilt = false;
                    this._api.UploadToDatabase<ItemResult, ItemEntity>(resource, this._entities.Items);
                    break;
                case APIResource.Recipes:
                    this._isModelBuilt = false;
                    this._api.UploadToDatabase<RecipeResult, RecipeEntity>(resource, this._entities.Recipes);
                    break;
                case APIResource.CommerceListings:
                    this.BuildModel();
                    var sellableItems = this.Items.ExcludeNonSellable();
                    var ids = this.Items.Select(i => i.ID);
                    this._api.UploadToDatabase<ListingResult, ListingEntity>(resource, this._entities.Listings, sellableItems);
                    sellableItems.AttachListings(this._entities.Listings);
                    break;
                default:
                    throw new ArgumentException($"Unable to load data for API resource \"{nameof(resource)}\".", nameof(resource));
            }
        }

        /// <summary>
        /// Takes the data that has been loaded to the SQL database for items and recipes and constructs
        /// model objects to represent that data and its relationships.
        /// </summary>
        public void BuildModel(bool force = false)
        {
            if (this._isModelBuilt && !force)
                return;

            this._isModelBuilt = false;

            var watchedItemIDs = this._entities.WatchedItems.Select(i => i.APIID);

            //BUG: doesn't build the model for objects more than 1 level down the crafting tree from watched items
            var watchedItems = this._entities.WatchedItems.Any()
                ? this._entities.Items.Where(i => watchedItemIDs.Contains(i.APIID))
                : this._entities.Items;

            this.Items = new Items(watchedItems);

            var before = 0;
            var after = 0;

            do
            {
                before = after;
                this.Items.AttachGeneratingRecipes(this.GetGeneratingRecipes);
                after = this.Items.Count;
            } while (before != after);

            this._isModelBuilt = true;
        }

        /// <summary>
        /// Resolves a unique identifier in the GW2 API to an instance of <see cref="Item"/>.
        /// </summary>
        /// <param name="id">The unique identifier to be resolved.</param>
        /// <returns>An instance of <see cref="Item"/> with the specified ID.</returns>
        private Item GetItem(int id)
        {
            var existingItems = this.Items.Where(i => i.ID == id);

            if (existingItems.Any())
                return existingItems.First();

            var entity = this._entities.Items.Where(i => i.APIID == id).First();
            var item = new Item(entity);
            this.Items.Add(item);

            return item;
        }

        /// <summary>
        /// Returns a collection of all <see cref="Recipe"/>s that result in the specified <see cref="Item"/> as their output.
        /// </summary>
        /// <param name="item">The item for which to get generating recipes.</param>
        /// <returns>A collection of all <see cref="Recipe"/>s that result in the specified <see cref="Item"/> as their output.</returns>
        private IEnumerable<Recipe> GetGeneratingRecipes(Item item)
        {
            var recipeEntities = this._entities.Recipes.Where(r => r.OutputItemID == item.ID);
            return recipeEntities.ToList().Select(r => new Recipe(r, this.GetItem));
        }

        /// <summary>
        /// Returns the lowest price for which a number of an item can be obtained, whether by
        /// crafting in whole or in part or by purchasing from the trading post.
        /// </summary>
        /// <param name="itemName">The name of the item to be priced.</param>
        /// <param name="count">The number of the item required.</param>
        /// <returns>The lowest price for which a number of an item can be obtained</returns>
        public int GetLowestPrice(string itemName, int count, out AcquisitionPlan plan)
        {
            this.BuildModel();

            var item = this.Items.Where(i => i.Name.Equals(itemName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            if (item == null)
                throw new InvalidOperationException($"Could not find an item with name \"{itemName}\""); //TODO: use bespoke exception type

            plan = new AcquisitionPlan();
            return item.GetLowestPrice(count, ref plan);
        }

        public int GetLowestPrice(string itemName, int count)
        {
            AcquisitionPlan plan;
            return this.GetLowestPrice(itemName, count, out plan);
        }

        #region ArbitraderEntities Pass-Through
        /// <summary>
        /// Adds items to the list of watched items when their names contain the specified text.
        /// </summary>
        /// <param name="pattern">The string pattern to compare item names against.</param>
        public void AddWatchedItems(string pattern)
        {
            this._entities.AddWatchedItems(pattern);
        }

        /// <summary>
        /// Removes an item from the list of watched items when its name exactly matches the specified text.
        /// </summary>
        /// <param name="name">The exact case-insensitive name of the item to be removed.</param>
        public void RemoveWatchedItem(string name)
        {
            this._entities.RemoveWatchedItems(name, false);
        }

        /// <summary>
        /// Removes all items from the list of watched items when their names contain the specified text.
        /// </summary>
        /// <param name="pattern">The string pattern to compare item names against.</param>
        public void RemoveWatchedItems(string pattern)
        {
            this._entities.RemoveWatchedItems(pattern);
        }

        /// <summary>
        /// Removes all items from the list of watched items.
        /// </summary>
        public void ClearWatchedItems()
        {
            this._entities.ClearWatchedItems();
        }
        #endregion

        #region IDisposable Support
        // This code added to correctly implement the disposable pattern.
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this._entities.Dispose();
                }

                disposedValue = true;
            }
        }
        
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
