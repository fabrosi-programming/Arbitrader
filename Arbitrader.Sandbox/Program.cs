﻿using Arbitrader.GW2API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            var menu = new ConsoleMenu("Select an option:", true);
            menu.AddOption('r', new ConsoleMenu.MenuOption()
            {
                Description = "Get missing data from API",
                Action = () => RefreshDataFromAPI(false)
            });
            menu.AddOption('n', new ConsoleMenu.MenuOption()
            {
                Description = "Replace existing data",
                Action = () => RefreshDataFromAPI(true)
            });
            menu.AddOption('w', new ConsoleMenu.MenuOption()
            {
                Description = "Modify watched items",
                Action = () => ModifyWatchedItems()
            });

            menu.Display();
        }

        /// <summary>
        /// Displays a menu that allows the refreshing or replacement of recipe and item data from the GW2 API.
        /// </summary>
        /// <param name="replace">Indicates whether the user selection will append to or replace existing data.</param>
        private static void RefreshDataFromAPI(bool replace)
        {
            var context = new ItemContext();
            var client = new HttpClient();

            context.DataLoadStarted += (sender, e) => Console.WriteLine($"Started loading data for {e.Count} ids from resource \"{e.Resource}\"");
            context.DataLoadFinished += (sender, e) => Console.WriteLine($"Finished loading data from resource \"{e.Resource}\"");
            context.DataLoadStatusUpdate += (sender, e) => Console.WriteLine($"Resource: {e.Resource}\t\tCount: {e.Count}\t\tMessage: {e.Message}");

            var menu = new ConsoleMenu($"Select data resource to {(replace ? "replace" : "refresh")}:");
            var action = replace ? "Replacing" : "Refreshing";

            menu.AddOption('i', new ConsoleMenu.MenuOption()
            {
                Description = "Items",
                Action = () =>
                {
                    var startTime = DateTime.Now;
                    Console.WriteLine($"{action} item data...");

                    context.Load(client, APIResource.Items, replace);

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

                    context.Load(client, APIResource.Recipes, replace);

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

                    context.Load(client, APIResource.CommerceListings, replace);

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

                    context.Load(client, APIResource.Items, replace);
                    context.Load(client, APIResource.Recipes, replace);
                    context.Load(client, APIResource.CommerceListings, replace);

                    var elapsed = DateTime.Now - startTime;
                    Console.WriteLine($"Done. ({elapsed.TotalSeconds} s)");
                }
            });

            menu.Display();
        }

        private static void ModifyWatchedItems()
        {
            var context = new ItemContext();

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
                        context.RemoveWatchedItem(pattern);
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
                        context.RemoveWatchedItem(name, false);
                    }
                    catch (InvalidOperationException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            });

            menu.Display();
        }
    }
}