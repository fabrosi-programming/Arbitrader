using System;
using System.ComponentModel.DataAnnotations.Schema;
using Arbitrader.GW2API.Results;

namespace Arbitrader.GW2API.Entities
{
    /// <summary>
    /// A row of data in the RecipeFlags table. Associated with the result type <see cref="RecipeFlagResult"/>.
    /// </summary>
    [Table("RecipeFlags")]
    public class RecipeFlagEntity : Entity
    {
        /// <summary>
        /// Gets or sets the name of the recipe flag.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Converts from <see cref="RecipeFlagResult"/> to its associated entity, <see cref="RecipeFlagEntity"/>.
        /// </summary>
        /// <param name="result">A result containing the data to be mapped to the entity.</param>
        public static implicit operator RecipeFlagEntity(RecipeFlagResult result)
        {
            return new RecipeFlagEntity()
            {
                APIID = result.id,
                LoadDate = result.LoadDate,
                Name = result.name
            };
        }
    }
}