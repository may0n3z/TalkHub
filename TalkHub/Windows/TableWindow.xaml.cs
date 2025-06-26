using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TalkHub
{
    public partial class TableWindow : Window
    {
        public TableWindow()
        {
            InitializeComponent();
            DataContext = new TableViewModel();

            // Регистрируем обработчики для обоих ScrollViewer'ов
            LeftScrollViewer.PreviewMouseWheel += OnScrollViewerMouseWheel;
            RightScrollViewer.PreviewMouseWheel += OnScrollViewerMouseWheel;
        }

        private void OnScrollViewerMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer != null)
            {
                // Прокручиваем на величину e.Delta / 3
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta / 3.0);
                e.Handled = true;
            }
        }
    }
}
