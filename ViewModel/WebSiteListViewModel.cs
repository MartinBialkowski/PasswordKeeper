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
    public class WebSiteListViewModel : WorkspaceViewModel
    {
        private readonly WebSiteDAO webSiteDAO;
        private readonly CategoryDAO categoryDAO;
        public ObservableCollection<WebSiteViewModel> WebSiteList { get; private set; }
        public CollectionView WebSiteView { get; private set; }
        public string SearchBar { get; set; }
        public CommandViewModel SearchCommand { get; private set; }
        public WebSiteViewModel SelectedWebSite { get; set; }
        private CommonNotification msg;
        /// <summary>
        /// Initializes a new instance of the <see cref="WebSiteListViewModel"/>
        /// </summary>
        /// <param name="dao">Data Access Object for WebSite</param>
        /// <param name="cdao">Data Access Object for Category</param>
        public WebSiteListViewModel(WebSiteDAO dao, CategoryDAO cdao)
        {
            base.DisplayName = Resources.WebSiteListViewModel_DisplayName;
            webSiteDAO = dao;
            categoryDAO = cdao;
            webSiteDAO.WebSiteAdded += OnWebSiteAddedToDB;
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
            WebSiteView.Refresh();
        }

        /// <summary>
        /// Loads list from database
        /// </summary>
        private void LoadList()
        {
            List<WebSiteViewModel> all = (from webSite in webSiteDAO.GetAll()
                                          select new WebSiteViewModel(webSite, webSiteDAO, categoryDAO))
                                          .ToList();

            WebSiteList = new ObservableCollection<WebSiteViewModel>(all);
            WebSiteView = (CollectionView)CollectionViewSource.GetDefaultView(WebSiteList);
            WebSiteView.Filter = WebSiteFilter;
        }

        /// <summary>
        /// Filter for CollectionView of Web Sites,
        /// compares text in SearchBar field with web site address, description and category
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool WebSiteFilter(object obj)
        {
            if(string.IsNullOrWhiteSpace(SearchBar))
            {
                return true;
            }
            else
            {
                var webSiteTmp = (obj as WebSiteViewModel);
                return (webSiteTmp.Address.IndexOf(SearchBar, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    webSiteTmp.Description.IndexOf(SearchBar, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    webSiteTmp.Category.name.IndexOf(SearchBar, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        /// <summary>
        /// Removes selected object from DataBase and memory
        /// </summary>
        public void Remove()
        {
            if (SelectedWebSite != null)
            {
                webSiteDAO.Delete(SelectedWebSite.ID);
                WebSiteList.Remove(SelectedWebSite);
                webSiteDAO.Save();

                Messenger.Default.Send<CommonNotification>(msg);
            }
        }

        void OnWebSiteAddedToDB(object sender, DataAccessLayer.EventArg.WebSiteAddedEventArgs e)
        {
            var viewModel = new WebSiteViewModel(e.NewWebSite, webSiteDAO, categoryDAO);
            WebSiteList.Add(viewModel);
        }
    }
}
