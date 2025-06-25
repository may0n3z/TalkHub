using System.Windows;

namespace TalkHub
{
    public partial class UserInfoWindow : Window
    {
        public UserInfoWindow()
        {
            InitializeComponent();
            DataContext = new UserInfoViewModel();
        }
    }
}
