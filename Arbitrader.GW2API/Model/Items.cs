using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arbitrader.GW2API.Entities;

namespace Arbitrader.GW2API.Model
{
    internal class Items : List<Item>
    {
        public Items() : base()
        { }

        public Items(IEnumerable<Item> items) : base(items)
        { }

        public void AttachListings(IEnumerable<ListingEntity> listings)
        {
            foreach (var item in this)
            {
                var listing = listings.Where(l => l.APIID == item.ID).FirstOrDefault();

                if (listing == null)
                    continue;

                foreach (var individualListing in listing.IndividualListings)
                    item.Listings.Merge(new Listing(individualListing));
            }
        }

        /// <summary>
        /// Returns a new <see cref="Items"/> collection that includes the contents of the current <see cref="Items"/> collection
        /// except that it excludes those items that cannot be traded on the trading post.
        /// </summary>
        /// <returns>A new <see cref="Items"/> collection that includes the contents of the current <see cref="Items"/> collection
        /// except that it excludes those items that cannot be traded on the trading post.</returns>

        public Items ExcludeNonSellable()
        {
            return new Items(this.Where(i => !i.Flags.Contains(Flag.NoSell))
                                 .Where(i => !i.Flags.Contains(Flag.AccountBound))
                                 .Where(i => !i.Flags.Contains(Flag.MonsterOnly))
                                 .Where(i => !i.Flags.Contains(Flag.SoulbindOnAcquire)));
        }
    }
}
