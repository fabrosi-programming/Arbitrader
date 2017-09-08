using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrader.Sandbox
{
    /// <summary>
    /// A menu in a console application that allows execution paths to be selected with single-character input.
    /// </summary>
    public class ConsoleMenu
    {
        /// <summary>
        /// A single option in a console menu.
        /// </summary>
        public class MenuOption
        {
            /// <summary>
            /// Gets or sets the description for the menu option for display to a user.
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Gets or sets the action executed when the menu option is invoked by a containing menu.
            /// </summary>
            public Action Action { get; set; }
        }

        /// <summary>
        /// The collection of options available when the menu is displayed. Each option is paired with
        /// a character that a user may use to select that option.
        /// </summary>
        private Dictionary<char, MenuOption> _menuOptions;

        /// <summary>
        /// Gets or sets the message displayed to the user before the list of options is written out.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the menu is the first menu displayed by the application
        /// or a descendant menu. This property only affects display of the hard-coded "x" option in the menu
        /// and does affect the Action of the "x" option.
        /// </summary>
        public bool IsTopLevel { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="ConsoleMenu"/>.
        /// </summary>
        /// <param name="header">The message displayed to the user before the list of options is written out.</param>
        /// <param name="isTopLevel">Indicates whether the menu is the first menu displayed by the application
        /// or a descendant menu.</param>
        public ConsoleMenu(string header, bool isTopLevel = false)
        {
            this._menuOptions = new Dictionary<char, MenuOption>();
            this.Header = header;
            this.IsTopLevel = isTopLevel;
        }

        /// <summary>
        /// Prints the menu header and all of its options to the console. Processes user input and handles invocation
        /// of menu options. Requests valid input from the user when invalid input is given.
        /// </summary>
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

        /// <summary>
        /// Adds an option to the menu.
        /// </summary>
        /// <param name="key">The key that the user presses to invoke the menu option.</param>
        /// <param name="menuOption">The option to be invoked when the user presses the indicated key.</param>
        public void AddOption(char key, MenuOption menuOption)
        {
            if (key == 'x' || key == 'X')
                throw new ArgumentException("x and X are reserved characters.");

            if (this._menuOptions.Keys.Contains(key))
                throw new InvalidOperationException($"Key {key} has already been assigned.");

            this._menuOptions.Add(key, menuOption);
        }
    }
}
