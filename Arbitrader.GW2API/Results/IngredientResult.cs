using System;
using Arbitrader.GW2API.Entities;

namespace Arbitrader.GW2API.Results
{
    public class IngredientResult : APIDataResult
    {
        public int item_id { get; set; }
        public int? count { get; set; }

        public override Entity ToEntity()
        {
            return new IngredientEntity()
            {
                APIID = this.id,
                LoadDate = this.LoadDate,
                Count = this.count,
                ItemID = this.item_id
            };
        }
    }
}