using System;
using Arbitrader.GW2API.Entities;

namespace Arbitrader.GW2API.Results
{
    /// <summary>
    /// A sub-result of an Item query to the GW2 API. Specifies a single flag assigned to an item.
    /// </summary>
    public class ItemFlagResult : APIDataResult<ItemFlagEntity>
    {
        /// <summary>
        /// Gets or sets the name of the item flag.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Returns a <see cref="ItemFlagResult"/> that corresponds to the name given in the provided string.
        /// </summary>
        /// <param name="s">A <see cref="ItemFlagResult"/> that corresponds to the name given in the provided string.</param>
        public static implicit operator ItemFlagResult(string s)
        {
            return new ItemFlagResult()
            {
                name = s
            };
        }

        /// <summary>
        /// Returns a <see cref="ItemFlagEntity"/> that contains the data from the <see cref="ItemFlagResult"/>.
        /// </summary>
        /// <returns>A <see cref="ItemFlagEntity"/> that contains the data from the <see cref="ItemFlagResult"/>.</returns>
        internal override ItemFlagEntity ToEntity()
        {
            return new ItemFlagEntity()
            {
                APIID = this.id,
                LoadDate = this.LoadDate,
                Name = this.name
            };
        }
    }
}