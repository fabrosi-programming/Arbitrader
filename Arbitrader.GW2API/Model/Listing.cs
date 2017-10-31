using System;
using System.ComponentModel.DataAnnotations.Schema;
using Arbitrader.GW2API.Entities;
using Arbitrader.GW2API.Results;

namespace Arbitrader.GW2API.Model
{
    internal class Listing
    {
        /// <summary>
        /// Gets or sets whether the listing is a buy or sell order.
        /// </summary>
        internal Direction Direction { get; set; }

        /// <summary>
        /// Gets or sets the number of individual listings at this price point.
        /// </summary>
        internal int ListingCount { get; set; }

        /// <summary>
        /// Gets or sets the order price per unit.
        /// </summary>
        internal int UnitPrice { get; set; }

        /// <summary>
        /// Gets or sets the total number of units available at this price point.
        /// </summary>
        internal int Quantity { get; set; }

        public Listing(IndividualListingEntity individualListingEntity)
        {
            this.Direction = Enum.TryParse(individualListingEntity.Direction, out Direction direction) ? direction : Direction.None;
            this.ListingCount = individualListingEntity.ListingCount;
            this.UnitPrice = individualListingEntity.UnitPrice;
            this.Quantity = individualListingEntity.Quantity;
        }
    }
}