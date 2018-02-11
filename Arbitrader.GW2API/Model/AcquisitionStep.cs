using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrader.GW2API.Model
{
    public abstract class AcquisitionStep// : IEnumerable<AcquisitionStep>
    {
        public Item Item { get; private set; }

        public int Count { get; private set; }

        public AcquisitionStep(Item item, int count)
        {
            this.Item = item;
            this.Count = count;
        }

        public abstract int? GetBestPrice();

        public abstract IEnumerable<AcquisitionStep> GetBestSteps();

        public override string ToString()
        {
            return $"Item: {this.Item.Name} | Count: {this.Count}";
        }

        public virtual string ToString(string prepend)
        {
            return $"{prepend}{this.ToString()}";
        }
    }

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

    public class BuyStep : AcquisitionStep
    {
        private Func<int, int> _getMarketPrice;

        public BuyStep(Item item, int count, Func<int, int> getMarketPrice) : base(item, count)
        {
            this._getMarketPrice = getMarketPrice;
        }

        public override int? GetBestPrice()
        {
            return this._getMarketPrice(this.Count);
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
            return $"{base.ToString()} | Action: Buy | Price: {this.GetBestPrice()}";
        }
    }

    public class CraftStep : AcquisitionStep
    {
        public Dictionary<Item, IEnumerable<AcquisitionStep>> IngredientSteps { get; set; }

        public CraftStep(Recipe recipe, int count) : base(recipe.OutputItem, count)
        {
            this.IngredientSteps = new Dictionary<Item, IEnumerable<AcquisitionStep>>();

            foreach (var ingredient in recipe.Ingredients)
                this.IngredientSteps.Add(ingredient.Key, ingredient.Key.GetAcquisitionSteps(ingredient.Value * count));
        }

        public override int? GetBestPrice()
        {
            return this.GetBestSteps()?.Select(s => s.GetBestPrice())?.Sum();
        }

        public override IEnumerable<AcquisitionStep> GetBestSteps()
        {
            foreach (var ingredient in this.IngredientSteps)
                yield return ingredient.Value.MinimizeOn(s => s.GetBestPrice());
        }

        public override string ToString()
        {
            return this.ToString("");
        }

        public override string ToString(string prepend)
        {
            var sb = new StringBuilder($"{prepend}{base.ToString()} | Action: Craft");

            foreach (var step in this.GetBestSteps())
                sb.Append($"\r\n{step.ToString($"{prepend}  ")}");

            return sb.ToString();
        }
    }

    public static class AcquisitionStepExtensions
    {
        public static AcquisitionStep MinimizeOn(this IEnumerable<AcquisitionStep> steps, Func<AcquisitionStep, int?> criteria)
        {
            var valueMap = new Dictionary<AcquisitionStep, int?>();

            foreach (var step in steps)
                valueMap.Add(step, criteria(step));

            var minValue = valueMap.Values.Min(); // Min() excludes null
            var minSteps = valueMap.Where(s => s.Value == minValue).Select(s => s.Key);

            AcquisitionStep buyStep = minSteps.OfType<BuyStep>().FirstOrDefault();
            AcquisitionStep craftStep = minSteps.OfType<CraftStep>().FirstOrDefault();
            AcquisitionStep acquireStep = minSteps.OfType<AcquireStep>().FirstOrDefault();

            return buyStep ?? craftStep ?? acquireStep;            
        }

        public static IEnumerable<AcquisitionStep> OfType<T>(this IEnumerable<AcquisitionStep> steps)
            where T : AcquisitionStep
        {
            return steps.Where(s => s.GetType() == typeof(T));
        }
    }
}
