using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Arbitrader.GW2API.Entities;

namespace Arbitrader.GW2API.Results
{
    public abstract class APIDataResult
    {
        public int id { get; set; }
        public DateTime LoadDate { get; set; }

        public APIDataResult()
        {
            this.LoadDate = DateTime.Now;
        }

        public abstract Entity ToEntity();
    }
}