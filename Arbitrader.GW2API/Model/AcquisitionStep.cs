using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arbitrader.GW2API.Model
{
    public abstract class AcquisitionStep
    {
        public Item Item { get; private set; }

        public int Count { get; protected set; }

        public AcquisitionStep(Item item, int count)
        {
            Item = item;
            Count = count;
        }

        public abstract int? GetBestPrice();

        public abstract IEnumerable<AcquisitionStep> GetBestSteps();

        public override string ToString()
        {
            return $"Item: {Item.Name} | Count: {Count}";
        }

        public virtual string ToString(string prepend)
        {
            return $"{prepend}{ToString()}";
        }
    }
}
