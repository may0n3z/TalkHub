using System.Windows;
using System.Windows.Controls;

namespace TalkHub
{
    public partial class AddUser : Window
    {
        private AddUserViewModel ViewModel => DataContext as AddUserViewModel;

        public AddUser()
        {
            InitializeComponent();
            DataContext = new AddUserViewModel();
        }

        // PasswordBox не поддерживает прямую привязку к Password, поэтому обновляем ViewModel вручную
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null && sender is PasswordBox passwordBox)
            {
                ViewModel.Password = passwordBox.Password;
            }
        }
    }
}
