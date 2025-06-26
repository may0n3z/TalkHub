using System;
using System.Windows;
using System.Windows.Input;

namespace TalkHub
{
    public class MainWindowViewModel
    {
        public ICommand LoginCommand { get; }

        public MainWindowViewModel()
        {
            LoginCommand = new RelayCommand(ExecuteLogin);
        }

        private void ExecuteLogin(object parameter)
        {
            // Создаем и показываем окно TableWindow
            TableWindow TableWindow = new TableWindow();
            TableWindow.Show();

            // Закрываем текущее окно
            if (parameter is Window currentWindow)
            {
                currentWindow.Close();
            }
        }
    }
}
