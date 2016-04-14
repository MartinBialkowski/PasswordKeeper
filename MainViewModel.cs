using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using PasswordsKeeper.Properties;
using System.Collections.Specialized;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Data;
using DataAccessLayer;
using Entity;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace PasswordsKeeper.ViewModel
{
    /// <summary>
    /// ViewModel for application's main window.
    /// </summary>
    public class MainViewModel : WorkspaceViewModel
    {

        ObservableCollection<WorkspaceViewModel> workspaces;
        readonly RegistryDAO registryDAO;
        readonly MailDAO mailDAO;
        readonly WebSiteDAO webSiteDAO;
        readonly CategoryDAO categoryDAO;
        readonly UsernameDAO usernameDAO;
        readonly PasswordDAO passwordDAO;
        //E-Mail Commands
        public CommandViewModel AddEmailCommand { get; private set; }
        public CommandViewModel ShowAllEmailsCommand { get; private set; }
        public CommandViewModel ChangeEmailCommand { get; private set; }
        public CommandViewModel RemoveEmailCommand { get; private set; }
        //Website Commands
        public CommandViewModel AddWebSiteCommand { get; private set; }
        public CommandViewModel ShowAllWebSitesCommand { get; private set; }
        public CommandViewModel ChangeWebSiteCommand { get; private set; }
        public CommandViewModel RemoveWebSiteCommand { get; private set; }
        //Registry Commands
        public CommandViewModel AddRegistryCommand { get; private set; }
        public CommandViewModel ShowAllRegistersCommand { get; private set; }
        public CommandViewModel ChangeRegistryCommand { get; private set; }
        public CommandViewModel RemoveRegistryCommand { get; private set; }

        private string message;
        public string CommonMessage
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/>
        /// </summary>
        public MainViewModel()
        {
            base.DisplayName = Resources.MainViewModel_DisplayName;
            registryDAO = new RegistryDAO();
            mailDAO = new MailDAO();
            webSiteDAO = new WebSiteDAO();
            categoryDAO = new CategoryDAO();
            usernameDAO = new UsernameDAO();
            passwordDAO = new PasswordDAO();

            CreateCommands();

            Messenger.Default.Register<CommonNotification>(this, (msg) =>
                {
                    CommonMessage = msg.Message;
                });
        }

        /// <summary>
        /// Initializes every command field
        /// </summary>
        private void CreateCommands()
        {
            AddEmailCommand = new CommandViewModel(
                Resources.MainViewModel_Command_AddEmail,
                new RelayCommand(param => AddEmail()));

            ShowAllEmailsCommand = new CommandViewModel(
                Resources.MainViewModel_Command_ViewAllMails,
                new RelayCommand(param => ShowAllEmails()));

            ChangeEmailCommand = new CommandViewModel(
                Resources.MainViewModel_Command_ChangeEmail,
                new RelayCommand(param => ChangeEmail()));

            RemoveEmailCommand = new CommandViewModel(
                Resources.MainViewModel_Command_RemoveEmail,
                new RelayCommand(param => RemoveEmail()));

            AddWebSiteCommand = new CommandViewModel(
                Resources.MainViewModel_Command_AddWebSite,
                new RelayCommand(param => AddWebSite()));

            ShowAllWebSitesCommand = new CommandViewModel(
                Resources.MainViewModel_Command_AddWebSite,
                new RelayCommand(param => ShowAllWebSites()));

            ChangeWebSiteCommand = new CommandViewModel(
                Resources.MainViewModel_Command_ChangeWebSite,
                new RelayCommand(param => ChangeWebSite()));

            RemoveWebSiteCommand = new CommandViewModel(
                Resources.MainViewModel_Command_RemoveWebSite,
                new RelayCommand(param => RemoveWebSite()));

            AddRegistryCommand = new CommandViewModel(
                Resources.MainViewModel_Command_AddRegistry,
                new RelayCommand(param => AddRegistry()));

            ShowAllRegistersCommand = new CommandViewModel(
                Resources.MainViewModel_Command_ViewAllMails,
                new RelayCommand(param => ShowAllRegisters()));

            ChangeRegistryCommand = new CommandViewModel(
                Resources.MainViewModel_Command_ChangeRegistry,
                new RelayCommand(param => ChangeRegistry()));

            RemoveRegistryCommand = new CommandViewModel(
                Resources.MainViewModel_Command_RemoveRegistry,
                new RelayCommand(param => RemoveRegistry()));

        }

        /// <summary>
        /// Getter for list of workspaces
        /// </summary>
        public ObservableCollection<WorkspaceViewModel> Workspaces
        {
            get
            {
                if(workspaces == null)
                {
                    workspaces = new ObservableCollection<WorkspaceViewModel>();
                    workspaces.CollectionChanged += OnWorkspaceChanged;
                }
                return workspaces;
            }
        }

        /// <summary>
        /// Displays workspace, which adds new E-mail and allows modifing it
        /// </summary>
        private void AddEmail()
        {
            Mail mail = new Mail();
            EmailViewModel workspace = new EmailViewModel(mail, mailDAO);
            Workspaces.Add(workspace);
            SetActiveWorkspace(workspace);
        }

        /// <summary>
        /// Displays workspace with list of every E-mail
        /// </summary>
        private void ShowAllEmails()
        {
            EmailListViewModel workspace =
                Workspaces.FirstOrDefault(vm => vm is EmailListViewModel)
                as EmailListViewModel;

            if(workspace == null)
            {
                workspace = new EmailListViewModel(mailDAO);
                Workspaces.Add(workspace);
            }

            SetActiveWorkspace(workspace);
        }

        /// <summary>
        /// Displays workspace, which allows modifing selected E-mail
        /// </summary>
        private void ChangeEmail()
        {
            EmailListViewModel workspace = 
                Workspaces.FirstOrDefault(vm => vm is EmailListViewModel)
                as EmailListViewModel;

            if (workspace == null)
            {
                workspace = new EmailListViewModel(mailDAO);
                Workspaces.Add(workspace);
                SetActiveWorkspace(workspace);
            }
            if (workspace.SelectedMail == null)
            {
                ShowMessage(Resources.UserNotificationMessage_SelectItem);
                return;
            }

            EmailViewModel mailViewModel = workspace.SelectedMail;
            Workspaces.Add(mailViewModel);
            SetActiveWorkspace(mailViewModel);
        }

        /// <summary>
        /// Finds particular E-mail and remove it from workspaces and database
        /// </summary>
        private void RemoveEmail()
        {
            EmailListViewModel workspace =
                Workspaces.FirstOrDefault(vm => vm is EmailListViewModel)
                as EmailListViewModel;

            if (workspace == null)
            {
                workspace = new EmailListViewModel(mailDAO);
                Workspaces.Add(workspace);
                SetActiveWorkspace(workspace);                
            }
            if (workspace.SelectedMail == null)
            {
                ShowMessage(Resources.UserNotificationMessage_SelectItem);
                return;
            }

            workspace.Remove();
            
        }

        /// <summary>
        /// Displays new workspace, which adds new Web Site and allows
        /// modifing it
        /// </summary>
        private void AddWebSite()
        {
            WebSite webSite = new WebSite();
            WebSiteViewModel workspace = new WebSiteViewModel(webSite, webSiteDAO, categoryDAO);
            Workspaces.Add(workspace);
            SetActiveWorkspace(workspace);
        }

        /// <summary>
        /// Displays new workspace, which presents list of Web Sites
        /// </summary>
        private void ShowAllWebSites()
        {
            WebSiteListViewModel workspace = Workspaces.FirstOrDefault(vm => vm is WebSiteListViewModel)
                as WebSiteListViewModel;

            if(workspace == null)
            {
                workspace = new WebSiteListViewModel(webSiteDAO, categoryDAO);
                Workspaces.Add(workspace);
            }

            SetActiveWorkspace(workspace);
        }

        /// <summary>
        /// Displays new workspace, which allows modifing selected Web Site
        /// </summary>
        private void ChangeWebSite()
        {
            WebSiteListViewModel workspace =
                Workspaces.FirstOrDefault(vm => vm is WebSiteListViewModel)
                as WebSiteListViewModel;
            if(workspace == null)
            {
                workspace = new WebSiteListViewModel(webSiteDAO, categoryDAO);
                Workspaces.Add(workspace);
                SetActiveWorkspace(workspace);             
            }
            if(workspace.SelectedWebSite == null)
            {
                ShowMessage(Resources.UserNotificationMessage_SelectItem);
                return;
            }

            WebSiteViewModel webSiteViewModel = workspace.SelectedWebSite;
            Workspaces.Add(webSiteViewModel);
            SetActiveWorkspace(webSiteViewModel);
        }

        /// <summary>
        /// Finds particular Web Site and remove it from workspaces and database
        /// </summary>
        private void RemoveWebSite()
        {
            WebSiteListViewModel workspace =
                Workspaces.FirstOrDefault(vm => vm is WebSiteListViewModel)
                as WebSiteListViewModel;
            if (workspace == null)
            {
                workspace = new WebSiteListViewModel(webSiteDAO, categoryDAO);
                Workspaces.Add(workspace);
                SetActiveWorkspace(workspace);
            }
            if (workspace.SelectedWebSite == null)
            {
                ShowMessage(Resources.UserNotificationMessage_SelectItem);
                return;
            }

            workspace.Remove();
        }

        /// <summary>
        /// Displays new workspace, which adds new Registry and allows
        /// modifing it
        /// </summary>
        private void AddRegistry()
        {
            Registry registry = new Registry();
            RegistryViewModel workspace = new RegistryViewModel(registry, registryDAO, webSiteDAO, mailDAO,
                usernameDAO, passwordDAO, categoryDAO);
            Workspaces.Add(workspace);
            SetActiveWorkspace(workspace);
        }

        /// <summary>
        /// Displays new workspace, which presents list of Registers
        /// </summary>
        private void ShowAllRegisters()
        {
            RegistryListViewModel workspace =
                Workspaces.FirstOrDefault(vm => vm is RegistryListViewModel)
                as RegistryListViewModel;

            if (workspace == null)
            {
                workspace = new RegistryListViewModel(registryDAO, webSiteDAO, mailDAO,
                    usernameDAO, passwordDAO, categoryDAO);
                Workspaces.Add(workspace);
            }

            SetActiveWorkspace(workspace);
        }

        /// <summary>
        /// Displays new workspace, which allows modifing selected Registry
        /// </summary>
        private void ChangeRegistry()
        {
            RegistryListViewModel workspace =
                Workspaces.FirstOrDefault(vm => vm is RegistryListViewModel)
                as RegistryListViewModel;

            if(workspace == null)
            {
                workspace = new RegistryListViewModel(registryDAO, webSiteDAO, mailDAO,
                    usernameDAO, passwordDAO, categoryDAO);
                Workspaces.Add(workspace);
                SetActiveWorkspace(workspace);   
            }
            if (workspace.SelectedRegistry == null)
            {
                ShowMessage(Resources.UserNotificationMessage_SelectItem);
                return;
            }
            RegistryViewModel registryViewModel = workspace.SelectedRegistry;
            Workspaces.Add(registryViewModel);
            SetActiveWorkspace(registryViewModel);

        }

        /// <summary>
        /// Finds particular workspace and remove it from workspaces and database
        /// </summary>
        private void RemoveRegistry()
        {
            RegistryListViewModel workspace =
                Workspaces.FirstOrDefault(vm => vm is RegistryListViewModel)
                as RegistryListViewModel;
            if(workspace == null)
            {
                workspace = new RegistryListViewModel(registryDAO, webSiteDAO, mailDAO,
                    usernameDAO, passwordDAO, categoryDAO);
                Workspaces.Add(workspace);
                SetActiveWorkspace(workspace);
            }
            if (workspace.SelectedRegistry == null)
            {
                ShowMessage(Resources.UserNotificationMessage_SelectItem);
                return;
            }
            workspace.Remove();
        }

        /// <summary>
        /// Sends message for ToastList which is displayed in UI.
        /// </summary>
        /// <param name="msg">message to display</param>
        private void ShowMessage(string msg)
        {
            UserNotificationMessage message = new UserNotificationMessage(msg, 5);
            Messenger.Default.Send<UserNotificationMessage>(message);
        }

        /// <summary>
        /// Sets workspace sent as argument as active tab item
        /// </summary>
        /// <param name="workspace">ViewModel implementing WorkspaceViewModel</param>
        private void SetActiveWorkspace(WorkspaceViewModel workspace)
        {
            Debug.Assert(Workspaces.Contains(workspace));

            ICollectionView collectionView = CollectionViewSource.GetDefaultView(Workspaces);
            if (collectionView != null)
            {
                collectionView.MoveCurrentTo(workspace);
            }
        }

        void OnWorkspaceChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
            {
                foreach (WorkspaceViewModel workspace in e.NewItems)
                {
                    workspace.RequestClose += OnWorkspaceRequestClose;
                }
            }
            if (e.OldItems != null && e.OldItems.Count != 0)
            {
                foreach (WorkspaceViewModel workspace in e.OldItems)
                {
                    workspace.RequestClose -= OnWorkspaceRequestClose;
                }
            }
        }

        void OnWorkspaceRequestClose(object sender, EventArgs e)
        {
            WorkspaceViewModel workspace = sender as WorkspaceViewModel;
            workspace.Dispose();
            Workspaces.Remove(workspace);
        }
    }
}
