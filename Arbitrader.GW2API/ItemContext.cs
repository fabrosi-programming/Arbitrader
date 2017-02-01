using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;

namespace Arbitrader.GW2API
{
    public class ItemContext
    {
        private List<Item> items;
        private List<Recipe> recipes;

        private static readonly string itemsResource = "items";
        private static readonly string recipesResource = "recipes";

        public async Task Initialize(HttpClient client, ManualResetEvent resetEvent)
        {
            this.InitializeHttpClient(client);

            //TODO: parallelize
            await this.GetItemsAsync(client);
            await this.GetRecipesAsync(client);

            resetEvent.Set();
        }

        private void InitializeHttpClient(HttpClient client)
        {
            //client.BaseAddress = new Uri(Properties.Settings.Default.APIBaseURL);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private async Task GetItemsAsync(HttpClient client)
        {
            var itemIDs = new List<int>();

            var baseURL = Properties.Settings.Default.APIBaseURL;
            var itemsURL = $"{baseURL}/{itemsResource}";
            var itemsResponse = await client.GetAsync(itemsURL);

            if (itemsResponse.IsSuccessStatusCode)
                itemIDs = await itemsResponse.Content.ReadAsAsync<List<int>>();

            foreach (var id in itemIDs)
            {
                var singleItemURL = $"{itemsURL}/{id}";
                var singleItemResponse = await client.GetAsync(singleItemURL);

                if (singleItemResponse.IsSuccessStatusCode)
                {
                    var item = await singleItemResponse.Content.ReadAsAsync<Item>();
                    this.items.Add(item);
                }
            }
        }

        public async Task GetRecipesAsync(HttpClient client)
        {
            var baseURL = Properties.Settings.Default.APIBaseURL;
            var recipesURL = $"{baseURL}/{recipesResource}";
            var recipesRequest = WebRequest.Create(recipesURL);
            recipesRequest.Method = "GET";
            var recipesResponse = recipesRequest.GetResponse();
        }
    }
}
