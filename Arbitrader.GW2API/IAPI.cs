using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arbitrader.GW2API.Entities;
using Arbitrader.GW2API.Model;
using Arbitrader.GW2API.Results;

namespace Arbitrader.GW2API
{
    /// <summary>
    /// Exposes the GW2 web API.
    /// </summary>
    interface IAPI
    {
        /// <summary>
        /// Gets results from the GW2 API and saves those results to the SQL database.
        /// </summary>
        /// <typeparam name="R">The result type that query results from the GW2 API are to be filtered into.</typeparam>
        /// <typeparam name="E">The entity type that is to be used to save the result data to the SQL database.</typeparam>
        /// <param name="resource">The type of resource to get data for.</param>
        /// <param name="targetDataSet">The dataset containing entities to be populated from results from the GW2 API.</param>
        /// <param name="items">The items in the GW2 API for which data is to be retrieved.</param>
        void UploadToDatabase<R, E>(APIResource resource, DbSet<E> targetDataSet, IEnumerable<Item> items)
            where R : APIDataResult<E>
            where E : Entity;

        /// <summary>
        /// Gets results from the GW2 API and saves those results to the SQL database.
        /// </summary>
        /// <typeparam name="R">The result type that query results from the GW2 API are to be filtered into.</typeparam>
        /// <typeparam name="E">The entity type that is to be used to save the result data to the SQL database.</typeparam>
        /// <param name="resource">The type of resource to get data for.</param>
        /// <param name="targetDataSet">The dataset containing entities to be populated from results from the GW2 API.</param>
        void UploadToDatabase<R, E>(APIResource resource, DbSet<E> targetDataSet)
            where R : APIDataResult<E>
            where E : Entity;
    }
}
