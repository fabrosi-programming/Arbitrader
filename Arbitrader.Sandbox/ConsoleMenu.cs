using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrader.Sandbox
{
    public class ConsoleMenu
    {
        public class MenuOption
        {
            public string Description { get; set; }
            public Action Action { get; set; }
        }

        private Dictionary<char, MenuOption> _menuOptions { get; set; }
        public string Header { get; set; }
        public bool IsTopLevel { get; set; }

        public ConsoleMenu(string header, bool isTopLevel = false)
        {
            this._menuOptions = new Dictionary<char, MenuOption>();
            this.Header = header;
            this.IsTopLevel = IsTopLevel;
        }

        public void Display()
        {
            var loop = true;

            while (loop)
            {
                Console.WriteLine(this.Header);

                foreach (var option in this._menuOptions.OrderBy(option => option.Key))
                    Console.WriteLine($"  ({option.Key}) : {option.Value.Description}");

                Console.WriteLine($"  (x) : {(this.IsTopLevel ? "Exit" : "Go back")}");
                Console.WriteLine();

                var input = Console.ReadKey(true).KeyChar;

                if (input == 'x')
                    loop = false;
                else if (this._menuOptions.ContainsKey(input))
                    this._menuOptions[input].Action.Invoke();
                else
                    Console.WriteLine("Invalid selection.");
            }
        }

        public void AddOption(char key, MenuOption menuOption)
        {
            if (key == 'x' || key == 'X')
                throw new ArgumentException("x and X are reserved characters.");

            this._menuOptions.Add(key, menuOption);
        }
    }
}
