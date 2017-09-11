using System.Collections.Generic;
using Arbitrader.GW2API.Entities;

namespace Arbitrader.GW2API.Results
{
    public class ListingResult : APIDataResult
    {
        /// <summary>
        /// Gets or sets the list of all buy orders for the item referenced by the listing.
        /// </summary>
        public IList<IndividualListingResult> buys { get; set; } = new List<IndividualListingResult>();

        /// <summary>
        /// Gets or sets the list of all sell orders for the item referenced by the listing.
        /// </summary>
        public IList<IndividualListingResult> sells { get; set; } = new List<IndividualListingResult>();

        /// <summary>
        /// Returns a <see cref="ListingEntity"/> that contains the data from the <see cref="ListingResult"/>.
        /// </summary>
        /// <returns>A <see cref="ListingEntity"/> that contains the data from the <see cref="ListingResult"/>.</returns>
        internal override Entity ToEntity()
        {
            return (ListingEntity)this;
        }
    }
}