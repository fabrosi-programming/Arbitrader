using Arbitrader.GW2API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Arbitrader.GW2API.Model;

namespace Arbitrader.Sandbox
{
    /// <summary>
    /// Provides a console-based interface to allow interactions with Arbitrader functionality. Will later
    /// be supplanted by a GUI.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The entry point of the console application.
        /// </summary>
        /// <param name="args">Command line arguments passed in the invocation of the application.</param>
        public static void Main(string[] args)
        {
            var context = new ItemContext();

            var menu = new ConsoleMenu("Select an option:", true);
            menu.AddOption('r', new ConsoleMenu.MenuOption()
            {
                Description = "Get missing data from API",
                Action = () => RefreshDataFromAPI(context, false)
            });
            menu.AddOption('n', new ConsoleMenu.MenuOption()
            {
                Description = "Replace existing data",
                Action = () => RefreshDataFromAPI(context, true)
            });
            menu.AddOption('w', new ConsoleMenu.MenuOption()
            {
                Description = "Modify watched items",
                Action = () => ModifyWatchedItems(context)
            });
            menu.AddOption('p', new ConsoleMenu.MenuOption()
            {
                Description = "Find the cheapest price to acquire an item",
                Action = () => FindCheapestPrice(context)
            });

            menu.Display();
        }

        /// <summary>
        /// Displays a menu that allows the refreshing or replacement of recipe and item data from the GW2 API.
        /// </summary>
        /// <param name="replace">Indicates whether the user selection will append to or replace existing data.</param>
        private static void RefreshDataFromAPI(ItemContext context, bool replace)
        {
            var menu = new ConsoleMenu($"Select data resource to {(replace ? "replace" : "refresh")}:");
            var action = replace ? "Replacing" : "Refreshing";

            menu.AddOption('i', new ConsoleMenu.MenuOption()
            {
                Description = "Items",
                Action = () =>
                {
                    var startTime = DateTime.Now;
                    Console.WriteLine($"{action} item data...");

                    context.Load(APIResource.Items, replace);

                    var elapsed = DateTime.Now - startTime;
                    Console.WriteLine($"Done. ({elapsed.TotalSeconds} s)");
                }
            });
            menu.AddOption('r', new ConsoleMenu.MenuOption()
            {
                Description = "Recipes",
                Action = () =>
                {
                    var startTime = DateTime.Now;
                    Console.WriteLine($"{action} recipe data...");

                    context.Load(APIResource.Recipes, replace);

                    var elapsed = DateTime.Now - startTime;
                    Console.WriteLine($"Done. ({elapsed.TotalSeconds} s)");
                }
            });
            menu.AddOption('m', new ConsoleMenu.MenuOption()
            {
                Description = "Market Listings",
                Action = () =>
                {
                    var startTime = DateTime.Now;
                    Console.WriteLine($"{action} market listings...");

                    context.Load(APIResource.CommerceListings, replace);

                    var elapsed = DateTime.Now - startTime;
                    Console.WriteLine($"Done. ({elapsed.TotalSeconds} s)");
                }
            });
            menu.AddOption('a', new ConsoleMenu.MenuOption()
            {
                Description = "All",
                Action = () =>
                {
                    var startTime = DateTime.Now;
                    Console.WriteLine($"{action} all data...");

                    context.Load(APIResource.Items, replace);
                    context.Load(APIResource.Recipes, replace);
                    context.Load(APIResource.CommerceListings, replace);

                    var elapsed = DateTime.Now - startTime;
                    Console.WriteLine($"Done. ({elapsed.TotalSeconds} s)");
                }
            });

            menu.Display();
        }

        private static void ModifyWatchedItems(ItemContext context)
        {
            var menu = new ConsoleMenu("Select an option:");
            menu.AddOption('a', new ConsoleMenu.MenuOption()
            {
                Description = "Add watched items by a search pattern",
                Action = () =>
                {
                    Console.WriteLine("Enter a search pattern:");
                    var pattern = Console.ReadLine();

                    try
                    {
                        context.AddWatchedItems(pattern);
                    }
                    catch (InvalidOperationException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            });
            menu.AddOption('c', new ConsoleMenu.MenuOption()
            {
                Description = "Clear watched items",
                Action = () => context.ClearWatchedItems()
            });
            menu.AddOption('r', new ConsoleMenu.MenuOption()
            {
                Description = "Remove watched item by a search pattern",
                Action = () =>
                {
                    Console.WriteLine("Enter a search pattern:");
                    var pattern = Console.ReadLine();

                    try
                    {
                        context.RemoveWatchedItems(pattern);
                    }
                    catch (InvalidOperationException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            });
            menu.AddOption('m', new ConsoleMenu.MenuOption()
            {
                Description = "Remove watched item by an exact match",
                Action = () =>
                {
                    Console.WriteLine("Enter the name of the watched item to remove:");
                    var name = Console.ReadLine();

                    try
                    {
                        context.RemoveWatchedItem(name);
                    }
                    catch (InvalidOperationException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            });

            menu.Display();
        }

        private static void FindCheapestPrice(ItemContext context)
        {
            Console.WriteLine("Item name:");
            var itemName = Console.ReadLine();

            var parsed = false;
            int count = 0;
            while (!parsed)
            {
                Console.WriteLine("Number required:");
                parsed = Int32.TryParse(Console.ReadLine(), out count);
            }

            try
            {
                AcquisitionPlan plan;
                var price = context.GetLowestPrice(itemName, count, out plan);
                Console.WriteLine($"Best price: {price}"); //TODO: indicate market; indicate price path

                foreach (var step in plan)
                    Console.WriteLine(step);
            }
            catch (InvalidOperationException e) //TODO: use bespoke exception type
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}