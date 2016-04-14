using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PasswordsKeeper.View
{
    /// <summary>
    /// Interaction logic for AllEmailsView.xaml
    /// </summary>
    public partial class EmailListView : UserControl
    {
        public EmailListView()
        {
            InitializeComponent();

            //CollectionView viewEmails = (CollectionView)CollectionViewSource.GetDefaultView(dgEmails.ItemsSource);
            //viewEmails.Filter = EmailFilter;
        }

        //private bool EmailFilter(object obj)
        //{
        //    if (string.IsNullOrWhiteSpace(txtSearchBar.Text))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return ((obj as ).description.IndexOf(txtSearchTaskType.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        //    }
        //}

        private void txtSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(dgEmails.ItemsSource).Refresh();
        }
    }
}
