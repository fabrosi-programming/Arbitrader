using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Arbitrader.GW2API.Results;

namespace Arbitrader.GW2API.Entities
{
    [Table("Recipes")]
    public class RecipeEntity : Entity
    {
        public string Type { get; set; }
        public int? OutputItemID { get; set; }
        public int? OutputItemCount { get; set; }
        public int? MinimumRating { get; set; }
        public int? OutputUpgradeID { get; set; }
        public IList<DisciplineEntity> Disciplines { get; set; } = new List<DisciplineEntity>();
        public IList<RecipeFlagEntity> Flags { get; set; } = new List<RecipeFlagEntity>();
        public IList<IngredientEntity> Ingredients { get; set; } = new List<IngredientEntity>();
        public IList<GuildIngredientEntity> GuildIngredients { get; set; } = new List<GuildIngredientEntity>();

        public static implicit operator RecipeEntity(RecipeResult result)
        {
            return new RecipeEntity()
            {
                APIID = result.id,
                LoadDate = result.LoadDate,
                Type = result.type,
                OutputItemID = result.output_item_id,
                OutputItemCount = result.output_item_count,
                MinimumRating = result.min_rating,
                OutputUpgradeID = result.output_upgrade_id,
                Disciplines = result.disciplines
                                    .Select(d => (DisciplineEntity)d)
                                    .ToList(),
                Flags = result.flags
                              .Select(f => (RecipeFlagEntity)f)
                              .ToList(),
                Ingredients = result.ingredients
                                    .Select(i => (IngredientEntity)i)
                                    .ToList(),
                GuildIngredients = result.guild_ingredients
                                          .Select(i => (GuildIngredientEntity)i)
                                          .ToList()
            };
        }
    }
}
