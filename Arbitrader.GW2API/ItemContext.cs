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
    public class ItemContext
    {
        #region Events
        /// <summary>
        /// Contains data for events raised at points throughout the data load process.
        /// </summary>
        public class DataLoadEventArgs : EventArgs
        {
            /// <summary>
            /// The GW2 API resource for which data is being loaded.
            /// </summary>
            public APIResource Resource { get; set; }

            /// <summary>
            /// The number of records affected since the last data load status update.
            /// </summary>
            public int? Count { get; set; }

            /// <summary>
            /// A message raised during the data load.
            /// </summary>
            public string Message { get; set; }

            /// <summary>
            /// Initializes a new instance of <see cref="DataLoadEventArgs"/>.
            /// </summary>
            /// <param name="resource">The GW2 API resource for which data is being loaded.</param>
            /// <param name="count">The number of records affected since the last data load status update.</param>
            /// <param name="message">A message raised during the data load.</param>
            public DataLoadEventArgs(APIResource resource, int? count = null, string message = null)
            {
                this.Resource = resource;
                this.Count = count;
                this.Message = message;
            }
        }

        /// <summary>
        /// Occurs when a data load has started.
        /// </summary>
        public event EventHandler<DataLoadEventArgs> DataLoadStarted;

        /// <summary>
        /// Occurs when a data load has finished.
        /// </summary>
        public event EventHandler<DataLoadEventArgs> DataLoadFinished;

        /// <summary>
        /// Occurs when a data load has a status update to report.
        /// </summary>
        public event EventHandler<DataLoadEventArgs> DataLoadStatusUpdate;

        /// <summary>
        /// Invokes any event handlers registered for <see cref="DataLoadStarted"/>.
        /// </summary>
        /// <param name="e">The arguments passed to the event handlers</param>
        protected virtual void OnDataLoadStarted(DataLoadEventArgs e)
        {
            DataLoadStarted?.Invoke(this, e);
        }

        /// <summary>
        /// Invokes any event handlers registered for <see cref="DataLoadFinished"/>.
        /// </summary>
        /// <param name="e">The arguments passed to the event handlers.</param>
        protected virtual void OnDataLoadFinished(DataLoadEventArgs e)
        {
            DataLoadFinished?.Invoke(this, e);
        }

        /// <summary>
        /// Invokes any event handlers registered for <see cref="DataLoadStatusUpdate"/>.
        /// </summary>
        /// <param name="e">The argumnets passed to the event handlers.</param>
        protected virtual void OnDataLoadStatusUpdate(DataLoadEventArgs e)
        {
            DataLoadStatusUpdate?.Invoke(this, e);
        }
        #endregion

        /// <summary>
        /// True if the item/recipe mode has been built; false if not.
        /// </summary>
        private bool _isModelBuilt = false;

        /// <summary>
        /// The set of recipes contained by the context.
        /// </summary>
        private List<Recipe> _recipes = new List<Recipe>();

        /// <summary>
        /// The set of items contained by the context.
        /// </summary>
        internal Items Items = new Items();

        private API _api = new API();

        internal List<Item> WatchedItems
        {
            get
            {
                var entities = new ArbitraderEntities();
                var watchedItemIDs = entities.WatchedItems.Select(i => i.APIID);
                return this.Items.Where(i => watchedItemIDs.Contains(i.ID)).ToList();
            }
        }

        public void Load(APIResource resource, bool replace)
        {
            var entities = new ArbitraderEntities();
            entities.Load();

            if (replace)
                entities.Delete(resource);

            switch (resource)
            {
                case APIResource.Items:
                    this._isModelBuilt = false;
                    this._api.UploadToDatabase<ItemResult, ItemEntity>(resource, entities.Items);
                    break;
                case APIResource.Recipes:
                    this._isModelBuilt = false;
                    this._api.UploadToDatabase<RecipeResult, RecipeEntity>(resource, entities.Recipes);
                    break;
                case APIResource.CommerceListings:
                    this.BuildModel(entities);
                    this.Items = this.Items.ExcludeNonSellable();
                    var ids = this.Items.Select(i => i.ID);
                    this._api.UploadToDatabase<ListingResult, ListingEntity>(resource, entities.Listings, ids);
                    this.Items.AttachListings(entities.Listings);
                    break;
                default:
                    throw new ArgumentException($"Unable to load data for API resource \"{nameof(resource)}\".", nameof(resource));
            }
        }

        /// <summary>
        /// Takes the data that has been loaded to the SQL database for items and recipes and constructs
        /// model objects to represent that data and its relationships.
        /// </summary>
        public void BuildModel(ArbitraderEntities entities, bool force = false)
        {
            if (this._isModelBuilt && !force)
                return;

            this._isModelBuilt = false;

            var watchedItemIDs = entities.WatchedItems.Select(i => i.APIID);

            //BUG: doesn't build the model for objects more than 1 level down the crafting tree from watched items
            IEnumerable<ItemEntity> watchedItems;
            IEnumerable<RecipeEntity> watchedRecipes;

            if (entities.WatchedItems.Any())
            {
                
                watchedItems = entities.Items.Where(i => watchedItemIDs.Contains(i.APIID));
                watchedRecipes = entities.Recipes.Where(r => r.OutputItemID.HasValue && watchedItemIDs.Contains(r.OutputItemID.Value));
            }
            else
            {
                watchedItems = entities.Items;
                watchedRecipes = entities.Recipes;
            }

            
            this._recipes = new List<Recipe>();

            foreach (var entity in watchedItems)
                if (!this.Items.Select(i => i.ID).Contains(entity.APIID))
                    this.Items.Add(new Item(entity));

            foreach (var entity in watchedRecipes)
                if (!this._recipes.Select(r => r.ID).Contains(entity.APIID))
                    this._recipes.Add(new Recipe(entity, this.GetItem));

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

            var entities = new ArbitraderEntities();

            var entity = entities.Items.Where(i => i.APIID == id).First();
            this.Items.Add(new Item(entity));

            return existingItems.First();
        }

        public int GetCheapestPrice(string itemName, int count)
        {
            this.BuildModel(new ArbitraderEntities());

            var item = this.Items.Where(i => i.Name.Equals(itemName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            if (item == null)
                throw new InvalidOperationException($"Could not find an item with name \"{itemName}\""); //TODO: use bespoke exception type

            return item.GetBestPrice(count);
        }

        #region ArbitraderEntities Pass-Through
        public void AddWatchedItems(string pattern)
        {
            var entities = new ArbitraderEntities();
            entities.AddWatchedItems(pattern);
        }

        public void RemoveWatchedItem(string name)
        {
            var entities = new ArbitraderEntities();
            entities.RemoveWatchedItems(name, false);
        }

        public void RemoveWatchedItems(string pattern)
        {
            var entities = new ArbitraderEntities();
            entities.RemoveWatchedItems(pattern);
        }

        public void ClearWatchedItems()
        {
            var entities = new ArbitraderEntities();
            entities.ClearWatchedItems();
        }
        #endregion
    }
}
