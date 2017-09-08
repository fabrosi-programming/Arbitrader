using System;
using Arbitrader.GW2API.Entities;

namespace Arbitrader.GW2API.Results
{
    /// <summary>
    /// A sub-result of a Recipe query to the GW2 API. Specifies a single flag associated with a single recipe.
    /// </summary>
    public class RecipeFlagResult : APIDataResult
    {
        /// <summary>
        /// Gets or sets the name of the recipe flag.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Returns a <see cref="RecipeFlagResult"/> that corresponds to the name given in the provided string.
        /// </summary>
        /// <param name="s">A <see cref="RecipeFlagResult"/> that corresponds to the name given in the provided string.</param>
        public static implicit operator RecipeFlagResult(string s)
        {
            return new RecipeFlagResult()
            {
                name = s
            };
        }

        /// <summary>
        /// Returns a <see cref="RecipeFlagEntity"/> that contains the data from the <see cref="RecipeFlagResult"/>.
        /// </summary>
        /// <returns>A <see cref="RecipeFlagEntity"/> that contains the data from the <see cref="RecipeFlagResult"/>.</returns>
        public override Entity ToEntity()
        {
            return new RecipeFlagEntity()
            {
                APIID = this.id,
                LoadDate = this.LoadDate,
                Name = this.name
            };
        }
    }
}
