using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PasswordsKeeper.ViewModel
{
    /// <summary>
    /// This ViewModelBase subclass requests to be removed from the UI
    /// when its CloseCommand executes.
    /// </summary>
    public abstract class WorkspaceViewModel : ViewModelBase
    {

        RelayCommand _closeCommand;

        protected WorkspaceViewModel()
        {

        }

        /// <summary>
        /// Returns the command that, when invoked, attempts
        /// to remove this workspace from the user interface.
        /// </summary>
        public ICommand CloseCommand
        {
            get
            {
                if(_closeCommand == null)
                {
                    _closeCommand = new RelayCommand(param => OnRequestClose());
                }

                return _closeCommand;
            }
        }

        /// <summary>
        /// Raised when this workspace should be removed from the UI.
        /// </summary>
        public event EventHandler RequestClose;

        protected void OnRequestClose()
        {
            if(RequestClose != null)
            {
                RequestClose(this, EventArgs.Empty);
            }
        }
    }
}
