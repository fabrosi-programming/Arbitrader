using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrader.UI.WPF.ViewModel
{
    public struct ActiveOrder
    {
        public string Item { get; set; }
        public Direction Direction { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public int SellAt { get; set; }
        public double ExpectedROI { get; set; }
    }
}
