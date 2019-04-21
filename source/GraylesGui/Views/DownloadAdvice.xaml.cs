using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GraylesGui.Views
{
    public class DownloadAdvice : Window
    {
        public DownloadAdvice()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Close(object sender, object args)
        {
            this.Close();
        }
    }
}