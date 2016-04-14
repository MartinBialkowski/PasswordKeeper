using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace PasswordsKeeper.ViewModel
{
    public class ToastListViewModel : ViewModelBase
    {
        public ObservableCollection<MessageForListBox> Messages { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToastListViewModel"/>
        /// </summary>
        public ToastListViewModel()
        {
            Messages = new ObservableCollection<MessageForListBox>();
            Messenger.Default.Register<UserNotificationMessage>(this, (action) => AddMessage(action));
        }

        /// <summary>
        /// Adds message to collection of messages
        /// </summary>
        /// <param name="msg">Message to display</param>
        private async void AddMessage(UserNotificationMessage msg)
        {
            MessageForListBox messageForListBox = new MessageForListBox { Message = msg.Message, IsGoing = false };
            Messages.Insert(0, messageForListBox);
            await Task.Delay(new TimeSpan(0, 0, msg.Seconds));
            messageForListBox.IsGoing = true;
            await Task.Delay(new TimeSpan(0, 0, 0, 1, 300));
            Messages.Remove(messageForListBox);
        }
    }

    public class MessageForListBox : ViewModelBase
    {
        public string Message { get; set; }
        private bool isGoing;

        public bool IsGoing
        {
            get
            {
                return isGoing;
            }
            set
            {
                if(isGoing == value)
                {
                    isGoing = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
