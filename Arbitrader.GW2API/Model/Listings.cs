using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrader.GW2API.Model
{
    internal class Listings : List<Listing>
    {
        public new void Add(Listing listing)
        {
            var match = this.Where(l => l.Direction == listing.Direction)
                            .Where(l => l.UnitPrice == listing.UnitPrice)
                            .FirstOrDefault();

            if (match != null)
                match.Quantity += listing.Quantity;
        }
    }
}