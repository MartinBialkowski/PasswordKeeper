using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PasswordsKeeper.Properties;
using DataAccessLayer.EventArg;

namespace PasswordsKeeper.ViewModel
{
    /// <summary>
    /// Represent a container of RegistryViewModel objects.
    /// </summary>
    public class AllRegistersViewModel : WorkspaceViewModel
    {

        readonly RegistryDAO _registryDAO;

        public AllRegistersViewModel(RegistryDAO registryDAO)
        {
            base.DisplayName = Resources.AllRegistersViewModel_DisplayName;

            _registryDAO = registryDAO;
            _registryDAO.RegistryAdded += OnRegistryAddedToDB;


        }

        private void OnRegistryAddedToDB(object sender, RegistryAddedEventArgs e)
        {

        }

    }

    //IMPORTANT do it after RegistryViewModel is done
}
