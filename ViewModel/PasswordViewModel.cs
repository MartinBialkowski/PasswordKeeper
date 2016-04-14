using Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordsKeeper.ViewModel
{
    public class PasswordViewModel : ViewModelBase
    {
        public ObservableCollection<Password> PasswordList { get; private set; }

    }
}
