using System.Data.Entity.Migrations;
using Arbitrader.GW2API.Entities;

namespace Arbitrader.GW2API.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ArbitraderEntities>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Configuration"/>.
        /// </summary>
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        /// <summary>
        /// Seeds a new database with test data.
        /// </summary>
        /// <param name="context">The entity context for the data to be seeded.</param>
        protected override void Seed(ArbitraderEntities context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
