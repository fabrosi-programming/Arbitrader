using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Arbitrader.GW2API.Entities;

namespace Arbitrader.GW2API.Results
{
    /// <summary>
    /// A sub-result of a Recipe query to the GW2 API. Specifies a guild ingredient required to craft a recipe.
    /// </summary>
    public class GuildIngredientResult : APIDataResult
    {
        /// <summary>
        /// Gets or sets the unique identifier in the GW2 API for the guild hall upgrade that requires the ingredient.
        /// </summary>
        public int upgrade_id { get; set; }

        /// <summary>
        /// Gets or sets the number of the ingredient required for the guild hall upgrade.
        /// </summary>
        public int? count { get; set; }

        /// <summary>
        /// Returns a <see cref="GuildIngredientEntity"/> that contains the data from the <see cref="GuildIngredientResult"/>.
        /// </summary>
        /// <returns>A <see cref="GuildIngredientEntity"/> that contains the data from the <see cref="GuildIngredientResult"/>.</returns>
        public override Entity ToEntity()
        {
            return new GuildIngredientEntity()
            {
                APIID = this.id,
                LoadDate = this.LoadDate,
                Count = this.count,
                UpgradeID = this.upgrade_id
            };
        }
    }
}