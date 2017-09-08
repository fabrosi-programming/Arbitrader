using System;
using Arbitrader.GW2API.Entities;

namespace Arbitrader.GW2API.Results
{
    /// <summary>
    /// A sub-result of a Recipe query to the GW2 API. Specifies an ingredient required to craft a recipe.
    /// </summary>
    public class IngredientResult : APIDataResult
    {
        /// <summary>
        /// Gets or sets the unique identifier in the GW2 APi for the item that is used as an ingredient.
        /// </summary>
        public int item_id { get; set; }

        /// <summary>
        /// Gets or sets the number of the the ingredient required by its recipe.
        /// </summary>
        public int? count { get; set; }

        /// <summary>
        /// Returns a <see cref="IngredientEntity"/> that contains the data from the <see cref="IngredientResult"/>.
        /// </summary>
        /// <returns>A <see cref="IngredientEntity"/> that contains the data from the <see cref="IngredientResult"/>.</returns>
        public override Entity ToEntity()
        {
            return new IngredientEntity()
            {
                APIID = this.id,
                LoadDate = this.LoadDate,
                Count = this.count,
                ItemID = this.item_id
            };
        }
    }
}