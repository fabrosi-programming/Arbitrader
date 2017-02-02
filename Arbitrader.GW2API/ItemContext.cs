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

namespace Arbitrader.GW2API
{
    public class ItemContext
    {
        private List<Item> _items;
        private List<Recipe> _recipes;

        private static readonly string _itemsResource = "items";
        private static readonly string _recipesResource = "recipes";

        public ItemContext()
        {
            this._items = new List<Item>();
            this._recipes = new List<Recipe>();
        }

        public async Task Initialize(HttpClient client, ManualResetEvent resetEvent)
        {
            this.InitializeHttpClient(client);

            //TODO: parallelize
            await this.GetAsync<Item>(client, _itemsResource, this._items);
            await this.GetAsync<Recipe>(client, _recipesResource, this._recipes);

            resetEvent.Set();
        }

        private void InitializeHttpClient(HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private async Task GetAsync<T>(HttpClient client, string resource, List<T> targetList)
        {
            List<int> ids = new List<int>();

            var baseURL = Settings.Default.APIBaseURL;
            var listURL = $"{baseURL}/{resource}";
            var response = await client.GetAsync(listURL);

            if (response.IsSuccessStatusCode)
                ids = await response.Content.ReadAsAsync<List<int>>();

#if DEBUG
            // limit to 100 items for testing
            ids = (from id in ids
                   where ids.IndexOf(id) < 100
                   select id).ToList();
#endif

            foreach (var id in ids)
            {
                var singleResultURL = $"{listURL}/{id}";
                var singleResultResponse = await client.GetAsync(singleResultURL);

                if (singleResultResponse.IsSuccessStatusCode)
                {
                    var result = await singleResultResponse.Content.ReadAsAsync<T>();
                    targetList.Add(result);
                }
            }

        }
    }
}
