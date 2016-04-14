using DataAccessLayer;
using Entity;
using PasswordsKeeper.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;

namespace PasswordsKeeper.ViewModel
{
    public class WebSiteViewModel : WorkspaceViewModel, IDataErrorInfo
    {
        private WebSite webSite;
        private WebSiteDAO dao;
        private CategoryDAO categoryDAO;
        private RelayCommand saveCommand;
        public string NewCategory { get; private set; }

        /// <summary>
        /// Command which adds new Category object.
        /// </summary>
        public CommandViewModel AddNewCategoryCommand { get; private set; }
        public ObservableCollection<Category> CategoryList { get; private set; }
        private CommonNotification msg;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSiteViewModel"/>
        /// </summary>
        /// <param name="webSite">WebSite Entity object</param>
        /// <param name="dao">Data Access Object for WebSite</param>
        /// <param name="categoryDAO">Data Access Object for Category</param>
        public WebSiteViewModel(WebSite webSite, WebSiteDAO dao, CategoryDAO categoryDAO)
        {
            base.DisplayName = Resources.WebSiteViewModel_DisplayName;
            this.webSite = webSite;
            this.dao = dao;
            this.categoryDAO = categoryDAO;

            AddNewCategoryCommand = new CommandViewModel(
                Resources.WebSiteViewModel_Command_AddNewCategoryCommand,
                new RelayCommand(param => AddCategory()));

            CategoryList = new ObservableCollection<Category>(categoryDAO.GetAll());
        }

        public int ID
        {
            get
            {
                return webSite.id;
            }
        }

        public string Address
        {
            get
            {
                return webSite.address;
            }
            set
            {
                if(value != webSite.address)
                {
                    webSite.address = value;

                    base.OnPropertyChanged();
                }
            }
        }

        public string Description
        { 
            get
            {
                return webSite.description;
            }
            set
            {
                if(value != webSite.description)
                {
                    webSite.description = value;

                    base.OnPropertyChanged();
                }
            }
        }

        public Category Category
        {
            get
            {
                return webSite.Category;
            }
            set
            {
                if (value != webSite.Category)
                {
                    webSite.Category = value;

                    base.OnPropertyChanged();
                }
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                if (saveCommand == null)
                {
                    saveCommand = new RelayCommand(
                        param => Save()
                        );
                }
                return saveCommand;
            }
        }

        /// <summary>
        /// Checks if exist, if not adds new Category
        /// </summary>
        private void AddCategory()
        {
            if(string.IsNullOrWhiteSpace(NewCategory))
            {
                UserNotificationMessage msg = new UserNotificationMessage(Resources.UserNotificationMessage_CategoryEmpty, 5);
                Messenger.Default.Send<UserNotificationMessage>(msg);
                return;
            }
            Category category = CategoryList.FirstOrDefault(c => c.name == NewCategory);
            if(category == null)
            {
                category = new Category()
                {
                    name = NewCategory,
                    isVisible = true
                };

                CategoryList.Add(category);
                categoryDAO.Insert(category); //FINALLY move it to save command
                categoryDAO.Save();
                Category = category;
                CommonNotification msg = new CommonNotification(Resources.CommonNotification_CategoryAdded);
                Messenger.Default.Send<CommonNotification>(msg);
            }
        }

        public override string DisplayName
        {
            get
            {
                return webSite.address;
            }
        }

        /// <summary>
        /// Calls validity method and saves it into database or updates if already exist in database
        /// </summary>
        private void Save()
        {
            AddCategory();
            if(IsInvalid())
            {
                return;
            }
            if (dao.GetById(webSite.id) != null)
            {
                dao.Update(webSite);
                msg = new CommonNotification(Resources.CommonNotification_WebsiteUpdated);
            }
            else
            {
                dao.Insert(webSite);
                msg = new CommonNotification(Resources.CommonNotification_WebsiteAdded);
            }
            dao.Save();
            Messenger.Default.Send<CommonNotification>(msg);

            base.OnPropertyChanged("DisplayName");
        }

        /// <summary>
        /// Checks if user filled every field in UI
        /// </summary>
        /// <returns></returns>
        private bool IsInvalid()
        {
            bool result = false;
            if(string.IsNullOrWhiteSpace(Address))
            {
                UserNotificationMessage msg = new UserNotificationMessage(Resources.UserNotificationMessage_AddressEmpty, 5);
                Messenger.Default.Send<UserNotificationMessage>(msg);
                result = true;
            }
            if(Category == null || Category.id == -1)
            {
                UserNotificationMessage msg = new UserNotificationMessage(Resources.UserNotificationMessage_CategoryEmpty, 5);
                Messenger.Default.Send<UserNotificationMessage>(msg);
                result = true;
            }
            return result;
        }

        public string Error
        {
            get { return string.Empty; }
        }

        public string this[string columnName]
        {
            get
            {
                if(columnName == "Address")
                {
                    if(string.IsNullOrWhiteSpace(Address))
                    {
                        return Resources.UserNotificationMessage_AddressEmpty;
                    }
                }
                return string.Empty;
            }
        }
    }
}
