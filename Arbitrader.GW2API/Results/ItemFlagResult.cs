using System;
using Arbitrader.GW2API.Entities;

namespace Arbitrader.GW2API.Results
{
    public class ItemFlagResult : APIDataResult
    {
        public string name { get; set; }

        public static implicit operator ItemFlagResult(string s)
        {
            return new ItemFlagResult()
            {
                name = s
            };
        }

        public override Entity ToEntity()
        {
            return new ItemFlagEntity()
            {
                APIID = this.id,
                LoadDate = this.LoadDate,
                Name = this.name
            };
        }
    }
}
