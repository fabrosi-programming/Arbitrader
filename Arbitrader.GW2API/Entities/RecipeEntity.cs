using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Arbitrader.GW2API.Results;

namespace Arbitrader.GW2API.Entities
{
    /// <summary>
    /// A row of data in the Recipes table. Associated with the result type <see cref="RecipeResult"/>.
    /// </summary>
    [Table("Recipes")]
    public class RecipeEntity : Entity
    {
        /// <summary>
        /// Gets or sets the type of the recipe.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier in the GW2 API for the item that results from crafting the recipe.
        /// </summary>
        public int? OutputItemID { get; set; }

        /// <summary>
        /// Gets or sets the count of the output item that results from crafting the recipe.
        /// </summary>
        public int? OutputItemCount { get; set; }

        /// <summary>
        /// Gets or sets the discipline rating level required to craft the recipe.
        /// </summary>
        public int? MinimumRating { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier in the GW2 API for the guild hall upgrade produced by crafting the recipe.
        /// </summary>
        public int? OutputUpgradeID { get; set; }

        /// <summary>
        /// Gets or sets the list of disciplines that are able to craft the recipe.
        /// </summary>
        public IList<DisciplineEntity> Disciplines { get; set; } = new List<DisciplineEntity>();

        /// <summary>
        /// Gets or sets the list of flags assigned to the recipe.
        /// </summary>
        public IList<RecipeFlagEntity> Flags { get; set; } = new List<RecipeFlagEntity>();

        /// <summary>
        /// Gets or sets the list of ingredients required to craft the recipe.
        /// </summary>
        public IList<IngredientEntity> Ingredients { get; set; } = new List<IngredientEntity>();

        /// <summary>
        /// Gets or sets the list of guild ingredients required to craft the recipe.
        /// </summary>
        public IList<GuildIngredientEntity> GuildIngredients { get; set; } = new List<GuildIngredientEntity>();

        /// <summary>
        /// Converts from <see cref="RecipeResult"/> to its associated entity, <see cref="RecipeEntity"/>.
        /// </summary>
        /// <param name="result">A result containing the data to be mapped to the entity.</param>
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