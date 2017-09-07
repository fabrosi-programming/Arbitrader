using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Arbitrader.GW2API.Entities;
using Arbitrader.GW2API.Results;

namespace Arbitrader.GW2API.Model
{
    public class Recipe
    {
        private int _id;
        private string _type;
        private Item _outputItem;
        private int _outputItemCount;
        private int _minRating;

        private Collection<Discipline> _disciplines = new Collection<Discipline>();
        private Collection<Flag> _flags = new Collection<Flag>();

        public Collection<Item> Ingredients { get; set; } = new Collection<Item>();

        public Recipe(RecipeEntity recipe, ItemContext context)
        {
            this._type = recipe.Type;
            this._outputItem = this.GetItem(recipe.OutputItemID.Value, context.Items);
            this._outputItemCount = recipe.OutputItemCount ?? 0;

            foreach (var disciplineResult in recipe.Disciplines)
                this._disciplines.Add((Discipline)Enum.Parse(typeof(Discipline), disciplineResult.Name));

            foreach (var flagResult in recipe.Flags)
                this._flags.Add((Flag)Enum.Parse(typeof(Flag), flagResult.Name));

            foreach (var ingredient in recipe.Ingredients)
                this.Ingredients.Add(this.GetItem(ingredient.ItemID, context.Items));

            foreach (var ingredient in recipe.GuildIngredients)
                this.Ingredients.Add(this.GetItem(ingredient.UpgradeID, context.Items));

            // update each ingredient's recipe list to allow travel up the crafting tree
            foreach (var ingredient in this.Ingredients)
                ingredient.DependentRecipes.Add(this);

            this._outputItem.GeneratingRecipes.Add(this);
        }

        private Item GetItem(int itemId, IEnumerable<Item> items)
        {
            return items.Where(i => i.ID == itemId)
                        .First();
        }

        public override string ToString()
        {
            return $"{this._outputItem} | {this._outputItemCount}";
        }
    }
}
