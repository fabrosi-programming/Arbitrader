using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrader.GW2API.Model
{
    public class Order
    {
        public Direction Direction { get; set; }
        public Item Item { get; set; }
        public int Count { get; set; }
        public int UnitPrice { get; set; }
    }
}
