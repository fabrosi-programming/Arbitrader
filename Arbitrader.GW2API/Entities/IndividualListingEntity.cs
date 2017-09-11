using System.ComponentModel.DataAnnotations.Schema;
using Arbitrader.GW2API.Results;

namespace Arbitrader.GW2API.Entities
{
    [Table("IndividualListings")]
    public class IndividualListingEntity : Entity
    {
        /// <summary>
        /// Gets or sets the number of individual listings at this price point.
        /// </summary>
        public int Listings { get; set; }

        /// <summary>
        /// Gets or sets the order price per unit.
        /// </summary>
        public int UnitPrice { get; set; }

        /// <summary>
        /// Gets or sets the total number of units available at this price point.
        /// </summary>
        public int Quantity { get; set; }

        public static implicit operator IndividualListingEntity(IndividualListingResult result)
        {
            return new IndividualListingEntity()
            {
                APIID = result.id,
                LoadDate = result.LoadDate,
                Listings = result.listings,
                UnitPrice = result.unit_price,
                Quantity = result.quantity
            };
        }
    }
}