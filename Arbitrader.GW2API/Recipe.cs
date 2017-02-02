using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrader.GW2API
{
    internal class Recipe
    {
        public int id{ get; set; }
        public string type{ get; set; }
        public int outputItemID{ get; set; }
        public int outputItemCount{ get; set; }
        public IEnumerable<CraftingDiscipline> disciplines{ get; set; }
        public int minRating{ get; set; }
        public List<Item> ingredients{ get; set; }
        public List<Item> guildIngredients{ get; set; }
    }
}
