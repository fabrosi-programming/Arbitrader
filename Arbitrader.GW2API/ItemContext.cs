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

namespace Arbitrader.GW2API
{
    public class ItemContext
    {
        #region Events
        public class DataLoadEventArgs : EventArgs
        {
            public string Resource { get; set; }
            public int? Count { get; set; }
            public string Message { get; set; }

            public DataLoadEventArgs(string resource, int? count = null, string message = null)
            {
                this.Resource = resource;
                this.Count = count;
                this.Message = message;
            }
        }

        public event EventHandler<DataLoadEventArgs> DataLoadStarted;
        public event EventHandler<DataLoadEventArgs> DataLoadFinished;
        public event EventHandler<DataLoadEventArgs> DataLoadStatusUpdate;

        protected virtual void OnDataLoadStarted(DataLoadEventArgs e)
        {
            DataLoadStarted?.Invoke(this, e);
        }

        protected virtual void OnDataLoadFinished(DataLoadEventArgs e)
        {
            DataLoadFinished?.Invoke(this, e);
        }

        protected virtual void OnDataLoadStatusUpdate(DataLoadEventArgs e)
        {
            DataLoadStatusUpdate?.Invoke(this, e);
        }
        #endregion

        public enum Resource
        {
            Items,
            Recipes
        }

        private static readonly string _itemsResource = "items";
        private static readonly string _recipesResource = "recipes";

        private int _updateInterval = 100;
        private bool _continueOnError = true;

        public ItemContext(int updateInterval = 100, bool continueOnError = true)
        {
            this._updateInterval = updateInterval;
            this._continueOnError = continueOnError;
        }

        public void Load(HttpClient client, Resource resource, bool replace)
        {
            this.InitializeHttpClient(client);

            var entities = new ArbitraderEntities();

            if (replace)
                this.DeleteExistingData(entities);

            switch (resource)
            {
                case Resource.Items:
                    this.UploadToDatabase<Item>(client, _itemsResource, entities.Items, entities);
                    break;
                case Resource.Recipes:
                    this.UploadToDatabase<Recipe>(client, _recipesResource, entities.Recipes, entities);
                    break;
                default:
                    break;
            }
        }

        private void InitializeHttpClient(HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private void DeleteExistingData(ArbitraderEntities entities)
        {
            var tableNames = new Dictionary<int, string>()
            {
                { 0, "Flag" },
                { 1, "ItemFlag" },
                { 2, "Item" },
                { 3, "Discipline" },
                { 4, "RecipeDiscipline" },
                { 5, "Ingredients" },
                { 6, "Recipe" }
            };

            foreach (var table in tableNames.OrderBy(kvp => kvp.Key))
                entities.Database.ExecuteSqlCommand($"DELETE FROM [dbo].[{table.Value}]");
        }

        private void UploadToDatabase<T>(HttpClient client, string resource, DbSet<T> targetDataSet, ArbitraderEntities entities) where T : class
        {
            var baseURL = Settings.Default.APIBaseURL;
            var listURL = $"{baseURL}/{resource}";
            var response = client.GetAsync(listURL).Result;

            if (!response.IsSuccessStatusCode)
                return;

            var queryableDbSet = (IQueryable<ICanHazID>)targetDataSet;
            var existingIds = (from row in queryableDbSet
                               select row.id).ToList();
            var unfilteredIds = response.Content.ReadAsAsync<List<int>>().Result;
            var ids = from id in unfilteredIds
                      where !existingIds.Contains(id)
                      select id;

            this.OnDataLoadStarted(new DataLoadEventArgs(resource, ids.Count()));

            var count = 0;
            T result = null;

            foreach (var id in ids)
            {
                count += 1;

                var singleResultURL = $"{listURL}/{id}";
                var singleResultResponse = client.GetAsync(singleResultURL).Result;

                if (singleResultResponse.IsSuccessStatusCode)
                {
                    result = singleResultResponse.Content.ReadAsAsync<T>().Result;
                    targetDataSet.Add(result);
                }

                if (count % _updateInterval == 0)
                {
                    this.SaveChanges(resource, targetDataSet, entities, result);
                    
                    this.OnDataLoadStatusUpdate(new DataLoadEventArgs(resource, count));
                }
            }

            this.SaveChanges(resource, targetDataSet, entities, result);

            this.OnDataLoadFinished(new DataLoadEventArgs(resource, null));
        }

        private void SaveChanges<T>(string resource, DbSet<T> targetDataSet, ArbitraderEntities entities, T result) where T : class
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
    }
}
