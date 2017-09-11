using System.Collections.Generic;
using Arbitrader.GW2API.Entities;

namespace Arbitrader.GW2API.Results
{
    public class ListingResult : APIDataResult
    {
        public IList<IndividualListingResult> buys { get; set; } = new List<IndividualListingResult>();

        public IList<IndividualListingResult> sells { get; set; } = new List<IndividualListingResult>();

        internal override Entity ToEntity()
        {
            return (ListingEntity)this;
        }
    }
}