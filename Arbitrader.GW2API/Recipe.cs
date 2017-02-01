using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrader.GW2API
{
    internal class Recipe
    {
        internal static readonly string Controller = "recipes";

        public int id;
        public string type;
        public int outputItemID;
        public int outputItemCount;
        public IEnumerable<CraftingDiscipline> disciplines;
        public int minRating;
        public List<Item> ingredients;
        public List<Item> guildIngredients;


    }
}
