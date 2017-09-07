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

namespace Arbitrader.GW2API
{
    public class ItemContext
    {
        #region Events
        public class DataLoadEventArgs : EventArgs
        {
            public Resource Resource { get; set; }
            public int? Count { get; set; }
            public string Message { get; set; }

            public DataLoadEventArgs(Resource resource, int? count = null, string message = null)
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

        private int _updateInterval = 100;
        private int _maxRetryCount = 1000;
        private bool _continueOnError = true;

        public Collection<Item> Items = new Collection<Item>();
        public Collection<Recipe> Recipes = new Collection<Recipe>();

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
                this.DeleteExistingData(resource, entities);

            switch (resource)
            {
                case Resource.Items:
                    this.UploadToDatabase<ItemResult, ItemEntity>(client, resource, entities.Items, entities);
                    break;
                case Resource.Recipes:
                    this.UploadToDatabase<RecipeResult, RecipeEntity>(client, resource, entities.Recipes, entities);
                    break;
                default:
                    break;
            }

            this.LoadEntities(entities);

            this.Items = new Collection<Item>();
            this.Recipes = new Collection<Recipe>();

            foreach (var entity in entities.Items)
                this.Items.Add(new Item(entity));

            foreach (var entity in entities.Recipes)
                this.Recipes.Add(new Recipe(entity, this));
        }

        private void LoadEntities(ArbitraderEntities entities)
        {
            entities.Disciplines.Load();
            entities.GuildIngredients.Load();
            entities.Ingredients.Load();
            entities.Items.Load();
            entities.ItemFlags.Load();
            entities.Recipes.Load();
            entities.RecipeFlags.Load();
        }

        private void InitializeHttpClient(HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private void DeleteExistingData(Resource resource, ArbitraderEntities entities)
        {
            // always clear out recipes due to foreign key relationships with items
            entities.Disciplines.RemoveRange(entities.Disciplines);
            entities.RecipeFlags.RemoveRange(entities.RecipeFlags);
            entities.Ingredients.RemoveRange(entities.Ingredients);
            entities.GuildIngredients.RemoveRange(entities.GuildIngredients);
            entities.Recipes.RemoveRange(entities.Recipes);

            if (resource == Resource.Items)
            {
                entities.ItemFlags.RemoveRange(entities.ItemFlags);
                entities.Items.RemoveRange(entities.Items);
            }
            
            entities.SaveChanges();
        }

        private void UploadToDatabase<R, E>(HttpClient client, Resource resource, DbSet<E> targetDataSet, ArbitraderEntities entities)
            where R : APIDataResult
            where E : Entity
        {
            var ids = GetIds(client, resource, targetDataSet);

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

        private static IEnumerable<int> GetIds<E>(HttpClient client, Resource resource, DbSet<E> targetDataSet)
            where E : Entity
        {
            var baseURL = Settings.Default.APIBaseURL;
            var listURL = $"{baseURL}/{resource}";
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

        private void SaveChanges<E>(Resource resource, DbSet<E> targetDataSet, ArbitraderEntities entities, E result)
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

        private Entity GetSingleResult<R, E>(HttpClient client, Resource resource, int id)
            where R : APIDataResult
        {
            var baseURL = Settings.Default.APIBaseURL;
            var listURL = $"{baseURL}/{resource}";
            var singleResultURL = $"{listURL}/{id}";

            var retryCount = 1;

            while (retryCount <= this._maxRetryCount)
            {
                var singleResultResponse = client.GetAsync(singleResultURL).Result;

                if (singleResultResponse.IsSuccessStatusCode)
                {
                    return singleResultResponse.Content.ReadAsAsync<R>().Result.ToEntity();
                }

                retryCount += 1;
            }

            return null;
        }
    }
}