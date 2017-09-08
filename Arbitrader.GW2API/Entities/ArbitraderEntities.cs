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
        public virtual DbSet<DisciplineEntity> Disciplines { get; set; }
    }
}