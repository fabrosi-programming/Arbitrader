using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Arbitrader.GW2API.Entities;
using Arbitrader.GW2API.Model;
using Arbitrader.GW2API.Properties;
using Arbitrader.GW2API.Results;

namespace Arbitrader.GW2API
{
    public class API : IAPI
    {
        /// <summary>
        /// The HTTP client used to get results from the GW2 API.
        /// </summary>
        private HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// The maximum number of times a query to the GW2 API can return a failure result before the ID
        /// being queried will be skipped.
        /// </summary>
        private int _maxRetryCount = 5;

        /// <summary>
        /// The number of records processed between occurences of <see cref="DataLoadStatusUpdate"/>.
        /// </summary>
        private int _updateInterval = 100;

        /// <summary>
        /// Determines whether errors saving records to the database will terminate further processing or
        /// if such errors will be overlooked to allow processing to continue.
        /// </summary>
        private bool _continueOnError = true;

        private ArbitraderEntities _entities;

        /// <summary>
        /// Initializes a new instance of <see cref="API"/>.
        /// </summary>
        /// <param name="updateInterval">The number of records processed between occurences of <see cref="DataLoadStatusUpdate"/>.</param>
        /// <param name="continueOnError">Determines whether errors saving records to the database will terminate further processing or
        /// if such errors will be overlooked to allow processing to continue.</param>
        public API(ArbitraderEntities entities, int updateInterval = 100, bool continueOnError = true)
        {
            this._entities = entities;

            if (!this._entities.Loaded)
                this._entities.Load();

            this.InitializeHttpClient();

            this._updateInterval = updateInterval;
            this._continueOnError = continueOnError;
        }

        /// <summary>
        /// Initializes the HTTP client that is used to send queries to and receive results from the GW2 API.
        /// </summary>
        /// <param name="client">The HTTP client used to interact with the GW2 API.</param>
        private void InitializeHttpClient()
        {
            this._httpClient.DefaultRequestHeaders.Accept.Clear();
            this._httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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
        private Entity GetSingleResult<R, E>(APIResource resource, int id)
            where R : APIDataResult<E>
            where E : Entity
        {
            var baseURL = Settings.Default.APIBaseURL;
            var listURL = $"{baseURL}/{resource.GetPath()}";
            var singleResultURL = $"{listURL}/{id}";

            var retryCount = 0;

            while (retryCount < this._maxRetryCount)
            {
                var singleResultResponse = this._httpClient.GetAsync(singleResultURL).Result;

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

        /// <summary>
        /// Gets results from the GW2 API and saves those results to the SQL database.
        /// </summary>
        /// <typeparam name="R">The result type that query results from the GW2 API are to be filtered into.</typeparam>
        /// <typeparam name="E">The entity type that is to be used to save the result data to the SQL database.</typeparam>
        /// <param name="resource">The type of resource to get data for.</param>
        /// <param name="targetDataSet">The dataset containing entities to be populated from results from the GW2 API.</param>
        /// <param name="ids">The unique identifiers in the GW2 API for the items for which data is to be retrieved.</param>
        private void UploadToDatabase<R, E>(APIResource resource, DbSet<E> targetDataSet, IEnumerable<int> ids)
            where R : APIDataResult<E>
            where E : Entity
        {
            if (ids == null)
                return;

            //this.OnDataLoadStarted(new DataLoadEventArgs(resource, ids.Count()));

            var count = 0;
            E result = null;

            foreach (var id in ids)
            {
                count += 1;
                result = this.GetSingleResult<R, E>(resource, id) as E;

                if (result != null)
                    targetDataSet.Add(result);

                if (count % _updateInterval == 0)
                {
                    this.SaveChanges(resource, () => targetDataSet.Remove(result));
                    //this.OnDataLoadStatusUpdate(new DataLoadEventArgs(resource, count));
                }
            }

            this.SaveChanges(resource, () => targetDataSet.Remove(result));
            //this.OnDataLoadFinished(new DataLoadEventArgs(resource, null));
        }

        /// <summary>
        /// Gets results from the GW2 API and saves those results to the SQL database.
        /// </summary>
        /// <typeparam name="R">The result type that query results from the GW2 API are to be filtered into.</typeparam>
        /// <typeparam name="E">The entity type that is to be used to save the result data to the SQL database.</typeparam>
        /// <param name="resource">The type of resource to get data for.</param>
        /// <param name="targetDataSet">The dataset containing entities to be populated from results from the GW2 API.</param>
        /// <param name="ids">The items in the GW2 API for which data is to be retrieved.</param>
        public void UploadToDatabase<R, E>(APIResource resource, DbSet<E> targetDataSet, IEnumerable<Item> items)
            where R : APIDataResult<E>
            where E : Entity
        {
            this.UploadToDatabase<R, E>(resource, targetDataSet, items.Select(i => i.ID));
        }

        /// <summary>
        /// Gets results from the GW2 API and saves those results to the SQL database.
        /// </summary>
        /// <typeparam name="R">The result type that query results from the GW2 API are to be filtered into.</typeparam>
        /// <typeparam name="E">The entity type that is to be used to save the result data to the SQL database.</typeparam>
        /// <param name="client">The HTTP client used to interact with the GW2 API.</param>
        /// <param name="resource">The type of resource to get data for.</param>
        /// <param name="targetDataSet">The dataset containing entities to be populated from results from the GW2 API.</param>
        public void UploadToDatabase<R, E>(APIResource resource, DbSet<E> targetDataSet)
            where R : APIDataResult<E>
            where E : Entity
        {
            var ids = this.GetIds(resource, targetDataSet);
            this.UploadToDatabase<R, E>(resource, targetDataSet, ids);
        }

        /// <summary>
        /// Returns a complete list of all possible IDs for the specified resource.
        /// </summary>
        /// <typeparam name="E">The entity type that is to be used to save the result data to the SQL database.</typeparam>
        /// <param name="client">The HTTP client used to interact with the GW2 API.</param>
        /// <param name="resource">The type of resource to get data for.</param>
        /// <param name="targetDataSet">The dataset containing entities to be populated from results from the GW2 API.</param>
        /// <returns>A complete list of all possible IDs for the specified resource.</returns>
        private IEnumerable<int> GetIds<E>(APIResource resource, DbSet<E> targetDataSet)
            where E : Entity
        {
            var baseURL = Settings.Default.APIBaseURL;
            var listURL = $"{baseURL}/{resource.GetPath()}";
            var response = this._httpClient.GetAsync(listURL).Result;

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
        /// <param name="result">The GW2 API query result containing data to be saved to the database.</param>
        private void SaveChanges(APIResource resource, Action onFailure)
        {
            try
            {
                this._entities.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                var message = $"Error saving changes for resource \"{resource}\". Exception message: {e.Message}";

                if (e.InnerException != null)
                    message += $" First sub-exception message: {e.InnerException.Message}";

                if (e.InnerException.InnerException != null)
                    message += $" Second sub-exception message: {e.InnerException.InnerException.Message}";

                message += $" : {e.StackTrace}";

                //this.OnDataLoadStatusUpdate(new DataLoadEventArgs(resource, null, message));

                onFailure?.Invoke();

                if (!this._continueOnError)
                    throw;
            }
        }
    }
}
