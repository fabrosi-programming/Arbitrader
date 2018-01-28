using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arbitrader.GW2API.Entities;

namespace Arbitrader.GW2API.Model
{
    public class Recipes : IEnumerable<Recipe>
    {
        private List<Recipe> _recipes = new List<Recipe>();

        private Func<int, Item> _getItem;

        public IEnumerable<Item> Ingredients
        {
            get
            {
                return this._recipes.SelectMany(r => r.Ingredients.Keys);
            }
        }

        public Recipes()
        { }

        public Recipes(Func<int, Item> getItem)
        {
            this._getItem = getItem;
        }

        public Recipes(IEnumerable<Recipe> recipes, Func<int, Item> getItem)
            : this(getItem)
        {
            this._recipes = recipes.ToList();
        }

        public Recipes(IEnumerable<RecipeEntity> entities, Func<int, Item> getItem)
            : this(entities.Select(e => new Recipe(e, getItem)), getItem)
        { }

        public void Add(Recipe recipe)
        {
            if (!this._recipes.Any(r => r.ID == recipe.ID))
                this._recipes.Add(recipe);
        }

        public void Add(IEnumerable<Recipe> recipes)
        {
            foreach (var recipe in recipes)
                this.Add(recipe);
        }

        public void Add(RecipeEntity entity)
        {
            this.Add(new Recipe(entity, this._getItem));
        }

        public void Add(IEnumerable<RecipeEntity> entities)
        {
            foreach (var entity in entities)
                this.Add(entity);
        }

        #region IEnumerable<Recipe> Support
        public IEnumerator<Recipe> GetEnumerator()
        {
            return ((IEnumerable<Recipe>)this._recipes).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Recipe>)this._recipes).GetEnumerator();
        }
        #endregion
    }
}
