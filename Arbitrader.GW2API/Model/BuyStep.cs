using System;
using System.Collections.Generic;

namespace Arbitrader.GW2API.Model
{
    public class BuyStep : AcquisitionStep
    {
        private Func<int, int> _getMarketPrice;

        public BuyStep(Item item, int count, Func<int, int> getMarketPrice) : base(item, count)
        {
            _getMarketPrice = getMarketPrice;
        }

        public override int? GetBestPrice()
        {
            return _getMarketPrice(Count);
        }

        public override IEnumerable<AcquisitionStep> GetBestSteps()
        {
            return new List<AcquisitionStep>()
            {
                this
            };
        }

        public override string ToString()
        {
            return $"{base.ToString()} | Action: Buy | Price: {GetBestPrice()}";
        }
    }
}
