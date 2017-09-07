using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arbitrader.GW2API.Results;

namespace Arbitrader.GW2API.Entities
{
    [Table("Ingredients")]
    public class IngredientEntity : Entity
    {
        public int ItemID { get; set; }
        public int? Count { get; set; }

        public static implicit operator IngredientEntity(IngredientResult result)
        {
            return (IngredientEntity)result.ToEntity();
        }
    }
}
