using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PasswordsKeeper.ViewModel
{
    public class UsernameViewModel : ViewModelBase
    {
        public ObservableCollection<Username> UsernameList { get; private set; }

    }
}
