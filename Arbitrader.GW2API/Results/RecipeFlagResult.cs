using System;
using Arbitrader.GW2API.Entities;

namespace Arbitrader.GW2API.Results
{
    public class RecipeFlagResult : APIDataResult
    {
        public string name { get; set; }

        public static implicit operator RecipeFlagResult(string s)
        {
            return new RecipeFlagResult()
            {
                name = s
            };
        }

        public override Entity ToEntity()
        {
            return new RecipeFlagEntity()
            {
                APIID = this.id,
                LoadDate = this.LoadDate,
                Name = this.name
            };
        }
    }
}
