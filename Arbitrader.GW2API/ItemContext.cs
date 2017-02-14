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

namespace Arbitrader.GW2API
{
    public class ItemContext
    {
        #region Events
        public class DataLoadEventArgs : EventArgs
        {
            public string Resource { get; set; }
            public int? Count { get; set; }

            public DataLoadEventArgs(string resource, int? count)
            {
                this.Resource = resource;
                this.Count = count;
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

        private int _updateInterval = 1000;

        public ItemContext(int updateInterval = 1000)
        {
            this._updateInterval = updateInterval;
        }

        public async Task InitializeAsync(HttpClient client, Resource resource)
        {
            this.InitializeHttpClient(client);

            var entities = new ArbitraderEntities();
            await DeleteExistingDataAsync(entities);

            switch (resource)
            {
                case Resource.Items:
                    await this.UploadToDatabaseAsync<Item>(client, _itemsResource, entities.Items, entities);
                    break;
                case Resource.Recipes:
                    await this.UploadToDatabaseAsync<Recipe>(client, _recipesResource, entities.Recipes, entities);
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

        private async Task DeleteExistingDataAsync(ArbitraderEntities entities)
        {
            await entities.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[Flag]");
            await entities.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[ItemFlag]");
            await entities.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[Item]");
            
            await entities.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[Discipline]");
            await entities.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[RecipeDiscipline]");
            await entities.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[Ingredients]");
            await entities.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[Recipe]");
        }

        private async Task UploadToDatabaseAsync<T>(HttpClient client, string resource, DbSet<T> targetDataSet, ArbitraderEntities entities) where T : class
        {
            List<int> ids = new List<int>();

            var baseURL = Settings.Default.APIBaseURL;
            var listURL = $"{baseURL}/{resource}";
            var response = await client.GetAsync(listURL);

            if (response.IsSuccessStatusCode)
                ids = await response.Content.ReadAsAsync<List<int>>();

            
            this.OnDataLoadStarted(new DataLoadEventArgs(resource, ids.Count));

#if DEBUG
            // limit to 100 items for testing
            ids = (from id in ids
                   where ids.IndexOf(id) < 100
                   select id).ToList();
#endif

            var count = 0;

            foreach (var id in ids)
            {
                count += 1;

                var singleResultURL = $"{listURL}/{id}";
                var singleResultResponse = await client.GetAsync(singleResultURL);

                if (singleResultResponse.IsSuccessStatusCode)
                {
                    var result = await singleResultResponse.Content.ReadAsAsync<T>();
                    targetDataSet.Add(result);
                }

                if (count % _updateInterval == 0)
                {
                    await Task.Run(() => entities.SaveChanges());
                    this.OnDataLoadStatusUpdate(new DataLoadEventArgs(resource, count));
                }
            }

            await Task.Run(() => entities.SaveChanges());

            this.OnDataLoadFinished(new DataLoadEventArgs(resource, null));
        }
    }
}
