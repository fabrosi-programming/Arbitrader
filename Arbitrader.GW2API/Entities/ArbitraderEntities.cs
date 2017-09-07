namespace Arbitrader.GW2API.Entities
{
    using System.Data.Entity;
    using System.Linq;

    public class ArbitraderEntities : DbContext
    {
        // Your context has been configured to use a 'DataModel' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'Arbitrader.GW2API.DataModel' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'DataModel' 
        // connection string in the application configuration file.
        public ArbitraderEntities()
            : base("name=ArbitraderEntities")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<ItemEntity> Items { get; set; }
        public virtual DbSet<ItemFlagEntity> ItemFlags { get; set; }
        public virtual DbSet<RecipeEntity> Recipes { get; set; }
        public virtual DbSet<RecipeFlagEntity> RecipeFlags { get; set; }
        public virtual DbSet<GuildIngredientEntity> GuildIngredients { get; set; }
        public virtual DbSet<IngredientEntity> Ingredients { get; set; }
        public virtual DbSet<DisciplineEntity> Disciplines { get; set; }
    }
}