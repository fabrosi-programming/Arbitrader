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
    }
}