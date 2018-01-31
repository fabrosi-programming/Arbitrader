using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrader.GW2API.Model
{
    public class AcquisitionPlan :IEnumerable<AcquisitionStep>
    {
        public List<AcquisitionStep> Steps { get; private set; } = new List<AcquisitionStep>();

        public void Append(ActionType actionType, Item item, int count, int price = 0)
        {
            this.Steps.Add(new AcquisitionStep(actionType, item, count, price));
        }

        public void Append(AcquisitionPlan plan)
        {
            this.Steps.AddRange(plan.Steps);
        }

        #region IEnumerable<AcquisitionStep> Support
        public IEnumerator<AcquisitionStep> GetEnumerator()
        {
            return ((IEnumerable<AcquisitionStep>)this.Steps).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<AcquisitionStep>)this.Steps).GetEnumerator();
        }
        #endregion
    }
}
