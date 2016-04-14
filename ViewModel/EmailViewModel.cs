using Entity;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Text.RegularExpressions;
using GalaSoft.MvvmLight.Messaging;
using System.ComponentModel;
using PasswordsKeeper.Properties;

namespace PasswordsKeeper.ViewModel
{
    public class EmailViewModel : WorkspaceViewModel, IDataErrorInfo
    {
        private Mail mail;
        private MailDAO mailDAO;
        private RelayCommand saveCommand;
        private CommonNotification msg;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailViewModel"/>
        /// </summary>
        /// <param name="mail">Instance of <see cref="Mail"/></param>
        /// <param name="dao">Data Access Object for <see cref="Mail"/></param>
        public EmailViewModel(Mail mail, MailDAO dao)
        {
            this.mail = mail;
            mailDAO = dao;
        }

        public int ID
        {
            get
            {
                return mail.id;
            }
        }
        /// <summary>
        /// First part of e-mail. Text before (at) sign.
        /// </summary>
        public string Username
        {
            get
            {
                return mail.username;
            }
            set
            {
                if(value != mail.username)
                {
                    mail.username = value;

                    base.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Second part of e-mail. Text after (at) sign.
        /// </summary>
        public string Provider
        {
            get
            {
                return mail.provider;
            }
            set
            {
                if(value != mail.provider)
                {
                    mail.provider = value;

                    base.OnPropertyChanged();
                }
            }
        }

        public string Password
        {
            get
            {
                return mail.password;
            }
            set
            {
                if(value != mail.password)
                {
                    mail.password = value;

                    base.OnPropertyChanged();
                }
            }
        }

        public string Email
        {
            get
            {
                return mail.username + "@" + mail.provider;
            }
        }

        public override string DisplayName
        {
            get
            {
                return Email;
            }
        }

        /// <summary>
        /// Saves e-mail on database and memory.
        /// </summary>
        public ICommand SaveCommand
        {
            get
            {
                if(saveCommand == null)
                {
                    saveCommand = new RelayCommand(
                        param => this.Save()
                        );
                }
                return saveCommand;
            }
        }

        /// <summary>
        /// Calls validity method and saves it into database or updates if already exist in database
        /// </summary>
        private void Save()
        {
            if(IsInvalid())
            {
                return;
            }
            if (mailDAO.GetById(mail.id) != null)
            {
                mailDAO.Update(mail);               
                msg = new CommonNotification(Resources.CommonNotification_MailUpdated);
            }
            else
            {
                mailDAO.Insert(mail);
                msg = new CommonNotification(Resources.CommonNotification_MailAdded);
            }
            mailDAO.Save();
            Messenger.Default.Send<CommonNotification>(msg);
            base.OnPropertyChanged("DisplayName");
        }

        /// <summary>
        /// Checks validity of data provided by user
        /// </summary>
        /// <returns>ture if data is invalid</returns>
        private bool IsInvalid()
        {
            bool result = false;
            if (string.IsNullOrWhiteSpace(Username))
            {
                UserNotificationMessage msg = new UserNotificationMessage(Resources.UserNotificationMessage_UsernameEmpty, 5);
                Messenger.Default.Send<UserNotificationMessage>(msg);
                result = true;
            }
            if (string.IsNullOrWhiteSpace(Provider))
            {
                UserNotificationMessage msg = new UserNotificationMessage(Resources.UserNotificationMessage_ProviderEmpty, 5);
                Messenger.Default.Send<UserNotificationMessage>(msg);
                result = true;
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                UserNotificationMessage msg = new UserNotificationMessage(Resources.UserNotificationMessage_PasswordEmpty, 5);
                Messenger.Default.Send<UserNotificationMessage>(msg);
                result = true;
            }
            if (ValidateEmail())
            {
                UserNotificationMessage msg = new UserNotificationMessage(Resources.Validation_ProviderNotValid, 5);
                Messenger.Default.Send<UserNotificationMessage>(msg);
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Checks validation of email using pattern
        /// </summary>
        /// <returns></returns>
        private bool ValidateEmail()
        {   
            if(Provider == null)
            {
                return true;
            }
            string pattern = @"[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

            return !Regex.IsMatch(Provider, pattern, RegexOptions.IgnoreCase);
        }

        public string Error
        {
            get 
            { 
                return string.Empty; 
            }
        }

        public string this[string columnName]
        {
            get
            {
                if(columnName == "Provider")
                {
                    if(ValidateEmail())
                    {
                        return Resources.Validation_ProviderNotValid;
                    }
                }
                if(columnName == "Username")
                {
                    if (string.IsNullOrWhiteSpace(Username))
                    {
                        return Resources.UserNotificationMessage_UsernameEmpty;
                    }
                }
                if(columnName == "Password")
                {
                    if (string.IsNullOrWhiteSpace(Password))
                    {
                        return Resources.UserNotificationMessage_PasswordEmpty;
                    }
                }
                return string.Empty;
            }
        }
    }
}
