using System;
using System.ComponentModel.DataAnnotations.Schema;
using Arbitrader.GW2API.Results;

namespace Arbitrader.GW2API.Entities
{
    [Table("RecipeFlags")]
    public class RecipeFlagEntity : Entity
    {
        public string Name { get; set; }

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