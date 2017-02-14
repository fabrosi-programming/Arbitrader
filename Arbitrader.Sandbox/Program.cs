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
        //static ManualResetEvent resetEvent = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            var menu = new ConsoleMenu("Select an option:", true);
            menu.AddOption('r', new ConsoleMenu.MenuOption()
            {
                Description = "Refresh trade and recipe data",
                Action = () => RefreshDataFromAPI()
            });

            menu.Display();
        }

        private static void RefreshDataFromAPI()
        {
            var context = new ItemContext();
            var client = new HttpClient();

            context.DataLoadStarted += (sender, e) => Console.WriteLine($"Started loading data for {e.Count} ids from resource \"{e.Resource}\"");
            context.DataLoadFinished += (sender, e) => Console.WriteLine($"Finished loading data from resource \"{e.Resource}\"");
            context.DataLoadStatusUpdate += (sender, e) => Console.WriteLine($"Resource: {e.Resource}\t\tCount: {e.Count}");

            var menu = new ConsoleMenu("Select data resource to refresh:");
            menu.AddOption('i', new ConsoleMenu.MenuOption()
            {
                Description = "Items",
                Action = () =>
                {
                    Console.WriteLine("Refreshing item data...");
                    context.InitializeAsync(client, ItemContext.Resource.Items).Wait();
                    Console.WriteLine("Done.");
                }
            });
            menu.AddOption('r', new ConsoleMenu.MenuOption()
            {
                Description = "Recipes",
                Action = () =>
                {
                    Console.WriteLine("Refreshing recipe data...");
                    context.InitializeAsync(client, ItemContext.Resource.Recipes).Wait();
                    Console.WriteLine("Done.");
                }
            });
            menu.AddOption('a', new ConsoleMenu.MenuOption()
            {
                Description = "All",
                Action = () =>
                {
                    Console.WriteLine("Refreshing all data...");
                    context.InitializeAsync(client, ItemContext.Resource.Items).Wait();
                    context.InitializeAsync(client, ItemContext.Resource.Recipes).Wait();
                    Console.WriteLine("Done.");
                }
            });

            menu.Display();
        }
    }
}