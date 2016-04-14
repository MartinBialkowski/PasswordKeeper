using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PasswordsKeeper.ViewModel
{
    public class CommandViewModel : ViewModelBase
    {
        public ICommand Command { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandViewModel"/>
        /// </summary>
        /// <param name="displayName">Name displayed to user</param>
        /// <param name="command">Command to perform</param>
        public CommandViewModel(string displayName, ICommand command)
        {
            if(command == null)
            {
                throw new ArgumentNullException("command");
            }

            base.DisplayName = displayName;
            this.Command = command;
        }
    }
}
