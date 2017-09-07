using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Arbitrader.GW2API.Entities;

namespace Arbitrader.GW2API.Results
{
    public class GuildIngredientResult : APIDataResult
    {
        public int upgrade_id { get; set; }
        public int? count { get; set; }

        public override Entity ToEntity()
        {
            return new GuildIngredientEntity()
            {
                APIID = this.id,
                LoadDate = this.LoadDate,
                Count = this.count,
                UpgradeID = this.upgrade_id
            };
        }
    }
}