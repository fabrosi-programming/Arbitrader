using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Arbitrader.GW2API.Model;

namespace Arbitrader.GW2API
{
    public class CraftingTreeNode
    {
        private Item _item;
        private Collection<CraftingTreeNode> _childNodes = new Collection<CraftingTreeNode>();

        public CraftingTreeNode(Item item)
        {
            this._item = item;

            foreach (var ingredient in this._item.DependentRecipes.First().Ingredients)
                this._childNodes.Add(new CraftingTreeNode(ingredient));
        }

        public int GetPrice(Market market)
        {
            if (_childNodes.Count() > 0)
                return this._childNodes.Sum(n => n.GetPrice(market));

            return market.GetBuyPrice(this._item);
        }
    }
}
