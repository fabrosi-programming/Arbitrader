using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arbitrader.GW2API.Results;

namespace Arbitrader.GW2API.Entities
{
    /// <summary>
    /// A row of data in the Ingredients table. Associated with the result type <see cref="IngredientResult"/>.
    /// </summary>
    [Table("Ingredients")]
    public class IngredientEntity : Entity
    {
        /// <summary>
        /// Gets or sets the unique identifier in the GW2 APi for the item that is used as an ingredient.
        /// </summary>
        public int ItemID { get; set; }

        /// <summary>
        /// Gets or sets the number of the the ingredient required by its recipe.
        /// </summary>
        public int? Count { get; set; }

        /// <summary>
        /// Converts from <see cref="IngredientResult"/> to its associated entity, <see cref="IngredientEntity"/>.
        /// </summary>
        /// <param name="result">A result containing the data to be mapped to the entity.</param>
        public static implicit operator IngredientEntity(IngredientResult result)
        {
            return (IngredientEntity)result.ToEntity();
        }
    }
}