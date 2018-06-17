using System;
using System.Collections.Generic;
using System.Linq;

namespace Arbitrader.GW2API.Model
{
    public static class AcquisitionStepExtensions
    {
        public static AcquisitionStep MinimizeOn(this IEnumerable<AcquisitionStep> steps, Func<AcquisitionStep, int?> criteria)
        {
            var valueMap = new Dictionary<AcquisitionStep, int?>();

            foreach (var step in steps)
                valueMap.Add(step, criteria(step));

            var minValue = valueMap.Values.Min(); // Min() excludes null
            var minSteps = valueMap.Where(s => s.Value == minValue).Select(s => s.Key);

            var buyStep = minSteps.OfType<BuyStep>().FirstOrDefault();
            var craftStep = minSteps.OfType<CraftStep>().FirstOrDefault();
            var acquireStep = minSteps.OfType<AcquireStep>().FirstOrDefault();

            return buyStep ?? craftStep ?? acquireStep;            
        }

        public static IEnumerable<AcquisitionStep> OfType<T>(this IEnumerable<AcquisitionStep> steps)
            where T : AcquisitionStep
        {
            return steps.Where(s => s.GetType() == typeof(T));
        }
    }
}
