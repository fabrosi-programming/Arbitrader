using System;
using Arbitrader.GW2API.Entities;

namespace Arbitrader.GW2API.Results
{
    public class DisciplineResult : APIDataResult
    {
        public string name { get; set; }

        public static implicit operator DisciplineResult(string s)
        {
            return new DisciplineResult()
            {
                name = s
            };
        }

        public override Entity ToEntity()
        {
            return new DisciplineEntity()
            {
                APIID = this.id,
                LoadDate = this.LoadDate,
                Name = this.name
            };
        }
    }
}