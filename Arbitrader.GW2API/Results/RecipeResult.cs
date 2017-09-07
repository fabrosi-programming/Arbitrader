using System;
using System.Collections.Generic;
using System.Linq;
using Arbitrader.GW2API.Entities;

namespace Arbitrader.GW2API.Results
{
    public class RecipeResult : APIDataResult
    {
        public string type { get; set; }
        public int? output_item_id { get; set; }
        public int? output_item_count { get; set; }
        public int? min_rating { get; set; }
        public int? output_upgrade_id { get; set; }
        public IList<DisciplineResult> disciplines { get; set; } = new List<DisciplineResult>();
        public IList<RecipeFlagResult> flags { get; set; } = new List<RecipeFlagResult>();
        public IList<IngredientResult> ingredients { get; set; } = new List<IngredientResult>();
        public IList<GuildIngredientResult> guild_ingredients { get; set; } = new List<GuildIngredientResult>();

        public override Entity ToEntity()
        {
            return new RecipeEntity()
            {
                APIID = this.id,
                LoadDate = this.LoadDate,
                Type = this.type,
                OutputItemID = this.output_item_id,
                OutputItemCount = this.output_item_count,
                MinimumRating = this.min_rating,
                OutputUpgradeID = this.output_upgrade_id,
                Disciplines = this.disciplines
                                  .Select(d => (DisciplineEntity)d)
                                  .ToList(),
                Flags = this.flags
                            .Select(f => (RecipeFlagEntity)f)
                            .ToList(),
                Ingredients = this.ingredients
                                  .Select(i => (IngredientEntity)i)
                                  .ToList(),
                GuildIngredients = this.guild_ingredients
                                       .Select(i => (GuildIngredientEntity)i)
                                       .ToList()
            };
        }
    }
}