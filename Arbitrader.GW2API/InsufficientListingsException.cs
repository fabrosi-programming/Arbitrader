using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arbitrader.GW2API.Model;

namespace Arbitrader.GW2API
{
    public class InsufficientListingsException : Exception
    {
        Item Item { get; set; }
        int Count { get; set; }

        public InsufficientListingsException(Item item, int count)
            : base($"Insufficient listings in the market to allow {count} of {item} to be bought.")
        {
            this.Item = item;
            this.Count = count;
        }
    }
}
