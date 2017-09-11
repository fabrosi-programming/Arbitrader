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
        public IList<IndividualListingEntity> Buys = new List<IndividualListingEntity>();

        public IList<IndividualListingEntity> Sells = new List<IndividualListingEntity>();

        public static implicit operator ListingEntity(ListingResult result)
        {
            return new ListingEntity()
            {
                APIID = result.id,
                LoadDate = result.LoadDate,
                Buys = result.buys.Select(l => (IndividualListingEntity)l.ToEntity()).ToList(),
                Sells = result.sells.Select(l => (IndividualListingEntity)l.ToEntity()).ToList()
            };
        }
    }
}