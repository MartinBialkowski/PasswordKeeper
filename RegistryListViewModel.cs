using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PasswordsKeeper.Properties;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Data;

namespace PasswordsKeeper.ViewModel
{
    public class RegistryListViewModel : WorkspaceViewModel
    {
        private RegistryDAO registryDAO;
        private WebSiteDAO webSiteDAO;
        private MailDAO mailDAO;
        private UsernameDAO usernameDAO;
        private PasswordDAO passwordDAO;
        private CategoryDAO categoryDAO;

        public ObservableCollection<RegistryViewModel> RegistryList { get; set; }
        public CollectionView RegistryView { get; private set; }
        public RegistryViewModel SelectedRegistry { get; set; }
        public string SearchBar { get; set; }
        public CommandViewModel SearchCommand { get; private set; }
        private CommonNotification msg;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryListViewModel"/>
        /// </summary>
        /// <param name="registryDAO">Data Access for <see cref="Registry"/></param>
        /// <param name="webSiteDAO">Data Access for <see cref="WebSite"/></param>
        /// <param name="mailDAO">Data Access for <see cref="Mail"/></param>
        /// <param name="usernameDAO">Data Access for <see cref="Username"/></param>
        /// <param name="passwordDAO">Data Access for <see cref="Password"/></param>
        /// <param name="categoryDAO">Data Access for <see cref="Category"/></param>
        public RegistryListViewModel(RegistryDAO registryDAO, WebSiteDAO webSiteDAO,
            MailDAO mailDAO, UsernameDAO usernameDAO, PasswordDAO passwordDAO, CategoryDAO categoryDAO)
        {
            base.DisplayName = Resources.RegistryViewModel_DisplayName;
            this.registryDAO = registryDAO;
            this.webSiteDAO = webSiteDAO;
            this.mailDAO = mailDAO;
            this.usernameDAO = usernameDAO;
            this.passwordDAO = passwordDAO;
            this.categoryDAO = categoryDAO;

            this.registryDAO.RegistryAdded += OnRegistryAddedToDB;
            LoadList();

            msg = new CommonNotification(Resources.CommonNotification_RegistryDeleted);
            SearchCommand = new CommandViewModel(
                Resources.SharedCommands_Command_Search,
                new RelayCommand(param => UpdateView()));
        }

        /// <summary>
        /// Refreshes CollectionView
        /// </summary>
        private void UpdateView()
        {
            RegistryView.Refresh();
        }

        /// <summary>
        /// Loads list from database and sets filter to CollectionView
        /// </summary>
        private void LoadList()
        {
            List<RegistryViewModel> all = (from registry in registryDAO.GetAll()
                                           select new RegistryViewModel(registry, registryDAO, webSiteDAO,
                                               mailDAO, usernameDAO, passwordDAO, categoryDAO))
                                               .ToList();

            RegistryList = new ObservableCollection<RegistryViewModel>(all);
            RegistryView = (CollectionView)CollectionViewSource.GetDefaultView(RegistryList);
            RegistryView.Filter = RegistryFilter;
        }

        /// <summary>
        /// Filter for CollectionView, compares text in SearchBar with e-mail address,
        /// web site address and username used to registration
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool RegistryFilter(object obj)
        {
            if(string.IsNullOrWhiteSpace(SearchBar))
            {
                return true;
            }
            else
            {
                var registryTmp = (obj as RegistryViewModel);
                return ((registryTmp.Email.IndexOf(SearchBar, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    registryTmp.WebSite.address.IndexOf(SearchBar, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    registryTmp.Username.username1.IndexOf(SearchBar, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        /// <summary>
        /// Removes selected registry from database and memory
        /// </summary>
        public void Remove()
        {
            if(SelectedRegistry != null)
            {
                registryDAO.Delete(SelectedRegistry.ID);
                RegistryList.Remove(SelectedRegistry);
                registryDAO.Save();

                Messenger.Default.Send<CommonNotification>(msg);
            }
        }

        void OnRegistryAddedToDB(object sender, DataAccessLayer.EventArg.RegistryAddedEventArgs e)
        {
            var viewModel = new RegistryViewModel(e.NewRegistry, registryDAO, webSiteDAO,
                                               mailDAO, usernameDAO, passwordDAO, categoryDAO);
            RegistryList.Add(viewModel);
        }
    }
}
