using System.Collections.Generic;

namespace Arbitrader.GW2API.Model
{
    public class AcquireStep : AcquisitionStep
    {
        public AcquireStep(Item item, int count) : base(item, count)
        { }

        public override int? GetBestPrice()
        {
            return null;
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
            return $"{base.ToString()} | Action: Acquire";
        }
    }
}
