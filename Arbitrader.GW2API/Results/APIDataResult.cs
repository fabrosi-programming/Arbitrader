using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Arbitrader.GW2API.Entities;

namespace Arbitrader.GW2API.Results
{
    /// <summary>
    /// The base class for all results obtained from the GW2 API.
    /// </summary>
    public abstract class APIDataResult
    {
        /// <summary>
        /// Gets or sets the unique identifier in the GW2 API for the result associated with the entity.
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Gets or sets the date and time at which the result associated with the entity was pulled from the GW2 API.
        /// </summary>
        internal DateTime LoadDate { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="APIDataResult"/>.
        /// </summary>
        internal APIDataResult()
        {
            this.LoadDate = DateTime.Now;
        }

        /// <summary>
        /// Returns a database entity that contains the data from the <see cref="APIDataResult"/>.
        /// </summary>
        /// <returns>A database entity that contains the data from the <see cref="APIDataResult"/>.</returns>
        internal abstract Entity ToEntity();
    }
}