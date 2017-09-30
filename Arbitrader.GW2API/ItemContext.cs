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
        /// The number of records processed between occurences of <see cref="DataLoadStatusUpdate"/>.
        /// </summary>
        private int _updateInterval = 100;

        /// <summary>
        /// The maximum number of times a query to the GW2 API can return a failure result before the ID
        /// being queried will be skipped.
        /// </summary>
        private int _maxRetryCount = 1000;

        /// <summary>
        /// Determines whether errors saving records to the database will terminate further processing or
        /// if such errors will be overlooked to allow processing to continue.
        /// </summary>
        private bool _continueOnError = true;

        /// <summary>
        /// The set of recipes contained by the context.
        /// </summary>
        private List<Recipe> _recipes = new List<Recipe>();

        /// <summary>
        /// The set of items contained by the context.
        /// </summary>
        internal List<Item> Items = new List<Item>();

        /// <summary>
        /// Initializes a new instance of <see cref="ItemContext"/>.
        /// </summary>
        /// <param name="updateInterval">The number of records processed between occurences of <see cref="DataLoadStatusUpdate"/>.</param>
        /// <param name="continueOnError">Determines whether errors saving records to the database will terminate further processing or
        /// if such errors will be overlooked to allow processing to continue.</param>
        public ItemContext(int updateInterval = 100, bool continueOnError = true)
        {
            this._updateInterval = updateInterval;
            this._continueOnError = continueOnError;
        }

        /// <summary>
        /// Loads a resource from the GW2 API into the SQL database.
        /// </summary>
        /// <param name="client">The HTTP client used to get results from the GW2 API.</param>
        /// <param name="resource">The type of resource to get data for.</param>
        /// <param name="replace">Determines whether existing data in the database is overwritten or appended to.</param>
        public void Load(HttpClient client, APIResource resource, bool replace)
        {
            this.InitializeHttpClient(client);

            using (var entities = new ArbitraderEntities())
            {
                this.LoadEntities(entities);

                if (replace)
                    this.DeleteExistingData(resource, entities);

                switch (resource)
                {
                    case APIResource.Items:
                        this.UploadToDatabase<ItemResult, ItemEntity>(client, resource, entities.Items, entities);
                        break;
                    case APIResource.Recipes:
                        this.UploadToDatabase<RecipeResult, RecipeEntity>(client, resource, entities.Recipes, entities);
                        break;
                    case APIResource.CommerceListings:
                        var ids = entities.WatchedItems.Select(i => i.APIID).ToList();
                        
                        // default to everything if the WatchedItems table is empty
                        if (ids.Count == 0)
                            ids = entities.Items.Select(i => i.APIID).ToList();

                        this.BuildModel();
                        ids = this.IncludeIngredientTree(ids);
                        ids = this.ExcludeNonSellableIds(ids);
                        this.UploadToDatabase<ListingResult, ListingEntity>(client, resource, entities.Listings, entities, ids);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Takes the data that has been loaded to the SQL database for items and recipes and constructs
        /// model objects to represent that data and its relationships.
        /// </summary>
        public void BuildModel()
        {
            using (var entities = new ArbitraderEntities())
            {
                this.LoadEntities(entities);

                this.Items = new List<Item>();
                this._recipes = new List<Recipe>();

                foreach (var entity in entities.Items)
                    this.Items.Add(new Item(entity));

                foreach (var entity in entities.Recipes)
                    this._recipes.Add(new Recipe(entity, this));
            }
        }

        /// <summary>
        /// Adds all items whose names contain the given pattern to the list of watched items.
        /// </summary>
        /// <param name="pattern">The string pattern to search for.</param>
        public void AddWatchedItems(string pattern)
        {
            var entities = new ArbitraderEntities();

            if (!entities.Items.Any())
                throw new InvalidOperationException("Unable to add watched items before items have been loaded into the database.");

            var existingWatchedIDs = entities.WatchedItems.Select(i => i.APIID);
            var newWatchItems = entities.Items.Where(i => i.Name.ToUpper().Contains(pattern.ToUpper()))
                                              .Where(i => !existingWatchedIDs.Contains(i.ID));

            foreach (var item in newWatchItems)
                entities.WatchedItems.Add(new WatchedItem(item.APIID));

            entities.SaveChanges();
        }

        /// <summary>
        /// Clears the entire list of watched items.
        /// </summary>
        public void ClearWatchedItems()
        {
            var entities = new ArbitraderEntities();
            entities.WatchedItems.RemoveRange(entities.WatchedItems);
            entities.SaveChanges();
        }

        /// <summary>
        /// Removes items from the list of watched items by checking their names against the given string pattern.
        /// </summary>
        /// <param name="pattern">The string pattern to search for.</param>
        /// <param name="substring">If true, the pattern may match only a substring of the names of items
        /// to be removed. If false, it must match the entire name.</param>
        public void RemoveWatchedItem(string pattern, bool substring = true)
        {
            var entities = new ArbitraderEntities();

            if (!entities.Items.Any())
                throw new InvalidOperationException("Unable to remove watched items before items have been loaded into the database.");

            IEnumerable<int> matchingIDs;

            if (substring)
                matchingIDs = entities.Items.Where(i => i.Name.ToUpper().Contains(pattern.ToUpper()))
                                            .Select(i => i.APIID);
            else
                matchingIDs = entities.Items.Where(i => String.Compare(i.Name, pattern, true) == 0)
                                            .Select(i => i.APIID);

            var watchItemsToRemove = entities.WatchedItems.Where(i => matchingIDs.Contains(i.APIID));
            entities.WatchedItems.RemoveRange(watchItemsToRemove);
            entities.SaveChanges();
        }

        /// <summary>
        /// Loads each set of entities from the SQL database.
        /// </summary>
        /// <param name="entities">An interface for item, recipe, and market data stored in the Arbitrader SQL database.</param>
        private void LoadEntities(ArbitraderEntities entities)
        {
            entities.Items.Load();
            entities.ItemFlags.Load();

            entities.Disciplines.Load();
            entities.GuildIngredients.Load();
            entities.Ingredients.Load();
            entities.Recipes.Load();
            entities.RecipeFlags.Load();

            entities.WatchedItems.Load();
            entities.Listings.Load();
            entities.IndividualListings.Load();
        }

        /// <summary>
        /// Initializes the HTTP client that is used to send queries to and receive results from the GW2 API.
        /// </summary>
        /// <param name="client">The HTTP client used to interact with the GW2 API.</param>
        private void InitializeHttpClient(HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Deletes existing data from the SQL database. Respects foreign key relationships in that data.
        /// </summary>
        /// <param name="resource">The resource for which data is to be deleted.</param>
        /// <param name="entities">An interface for item, recipe, and market data stored in the Arbitrader SQL database.</param>
        private void DeleteExistingData(APIResource resource, ArbitraderEntities entities)
        {
            if (resource == APIResource.Recipes || resource == APIResource.Items)
            {
                entities.Disciplines.RemoveRange(entities.Disciplines);
                entities.RecipeFlags.RemoveRange(entities.RecipeFlags);
                entities.Ingredients.RemoveRange(entities.Ingredients);
                entities.GuildIngredients.RemoveRange(entities.GuildIngredients);
                entities.Recipes.RemoveRange(entities.Recipes);
            }

            if (resource == APIResource.Items)
            {
                entities.ItemFlags.RemoveRange(entities.ItemFlags);
                entities.Items.RemoveRange(entities.Items);
            }

            entities.SaveChanges();
        }

        /// <summary>
        /// Gets results from the GW2 API and saves those results to the SQL database.
        /// </summary>
        /// <typeparam name="R">The result type that query results from the GW2 API are to be filtered into.</typeparam>
        /// <typeparam name="E">The entity type that is to be used to save the result data to the SQL database.</typeparam>
        /// <param name="client">The HTTP client used to interact with the GW2 API.</param>
        /// <param name="resource">The type of resource to get data for.</param>
        /// <param name="targetDataSet">The dataset containing entities to be populated from results from the GW2 API.</param>
        /// <param name="entities">An interface for item, recipe, and market data stored in the Arbitrader SQL database.</param>
        private void UploadToDatabase<R, E>(HttpClient client, APIResource resource, DbSet<E> targetDataSet, ArbitraderEntities entities)
            where R : APIDataResult
            where E : Entity
        {
            var ids = GetIds(client, resource, targetDataSet);
            this.UploadToDatabase<R, E>(client, resource, targetDataSet, entities, ids);
        }

        /// <summary>
        /// Gets results from the GW2 API and saves those results to the SQL database.
        /// </summary>
        /// <typeparam name="R">The result type that query results from the GW2 API are to be filtered into.</typeparam>
        /// <typeparam name="E">The entity type that is to be used to save the result data to the SQL database.</typeparam>
        /// <param name="client">The HTTP client used to interact with the GW2 API.</param>
        /// <param name="resource">The type of resource to get data for.</param>
        /// <param name="targetDataSet">The dataset containing entities to be populated from results from the GW2 API.</param>
        /// <param name="entities">An interface for item, recipe, and market data stored in the Arbitrader SQL database.</param>
        /// <param name="ids">The unique identifiers in the GW2 API for the items for which data is to be retrieved.</param>
        private void UploadToDatabase<R, E>(HttpClient client, APIResource resource, DbSet<E> targetDataSet, ArbitraderEntities entities, IEnumerable<int> ids)
            where R : APIDataResult
            where E : Entity
        {
            if (ids == null)
                return;

            this.OnDataLoadStarted(new DataLoadEventArgs(resource, ids.Count()));

            var count = 0;
            E result = null;

            foreach (var id in ids)
            {
                count += 1;
                result = this.GetSingleResult<R, E>(client, resource, id) as E;

                if (result != null)
                    targetDataSet.Add(result);

                if (count % _updateInterval == 0)
                {
                    this.SaveChanges(resource, targetDataSet, entities, result);
                    this.OnDataLoadStatusUpdate(new DataLoadEventArgs(resource, count));
                }
            }

            this.SaveChanges(resource, targetDataSet, entities, result);
            this.OnDataLoadFinished(new DataLoadEventArgs(resource, null));
        }

        /// <summary>
        /// Given a set of item IDs, includes all item IDs for possible ingredients to those items. Recursively
        /// gets ids all the way to raw materials
        /// </summary>
        /// <param name="ids">The set of item IDs to be appended to.</param>
        private List<int> IncludeIngredientTree(List<int> ids)
        {
            var items = this.Items.Where(i => ids.Contains(i.ID)).ToList();
            var subItems = new List<Item>();

            foreach (var item in items)
                foreach (var recipe in item.GeneratingRecipes)
                    subItems.AddRange(recipe.IngredientTreeNodes);

            items.AddRange(subItems.Except(items).Distinct());

            return items.Select(i => i.ID).ToList();
        }

        /// <summary>
        /// Given a set of item IDs, excludes those IDs that are for items that cannot be traded on the trading post.
        /// </summary>
        /// <param name="ids">The unique identifiers in the GW2 API of the items to be filtered.</param>
        /// <returns>The original list of unique identifiers except those that are for items that cannot be traded on
        /// the trading post.</returns>
        private List<int> ExcludeNonSellableIds(List<int> ids)
        {
            var items = this.Items.Where(i => ids.Contains(i.ID))
                                  .Where(i => !i.Flags.Contains(Flag.NoSell))
                                  .Where(i => !i.Flags.Contains(Flag.AccountBound))
                                  .Where(i => !i.Flags.Contains(Flag.MonsterOnly))
                                  .Where(i => !i.Flags.Contains(Flag.SoulbindOnAcquire));
            return items.Select(i => i.ID)
                        .ToList();
        }

        /// <summary>
        /// Returns a complete list of all possible IDs for the specified resource.
        /// </summary>
        /// <typeparam name="E">The entity type that is to be used to save the result data to the SQL database.</typeparam>
        /// <param name="client">The HTTP client used to interact with the GW2 API.</param>
        /// <param name="resource">The type of resource to get data for.</param>
        /// <param name="targetDataSet">The dataset containing entities to be populated from results from the GW2 API.</param>
        /// <returns>A complete list of all possible IDs for the specified resource.</returns>
        private static IEnumerable<int> GetIds<E>(HttpClient client, APIResource resource, DbSet<E> targetDataSet)
            where E : Entity
        {
            var baseURL = Settings.Default.APIBaseURL;
            var listURL = $"{baseURL}/{resource.GetPath()}";
            var response = client.GetAsync(listURL).Result;

            if (!response.IsSuccessStatusCode)
                return null;

            var queryableDbSet = (IQueryable<Entity>)targetDataSet;
            var existingIds = queryableDbSet.Select(row => row.APIID)
                                            .OrderBy(id => id)
                                            .ToList();
            var newIds = response.Content.ReadAsAsync<List<int>>().Result;            
            return newIds.Except(existingIds);
        }

        /// <summary>
        /// Saves a set of entity changes to the SQL database.
        /// </summary>
        /// <typeparam name="E">The entity type that is to be used to save the result data to the SQL database.</typeparam>
        /// <param name="resource">The type of resource to get data for.</param>
        /// <param name="targetDataSet">The dataset containing entities to be populated from results from the GW2 API.</param>
        /// <param name="entities">An interface for item, recipe, and market data stored in the Arbitrader SQL database.</param>
        /// <param name="result">The GW2 API query result containing data to be saved to the database.</param>
        private void SaveChanges<E>(APIResource resource, DbSet<E> targetDataSet, ArbitraderEntities entities, E result)
            where E : Entity
        {
            try
            {
                entities.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                var message = $"Error saving changes for resource \"{resource}\". Exception message: {e.Message}";

                if (e.InnerException != null)
                    message += $" First sub-exception message: {e.InnerException.Message}";

                if (e.InnerException.InnerException != null)
                    message += $" Second sub-exception message: {e.InnerException.InnerException.Message}";

                message += $" : {e.StackTrace}";

                this.OnDataLoadStatusUpdate(new DataLoadEventArgs(resource, null, message));

                if (result != null)
                    targetDataSet.Remove(result);

                if (!this._continueOnError)
                    throw;
            }
        }

        /// <summary>
        /// Returns a result for a single ID from the GW2 API for the specified resource.
        /// </summary>
        /// <typeparam name="R">The result type that query results from the GW2 API are to be filtered into.</typeparam>
        /// <typeparam name="E">The entity type that is to be used to save the result data to the SQL database.</typeparam>
        /// <param name="client">The HTTP client used to interact with the GW2 API.</param>
        /// <param name="resource">The type of resource to get data for.</param>
        /// <param name="id">The ID of the result to be retrieved.</param>
        /// <returns>A result for a single ID from the GW2 API for the specified resource.</returns>
        private Entity GetSingleResult<R, E>(HttpClient client, APIResource resource, int id)
            where R : APIDataResult
        {
            var baseURL = Settings.Default.APIBaseURL;
            var listURL = $"{baseURL}/{resource.GetPath()}";
            var singleResultURL = $"{listURL}/{id}";

            var retryCount = 0;

            while (retryCount < this._maxRetryCount)
            {
                var singleResultResponse = client.GetAsync(singleResultURL).Result;

                if (singleResultResponse.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"Resource : {resource} | ID : {id} | Retry Count : {retryCount}");
                    return singleResultResponse.Content.ReadAsAsync<R>().Result.ToEntity();
                }

                if (singleResultResponse.ReasonPhrase == "Not Found")
                    return null;

                retryCount += 1;
            }

            return null;
        }
    }
}