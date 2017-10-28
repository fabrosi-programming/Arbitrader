using System;
using System.Collections.Generic;
using System.Linq;
using Arbitrader.GW2API.Entities;

namespace Arbitrader.GW2API.Results
{
    /// <summary>
    /// A result from a Recipe query to the GW2 API.
    /// </summary>
    public class RecipeResult : APIDataResult<RecipeEntity>
    {
        /// <summary>
        /// Gets or sets the type of the recipe.
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier in the GW2 API for the item that results from crafting the recipe.
        /// </summary>
        public int? output_item_id { get; set; }

        /// <summary>
        /// Gets or sets the count of the output item that results from crafting the recipe.
        /// </summary>
        public int? output_item_count { get; set; }

        /// <summary>
        /// Gets or sets the discipline rating level required to craft the recipe.
        /// </summary>
        public int? min_rating { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier in the GW2 API for the guild hall upgrade produced by crafting the recipe.
        /// </summary>
        public int? output_upgrade_id { get; set; }

        /// <summary>
        /// Gets or sets the list of disciplines that are able to craft the recipe.
        /// </summary>
        public IList<DisciplineResult> disciplines { get; set; } = new List<DisciplineResult>();

        /// <summary>
        /// Gets or sets the list of flags assigned to the recipe.
        /// </summary>
        public IList<RecipeFlagResult> flags { get; set; } = new List<RecipeFlagResult>();

        /// <summary>
        /// Gets or sets the list of ingredients required to craft the recipe.
        /// </summary>
        public IList<IngredientResult> ingredients { get; set; } = new List<IngredientResult>();

        /// <summary>
        /// Gets or sets the list of guild ingredients required to craft the recipe.
        /// </summary>
        public IList<GuildIngredientResult> guild_ingredients { get; set; } = new List<GuildIngredientResult>();

        /// <summary>
        /// Returns a <see cref="RecipeEntity"/> that contains the data from the <see cref="RecipeResult"/>.
        /// </summary>
        /// <returns>A <see cref="RecipeEntity"/> that contains the data from the <see cref="RecipeResult"/>.</returns>
        internal override RecipeEntity ToEntity()
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
                        .Select(d => d.ToEntity())
                        .ToList(),
                Flags = this.flags
                  .Select(f => f.ToEntity())
                  .ToList(),
                Ingredients = this.ingredients
                        .Select(i => i.ToEntity())
                        .ToList(),
                GuildIngredients = this.guild_ingredients
                             .Select(i => i.ToEntity())
                             .ToList()
            };
        }
    }
}