using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Arbitrader.GW2API.Entities
{
    /// <summary>
    /// Provides an interface for item, recipe, and market data stored in the Arbitrader SQL database.
    /// </summary>
    public class ArbitraderEntities : DbContext
    {
        // If you wish to target a different database and/or database provider, modify the 'DataModel' 
        // connection string in the application configuration file.
        public ArbitraderEntities() : base("name=ArbitraderEntities")
        { }

        /// <summary>
        /// The set of items and their properties.
        /// </summary>
        public virtual DbSet<ItemEntity> Items { get; set; }

        /// <summary>
        /// The set of flags assigned to each item in <see cref="Items"/>.
        /// </summary>
        public virtual DbSet<ItemFlagEntity> ItemFlags { get; set; }

        /// <summary>
        /// The set of recipes and their properties.
        /// </summary>
        public virtual DbSet<RecipeEntity> Recipes { get; set; }

        /// <summary>
        /// The set of flags assigned to each recipe in <see cref="Recipes"/>.
        /// </summary>
        public virtual DbSet<RecipeFlagEntity> RecipeFlags { get; set; }

        /// <summary>
        /// The set of guild ingredients and their properties.
        /// </summary>
        public virtual DbSet<GuildIngredientEntity> GuildIngredients { get; set; }

        /// <summary>
        /// The set of associations between recipes and the items required to craft them.
        /// </summary>
        public virtual DbSet<IngredientEntity> Ingredients { get; set; }

        /// <summary>
        /// The set of disciplines and the recipes that each discipline is able to craft.
        /// </summary>
        /// 
        public virtual DbSet<DisciplineEntity> Disciplines { get; set; }

        /// <summary>
        /// The set of market listings per item.
        /// </summary>
        public virtual DbSet<ListingEntity> Listings { get; set; }

        /// <summary>
        /// The set of individual price points listed on the market per item.
        /// </summary>
        public virtual DbSet<IndividualListingEntity> IndividualListings { get; set; }

        /// <summary>
        /// The set of items for which market listings are watched.
        /// </summary>
        public virtual DbSet<WatchedItem> WatchedItems { get; set; }

        /// <summary>
        /// Loads each set of entities from the SQL database.
        /// </summary>
        /// <param name="entities">An interface for item, recipe, and market data stored in the Arbitrader SQL database.</param>
        public void Load()
        {
            this.Items.Load();
            this.ItemFlags.Load();
            this.Recipes.Load();
            this.RecipeFlags.Load();
            this.GuildIngredients.Load();
            this.Ingredients.Load();
            this.Disciplines.Load();
            this.Listings.Load();
            this.IndividualListings.Load();
            this.WatchedItems.Load();
        }

        /// <summary>
        /// Deletes existing data from the SQL database. Respects foreign key relationships in that data.
        /// </summary>
        /// <param name="resource">The resource for which data is to be deleted.</param>
        /// <param name="entities">An interface for item, recipe, and market data stored in the Arbitrader SQL database.</param>
        public void Delete(APIResource resource)
        {
            if (resource == APIResource.Recipes || resource == APIResource.Items)
            {
                this.Disciplines.RemoveRange(this.Disciplines);
                this.RecipeFlags.RemoveRange(this.RecipeFlags);
                this.Ingredients.RemoveRange(this.Ingredients);
                this.GuildIngredients.RemoveRange(this.GuildIngredients);
                this.Recipes.RemoveRange(this.Recipes);
            }

            if (resource == APIResource.Items)
            {
                this.ItemFlags.RemoveRange(this.ItemFlags);
                this.Items.RemoveRange(this.Items);
            }

            this.SaveChanges();
        }

        /// <summary>
        /// Adds all items whose names contain the given pattern to the list of watched items.
        /// </summary>
        /// <param name="pattern">The string pattern to search for.</param>
        public void AddWatchedItems(string pattern)
        {
            if (!this.Items.Any())
                throw new InvalidOperationException("Unable to add watched items before items have been loaded into the database.");

            var existingWatchedIDs = this.WatchedItems.Select(i => i.APIID);
            var newWatchItems = this.Items.Where(i => i.Name.ToUpper().Contains(pattern.ToUpper()))
                                          .Where(i => !existingWatchedIDs.Contains(i.ID));

            foreach (var item in newWatchItems)
                this.WatchedItems.Add(new WatchedItem(item.APIID));

            this.SaveChanges();
        }

        /// <summary>
        /// Clears the entire list of watched items.
        /// </summary>
        public void ClearWatchedItems()
        {
            this.WatchedItems.RemoveRange(this.WatchedItems);
            this.SaveChanges();
        }

        /// <summary>
        /// Removes items from the list of watched items by checking their names against the given string pattern.
        /// </summary>
        /// <param name="pattern">The string pattern to search for.</param>
        /// <param name="substring">If true, the pattern may match only a substring of the names of items
        /// to be removed. If false, it must match the entire name.</param>
        public void RemoveWatchedItems(string pattern, bool substring = true)
        {
            if (!this.Items.Any())
                throw new InvalidOperationException("Unable to remove watched items before items have been loaded into the database.");

            IEnumerable<int> matchingIDs;

            if (substring)
                matchingIDs = this.Items.Where(i => i.Name.ToUpper().Contains(pattern.ToUpper()))
                                        .Select(i => i.APIID);
            else
                matchingIDs = this.Items.Where(i => String.Compare(i.Name, pattern, true) == 0)
                                        .Select(i => i.APIID);

            var watchItemsToRemove = this.WatchedItems.Where(i => matchingIDs.Contains(i.APIID));
            this.WatchedItems.RemoveRange(watchItemsToRemove);
            this.SaveChanges();
        }
    }
}