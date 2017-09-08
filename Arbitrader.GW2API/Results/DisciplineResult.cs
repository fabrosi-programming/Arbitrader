using System;
using Arbitrader.GW2API.Entities;

namespace Arbitrader.GW2API.Results
{
    /// <summary>
    /// A sub-result of an Item query to the GW2 API. Specifies a single discipline and a single item
    /// that can be crafted by that discipline.
    /// </summary>
    public class DisciplineResult : APIDataResult
    {
        /// <summary>
        /// Gets or sets the name of the discipline.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Returns a <see cref="DisciplineResult"/> that corresponds to the name given in the provided string.
        /// </summary>
        /// <param name="s">A <see cref="DisciplineResult"/> that corresponds to the name given in the provided string.</param>
        public static implicit operator DisciplineResult(string s)
        {
            return new DisciplineResult()
            {
                name = s
            };
        }

        /// <summary>
        /// Returns a <see cref="DisciplineEntity"/> that contains the data from the <see cref="DisciplineResult"/>.
        /// </summary>
        /// <returns>A <see cref="DisciplineEntity"/> that contains the data from the <see cref="DisciplineResult"/>.</returns>
        public override Entity ToEntity()
        {
            return new DisciplineEntity()
            {
                APIID = this.id,
                LoadDate = this.LoadDate,
                Name = this.name
            };
        }
    }
}