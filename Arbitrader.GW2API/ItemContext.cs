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
        public class DataLoadStatusUpdateEventArgs : EventArgs
        {
            public string Resource { get; set; }
            public int Count { get; set; }

            public DataLoadStatusUpdateEventArgs(string resource, int count)
            {
                this.Resource = resource;
                this.Count = count;
            }
        }

        public event EventHandler<DataLoadStatusUpdateEventArgs> RaiseDataLoadStatusUpdate;

        protected virtual void OnRaiseDataLoadStatusUpdate(DataLoadStatusUpdateEventArgs e)
        {
            RaiseDataLoadStatusUpdate?.Invoke(this, e);
        }

        private static readonly string _itemsResource = "items";
        private static readonly string _recipesResource = "recipes";

        private int _updateInterval = 100;

        public ItemContext(int updateInterval = 100)
        {
            this._updateInterval = updateInterval;
        }

        public async Task Initialize(HttpClient client, ManualResetEvent resetEvent)
        {
            this.InitializeHttpClient(client);

            var entities = new ArbitraderEntities();
            await DeleteExistingData(entities);

            await this.UploadToDatabaseAsync<Item>(client, _itemsResource, entities.Items, entities);
            await this.UploadToDatabaseAsync<Recipe>(client, _recipesResource, entities.Recipes, entities);

            resetEvent.Set();
        }

        private void InitializeHttpClient(HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private static async Task DeleteExistingData(ArbitraderEntities entities)
        {
            await entities.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[Discipline]");
            await entities.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[RecipeDiscipline]");
            await entities.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[Ingredients]");
            await entities.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[Recipe]");
            
            await entities.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[Flag]");
            await entities.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[ItemFlag]");
            await entities.Database.ExecuteSqlCommandAsync("DELETE FROM [dbo].[Item]");
        }

        private async Task UploadToDatabaseAsync<T>(HttpClient client, string resource, DbSet<T> targetDataSet, ArbitraderEntities entities) where T : class
        {
            List<int> ids = new List<int>();

            var baseURL = Settings.Default.APIBaseURL;
            var listURL = $"{baseURL}/{resource}";
            var response = await client.GetAsync(listURL);

            if (response.IsSuccessStatusCode)
                ids = await response.Content.ReadAsAsync<List<int>>();

            //#if DEBUG
            //            // limit to 100 items for testing
            //            ids = (from id in ids
            //                   where ids.IndexOf(id) < 100
            //                   select id).ToList();
            //#endif

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
                    this.OnRaiseDataLoadStatusUpdate(new DataLoadStatusUpdateEventArgs(resource, count));
                }
            }
        }
    }
}
