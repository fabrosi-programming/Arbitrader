using System.ComponentModel.DataAnnotations.Schema;
using Arbitrader.GW2API.Results;

namespace Arbitrader.GW2API.Entities
{
    [Table("IndividualListings")]
    public class IndividualListingEntity : Entity
    {
        /// <summary>
        /// Gets or sets whether the listing is a buy or sell order.
        /// </summary>
        public string Direction { get; set; }

        /// <summary>
        /// Gets or sets the number of individual listings at this price point.
        /// </summary>
        public int ListingCount { get; set; }

        /// <summary>
        /// Gets or sets the order price per unit.
        /// </summary>
        public int UnitPrice { get; set; }

        /// <summary>
        /// Gets or sets the total number of units available at this price point.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Converts from <see cref="IndividualListingResult"/> to its associated entity, <see cref="IndividualListingEntity"/>.
        /// </summary>
        /// <param name="result">A result containing the data to be mapped to the entity.</param>
        public static implicit operator IndividualListingEntity(IndividualListingResult result)
        {
            return new IndividualListingEntity()
            {
                APIID = result.id,
                LoadDate = result.LoadDate,
                ListingCount = result.listings,
                UnitPrice = result.unit_price,
                Quantity = result.quantity
            };
        }
    }
}