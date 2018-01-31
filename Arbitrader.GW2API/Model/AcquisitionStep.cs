using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrader.GW2API.Model
{
    public class AcquisitionStep
    {
        public ActionType ActionType { get; private set; }

        public Item Item { get; private set; }

        public int Count { get; private set; }

        public int Price { get; private set; }

        public AcquisitionStep(ActionType actionType, Item item, int count, int price = 0)
        {
            this.ActionType = actionType;
            this.Item = item;
            this.Count = count;
            this.Price = price;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append($"Action: {this.ActionType}");
            sb.Append(" | ");
            sb.Append($"Item: {this.Item.Name}");
            sb.Append(" | ");
            sb.Append($"Count: {this.Count}");

            if (this.Price > 0)
            {
                sb.Append(" | ");
                sb.Append($"Price: {this.Price}");
            }

            return sb.ToString();
        }
    }
}
