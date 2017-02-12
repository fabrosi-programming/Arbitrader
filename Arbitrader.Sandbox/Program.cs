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
        static ManualResetEvent resetEvent = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            var loop = true;

            while (loop)
            {
                Console.WriteLine("Select an option:");
                Console.WriteLine("  (r) : Refresh trade and recipe data");
                Console.WriteLine("  (x) : Exit");
                Console.WriteLine();

                var input = Console.ReadKey(true).KeyChar.ToString().ToLower();

                switch (input)
                {
                    case "r":
                        RefreshDataFromAPI();
                        break;
                    case "x":
                        loop = false;
                        break;
                    default:
                        Console.WriteLine("Invalid selection.");
                        break;
                }
            }
        }

        private static void RefreshDataFromAPI()
        {
            Console.WriteLine("Refreshing trade and recipe data...");

            var context = new ItemContext();
            var client = new HttpClient();

            context.RaiseDataLoadStatusUpdate += (sender, e) =>
            {
                Console.WriteLine($"Resource: {e.Resource}\t\tCount: {e.Count}");
            };

            context.Initialize(client, resetEvent);

            resetEvent.WaitOne();

            Console.WriteLine("Done.");
        }
    }
}