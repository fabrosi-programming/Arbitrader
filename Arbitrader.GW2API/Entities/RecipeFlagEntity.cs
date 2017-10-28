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
    }
}