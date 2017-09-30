using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrader.GW2API.Entities
{
    [Table("WatchedItems")]
    public class WatchedItem : Entity
    {
        /// <summary>
        /// Initializes a new instance of <see cref="WatchedItem"/>.
        /// </summary>
        public WatchedItem()
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="WatchedItem"/> with the given API ID.
        /// </summary>
        /// <param name="apiID">The API ID of the item to be watched.</param>
        public WatchedItem(int apiID)
        {
            this.APIID = apiID;
        }
    }
}