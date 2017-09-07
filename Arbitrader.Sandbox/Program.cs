using Arbitrader.GW2API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arbitrader.Sandbox
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var menu = new ConsoleMenu("Select an option:", true);
            menu.AddOption('r', new ConsoleMenu.MenuOption()
            {
                Description = "Refresh trade and recipe data",
                Action = () => RefreshDataFromAPI(false)
            });
            menu.AddOption('n', new ConsoleMenu.MenuOption()
            {
                Description = "Replace trade and recipe data",
                Action = () => RefreshDataFromAPI(true)
            });

            menu.Display();
        }

        private static void RefreshDataFromAPI(bool replace)
        {
            var context = new ItemContext(continueOnError: true);
            var client = new HttpClient();

            context.DataLoadStarted += (sender, e) => Console.WriteLine($"Started loading data for {e.Count} ids from resource \"{e.Resource}\"");
            context.DataLoadFinished += (sender, e) => Console.WriteLine($"Finished loading data from resource \"{e.Resource}\"");
            context.DataLoadStatusUpdate += (sender, e) => Console.WriteLine($"Resource: {e.Resource}\t\tCount: {e.Count}\t\tMessage: {e.Message}");

            var menu = new ConsoleMenu(replace ? "Select data resource to replace:" : "Select data resource to refresh:");
            var action = replace ? "Replacing" : "Refreshing";
            menu.AddOption('i', new ConsoleMenu.MenuOption()
            {
                Description = "Items",
                Action = () =>
                {
                    Console.WriteLine($"{action} item data...");
                    context.Load(client, ItemContext.Resource.Items, replace);
                    Console.WriteLine("Done.");
                }
            });
            menu.AddOption('r', new ConsoleMenu.MenuOption()
            {
                Description = "Recipes",
                Action = () =>
                {
                    Console.WriteLine($"{action} recipe data...");
                    context.Load(client, ItemContext.Resource.Recipes, replace);
                    Console.WriteLine("Done.");
                }
            });
            menu.AddOption('a', new ConsoleMenu.MenuOption()
            {
                Description = "All",
                Action = () =>
                {
                    Console.WriteLine($"{action} all data...");
                    context.Load(client, ItemContext.Resource.Items, replace);
                    context.Load(client, ItemContext.Resource.Recipes, replace);
                    Console.WriteLine("Done.");
                }
            });

            menu.Display();
        }
    }
}