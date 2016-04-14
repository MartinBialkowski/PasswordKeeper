using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Entity;
using DataAccessLayer;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PasswordsKeeper.Properties;
using GalaSoft.MvvmLight.Messaging;

namespace PasswordsKeeper.ViewModel
{
    public class RegistryViewModel : WorkspaceViewModel, IDataErrorInfo
    {
        private Registry registry;
        private RegistryDAO registryDAO;
        private WebSiteDAO webSiteDAO;
        private MailDAO mailDAO;
        private UsernameDAO usernameDAO;
        private PasswordDAO passwordDAO;
        private CategoryDAO categoryDAO;
        public string NewUsername { get; set; }
        public string NewPassword { get; set; }
        public ObservableCollection<WebSite> WebSiteList { get; private set; }
        public ObservableCollection<Mail> EmailList { get; private set; }
        public CommandViewModel SaveCommand { get; private set; }

        private CommonNotification msg;
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryViewModel"/>
        /// </summary>
        /// <param name="registry">Instance of <see cref="Registry"/></param>
        /// <param name="registryDAO">Data Access for <see cref="Registry"/></param>
        /// <param name="webSiteDAO">Data Access for <see cref="WebSite"/></param>
        /// <param name="mailDAO">Data Access for <see cref="Mail"/></param>
        /// <param name="usernameDAO">Data Access for <see cref="Username"/></param>
        /// <param name="passwordDAO">Data Access for <see cref="Password"/></param>
        /// <param name="categoryDAO">Data Access for <see cref="Category"/></param>
        public RegistryViewModel(Registry registry, RegistryDAO registryDAO, WebSiteDAO webSiteDAO,
            MailDAO mailDAO, UsernameDAO usernameDAO, PasswordDAO passwordDAO, CategoryDAO categoryDAO)
        {
            this.registry = registry;
            this.registryDAO = registryDAO;
            this.webSiteDAO = webSiteDAO;
            this.mailDAO = mailDAO;
            this.usernameDAO = usernameDAO;
            this.passwordDAO = passwordDAO;
            this.categoryDAO = categoryDAO;

            mailDAO.MailAdded += OnMailAddedToDB;
            webSiteDAO.WebSiteAdded += OnWebSiteAddedToDB;

            CreateCommands();
            LoadLists();
        }

        public int ID
        { 
            get
            {
                return registry.id;
            }
        }

        public Mail Mail
        {
            get
            {
                return registry.Mail;
            }
            set
            {
                if(registry.Mail != value)
                {
                    registry.Mail = value;

                    base.OnPropertyChanged();
                }
            }
        }

        public WebSite WebSite
        {
            get
            {
                return registry.WebSite;
            }
            set
            {
                if(registry.WebSite != value)
                {
                    registry.WebSite = value;

                    base.OnPropertyChanged();
                }
            }
        }

        public Username Username
        {
            get
            {
                return registry.Username;
            }
            set
            {
                if(registry.Username != value)
                {
                    registry.Username = value;

                    base.OnPropertyChanged();
                }
            }
        }

        public Password Password
        {
            get
            {
                return registry.Password;
            }
            set
            {
                if(registry.Password != value)
                {
                    registry.Password = value;

                    base.OnPropertyChanged();
                }
            }
        }

        public string Email
        {
            get
            {
                return Mail.username + "@" + Mail.provider;
            }
        }

        /// <summary>
        /// Initializes every command
        /// </summary>
        private void CreateCommands()
        {
            SaveCommand = new CommandViewModel(
                Resources.RegistryViewModel_Command_Save,
                new RelayCommand(param => Save()));
        }

        /// <summary>
        /// Calls validity method and saves it into database or updates if already exist in database
        /// </summary>
        private void Save()
        {
            if (IsInvalid())
            {
                return;
            }
            if (registryDAO.GetById(registry.id) != null)
            {
                registryDAO.Update(registry);
                msg = new CommonNotification(Resources.CommonNotification_RegistryUpdated);
            }
            else
            {
                registryDAO.Insert(registry);
                msg = new CommonNotification(Resources.CommonNotification_RegistryAdded);
            }
            Messenger.Default.Send<CommonNotification>(msg);
            registryDAO.Save();
            base.OnPropertyChanged("DisplayName");
        }
        
        /// <summary>
        /// Loads lists from database
        /// </summary>
        private void LoadLists()
        {
            List<WebSite> all = webSiteDAO.GetAll().ToList();
            WebSiteList = new ObservableCollection<WebSite>(all);

            List<Mail> all1 = mailDAO.GetAll().ToList();
            EmailList = new ObservableCollection<Mail>(all1);
        }

        /// <summary>
        /// Calls validation method and inserts new password into database if not exist yet.
        /// </summary>
        /// <returns>True if succeed</returns>
        private bool AddPassword()
        {
            if(String.IsNullOrWhiteSpace(NewPassword))
            {
                return false;
            }
            Password password = passwordDAO.GetAll()
                .FirstOrDefault(p => p.password1 == NewPassword);
            if(password == null)
            {
                password = new Password()
                {
                    password1 = NewPassword
                };
                passwordDAO.Insert(password);
                passwordDAO.Save();
            }

            Password = password;
            return true;
        }

        /// <summary>
        /// Checks validation and inserts new username into database if not exist yet.
        /// </summary>
        /// <returns>True if succeed</returns>
        private bool AddUsername()
        {
            if(String.IsNullOrWhiteSpace(NewUsername))
            {
                return false;
            }
            Username username = usernameDAO.GetAll()
                .FirstOrDefault(u => u.username1 == NewUsername);

            if(username == null)
            {
                username = new Username()
                {
                    username1 = NewUsername
                };
                usernameDAO.Insert(username);
                usernameDAO.Save();
            }
            
            Username = username;
            return true;
        }

        /// <summary>
        /// Checks if user picked items from combo boxes and calls method for adding username and password
        /// in case user didn't do that
        /// </summary>
        /// <returns>True if succeed</returns>
        private bool IsInvalid()
        {
            bool result = false;
            if(Mail == null || Mail.id == -1)
            {
                result = true;
                UserNotificationMessage msg = new UserNotificationMessage(Resources.UserNotificationMessage_ChooseE_Mail, 5);
                Messenger.Default.Send<UserNotificationMessage>(msg);
            }
            if(WebSite == null || WebSite.id == -1)
            {
                result = true;
                UserNotificationMessage msg = new UserNotificationMessage(Resources.UserNotificationMessage_ChooseWebService, 5);
                Messenger.Default.Send<UserNotificationMessage>(msg);
            }
            if (!AddUsername())
            {
                result = true;
            }
            if (!AddPassword())
            {
                result = true;
            }
            return result;
        }

        void OnWebSiteAddedToDB(object sender, DataAccessLayer.EventArg.WebSiteAddedEventArgs e)
        {
            WebSiteList.Add(e.NewWebSite);
        }

        void OnMailAddedToDB(object sender, DataAccessLayer.EventArg.MailAddedEventArgs e)
        {
            EmailList.Add(e.NewMail);
        }


        public string Error
        {
            get { return string.Empty; }
        }

        public string this[string columnName]
        {
            get
            {
                if (columnName == "NewUsername")
                {
                    if (string.IsNullOrWhiteSpace(NewUsername))
                    {
                        return Resources.UserNotificationMessage_UsernameEmpty;
                    }
                }
                if (columnName == "NewPassword")
                {
                    if (string.IsNullOrWhiteSpace(NewPassword))
                    {
                        return Resources.UserNotificationMessage_PasswordEmpty;
                    }
                }
                return string.Empty;
            }
        }
    }
}
