using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using PasswordsKeeper.Properties;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using Entity;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Data;

namespace PasswordsKeeper.ViewModel
{
    public class EmailListViewModel : WorkspaceViewModel
    {
        public ObservableCollection<EmailViewModel> AllMails { get; private set; }
        public CollectionView EmailCollectionView { get; private set; }
        public EmailViewModel SelectedMail { get; set; }
        public string SearchBar { get; set; }
        public CommandViewModel SearchCommand { get; private set; }
        private readonly MailDAO mailDAO;        
        private CommonNotification message;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailListViewModel"/>
        /// </summary>
        /// <param name="mailDAO">Data Access Object for <see cref="Entity.PasswordKeeper.Mail"/></param>
        public EmailListViewModel(MailDAO mailDAO)
        {
            base.DisplayName = Resources.AllMailsViewModel_DisplayName;
            this.mailDAO = mailDAO;

            this.mailDAO.MailAdded += OnMailAddedToDB;
            LoadMails();
            message = new CommonNotification(Resources.CommonNotification_MailDeleted);
            SearchCommand = new CommandViewModel(
                Resources.SharedCommands_Command_Search,
                new RelayCommand(param => UpdateView()));
        }

        /// <summary>
        /// Removes selected mail from DateBase and memory
        /// </summary>
        public void Remove()
        {
            if (SelectedMail != null)
            {
                mailDAO.Delete(SelectedMail.ID);
                AllMails.Remove(SelectedMail);
                mailDAO.Save();

                Messenger.Default.Send<CommonNotification>(message);
            }
        }

        /// <summary>
        /// Loads list from database and sets filter to collection
        /// </summary>
        private void LoadMails()
        {
            List<EmailViewModel> all = (from mail in mailDAO.GetAll()
                                       select new EmailViewModel(mail, mailDAO)).ToList();

            AllMails = new ObservableCollection<EmailViewModel>(all);
            EmailCollectionView = (CollectionView)CollectionViewSource.GetDefaultView(AllMails);
            EmailCollectionView.Filter = EmailFilter;
        }

        /// <summary>
        /// Filter for CollectionView of Emails,
        /// compares text in SearchBar field with e-mail address. Case insensitive.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool EmailFilter(object obj)
        {
            if(string.IsNullOrWhiteSpace(SearchBar))
            {
                return true;
            }
            else
            {
                return ((obj as EmailViewModel).Email.IndexOf(SearchBar, StringComparison.OrdinalIgnoreCase) >= 0);
                //return ((obj as EmailViewModel).Email.Contains(SearchBar)); //case sensitive
            }
        }

        /// <summary>
        /// Refreshes CollectionView
        /// </summary>
        private void UpdateView()
        {
            EmailCollectionView.Refresh();
        }

        void OnMailAddedToDB(object sender, DataAccessLayer.EventArg.MailAddedEventArgs e)
        {
            var viewModel = new EmailViewModel(e.NewMail, mailDAO);
            AllMails.Add(viewModel);
        }
    }
}
