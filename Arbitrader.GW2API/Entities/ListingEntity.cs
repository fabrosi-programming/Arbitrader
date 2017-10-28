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
    }
}