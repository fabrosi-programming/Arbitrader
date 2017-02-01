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
    class Program
    {
        static ManualResetEvent resetEvent = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            var context = new ItemContext();
            var client = new HttpClient();
            context.Initialize(client, resetEvent);

            resetEvent.WaitOne();
        }
    }
}