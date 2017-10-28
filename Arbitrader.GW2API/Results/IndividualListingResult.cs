using System;
using Arbitrader.GW2API.Entities;

namespace Arbitrader.GW2API.Results
{
    public class IndividualListingResult : APIDataResult<IndividualListingEntity>
    {
        /// <summary>
        /// Gets or sets the number of individual listings at this price point.
        /// </summary>
        public int listings { get; set; }

        /// <summary>
        /// Gets or sets the order price per unit.
        /// </summary>
        public int unit_price { get; set; }

        /// <summary>
        /// Gets or sets the total number of units available at this price point.
        /// </summary>
        public int quantity { get; set; }

        /// <summary>
        /// Returns a <see cref="IndividualListingEntity"/> that contains the data from the <see cref="IndividualListingResult"/>.
        /// </summary>
        /// <returns>A <see cref="IndividualListingEntity"/> that contains the data from the <see cref="IndividualListingResult"/>.</returns>
        internal override IndividualListingEntity ToEntity()
        {
            return new IndividualListingEntity()
            {
                APIID = this.id,
                LoadDate = this.LoadDate,
                ListingCount = this.listings,
                UnitPrice = this.unit_price,
                Quantity = this.quantity
            };
        }
    }
}