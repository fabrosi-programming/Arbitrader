using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arbitrader.GW2API.Results;

namespace Arbitrader.GW2API.Entities
{
    [Table("Listings")]
    public class ListingEntity : Entity
    {
        /// <summary>
        /// Gets or sets the list of all buy or sell orders for the item referenced by the listing.
        /// </summary>
        public IList<IndividualListingEntity> IndividualListings { get; set; } = new List<IndividualListingEntity>();

        /// <summary>
        /// Converts from <see cref="ListingResult"/> to its associated entity, <see cref="ListingEntity"/>.
        /// </summary>
        /// <param name="result">A result containing the data to be mapped to the entity.</param>
        public static implicit operator ListingEntity(ListingResult result)
        {
            var entity = new ListingEntity()
            {
                APIID = result.id,
                LoadDate = result.LoadDate
            };

            foreach (var individualResult in result.buys)
            {
                var individualEntity = (IndividualListingEntity)individualResult.ToEntity();
                individualEntity.Direction = "Buy";
                entity.IndividualListings.Add(individualEntity);
            }

            foreach (var individualResult in result.sells)
            {
                var individualEntity = (IndividualListingEntity)individualResult.ToEntity();
                individualEntity.Direction = "Sell";
                entity.IndividualListings.Add(individualEntity);
            }

            return entity;
        }
    }
}