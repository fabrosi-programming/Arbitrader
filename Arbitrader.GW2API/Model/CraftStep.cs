using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arbitrader.GW2API.Model
{
    public class CraftStep : AcquisitionStep
    {
        public Dictionary<Item, IEnumerable<AcquisitionStep>> IngredientSteps { get; set; }

        public CraftStep(Recipe recipe, int count) : base(recipe.OutputItem, count)
        {
            IngredientSteps = new Dictionary<Item, IEnumerable<AcquisitionStep>>();

            foreach (var ingredient in recipe.Ingredients)
                IngredientSteps.Add(ingredient.Key, ingredient.Key.GetAcquisitionSteps(ingredient.Value * count));
        }

        public override int? GetBestPrice()
        {
            return GetBestSteps()?.Select(s => s.GetBestPrice())?.Sum();
        }

        public override IEnumerable<AcquisitionStep> GetBestSteps()
        {
            foreach (var ingredient in IngredientSteps)
                yield return ingredient.Value.MinimizeOn(s => s.GetBestPrice());
        }

        public override string ToString()
        {
            return ToString("");
        }

        public override string ToString(string prepend)
        {
            var sb = new StringBuilder($"{prepend}{base.ToString()} | Action: Craft");

            foreach (var step in GetBestSteps())
                sb.Append($"\r\n{step.ToString($"{prepend}  ")}");

            return sb.ToString();
        }
    }
}
