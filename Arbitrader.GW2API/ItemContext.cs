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

        private API _api;

        /// <summary>
        /// The set of items contained by the context.
        /// </summary>
        internal Items Items = new Items();

        private ArbitraderEntities _entities = new ArbitraderEntities();

        public ItemContext()
        {
            if (!this._entities.Loaded)
                this._entities.Load();

            this._api = new API(this._entities);
        }

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
                this.Items.FillGeneratingRecipes(this.GetGeneratingRecipes);
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

        private IEnumerable<Recipe> GetGeneratingRecipes(Item item)
        {
            var recipeEntities = this._entities.Recipes.Where(r => r.OutputItemID == item.ID);
            return recipeEntities.ToList().Select(r => new Recipe(r, this.GetItem));
        }
        
        public int GetCheapestPrice(string itemName, int count)
        {
            this.BuildModel();

            var item = this.Items.Where(i => i.Name.Equals(itemName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            if (item == null)
                throw new InvalidOperationException($"Could not find an item with name \"{itemName}\""); //TODO: use bespoke exception type

            return item.GetBestPrice(count);
        }

        #region ArbitraderEntities Pass-Through
        public void AddWatchedItems(string pattern)
        {
            this._entities.AddWatchedItems(pattern);
        }

        public void RemoveWatchedItem(string name)
        {
            this._entities.RemoveWatchedItems(name, false);
        }

        public void RemoveWatchedItems(string pattern)
        {
            this._entities.RemoveWatchedItems(pattern);
        }

        public void ClearWatchedItems()
        {
            this._entities.ClearWatchedItems();
        }
        #endregion

        #region IDisposable Support
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
        
        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
