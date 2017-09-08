using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Arbitrader.GW2API.Entities;
using Arbitrader.GW2API.Results;

namespace Arbitrader.GW2API.Model
{
    /// <summary>
    /// Represents a single recipe and its relationships to the items used to craft it.
    /// </summary>
    public class Recipe
    {
        /// <summary>
        /// The unique identifier in the GW2 API for the recipe.
        /// </summary>
        private int _id;

        /// <summary>
        /// The type of the recipe.
        /// </summary>
        private RecipeType _type;

        /// <summary>
        /// The unique identifier in the GW2 API for the item that results from crafting the recipe.
        /// </summary>
        private Item _outputItem;

        /// <summary>
        /// The count of the output item that results from crafting the recipe.
        /// </summary>
        private int _outputItemCount;

        /// <summary>
        /// The discipline rating level required to craft the recipe.
        /// </summary>
        private int _minimumRating;

        /// <summary>
        /// Gets or sets the list of disciplines that are able to craft the recipe.
        /// </summary>
        private Collection<Discipline> _disciplines = new Collection<Discipline>();

        /// <summary>
        /// Gets or sets the list of flags assigned to the recipe.
        /// </summary>
        private Collection<Flag> _flags = new Collection<Flag>();

        /// <summary>
        /// Gets or sets the list of ingredients required to craft the recipe.
        /// </summary>
        public Collection<Item> Ingredients { get; set; } = new Collection<Item>();

        /// <summary>
        /// Initializes a new instance of <see cref="Recipe"/>. Uses the database context to resolve item IDs to instances of <see cref="Item"/>.
        /// </summary>
        /// <param name="recipeEntity">The entity containing the descriptors for the recipe.</param>
        /// <param name="context">The database context used to resolve item IDs to instances of <see cref="Item"/>.</param>
        public Recipe(RecipeEntity recipeEntity, ItemContext context)
        {
            this._type = Enum.TryParse(recipeEntity.Type, out RecipeType type) ? type : RecipeType.Unknown;
            this._outputItem = this.GetItem(recipeEntity.OutputItemID.Value, context.Items);
            this._outputItemCount = recipeEntity.OutputItemCount ?? 0;

            foreach (var disciplineResult in recipeEntity.Disciplines)
                this._disciplines.Add((Discipline)Enum.Parse(typeof(Discipline), disciplineResult.Name));

            foreach (var flagResult in recipeEntity.Flags)
                this._flags.Add((Flag)Enum.Parse(typeof(Flag), flagResult.Name));

            foreach (var ingredient in recipeEntity.Ingredients)
                this.Ingredients.Add(this.GetItem(ingredient.ItemID, context.Items));

            foreach (var ingredient in recipeEntity.GuildIngredients)
                this.Ingredients.Add(this.GetItem(ingredient.UpgradeID, context.Items));

            // update each ingredient's recipe list to allow travel up the crafting tree
            foreach (var ingredient in this.Ingredients)
                ingredient.DependentRecipes.Add(this);

            this._outputItem.GeneratingRecipes.Add(this);
        }

        /// <summary>
        /// Resolves a unique identifier in the GW2 API to an instance of <see cref="Item"/>.
        /// </summary>
        /// <param name="itemId">The unique identifier to be resolved.</param>
        /// <param name="items">The collection of <see cref="Item"/> to search for the given unique identifier.</param>
        /// <returns></returns>
        private Item GetItem(int itemId, IEnumerable<Item> items)
        {
            return items.Where(i => i.ID == itemId)
                        .First();
        }

        /// <summary>
        /// Returns a string representation of the recipe.
        /// </summary>
        /// <returns>A string representation of the recipe.</returns>
        public override string ToString()
        {
            return $"{this._outputItem} | {this._outputItemCount}";
        }
    }
}