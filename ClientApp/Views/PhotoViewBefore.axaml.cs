using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ClientApp.Views
{
    public partial class PhotoViewBefore : UserControl
    {
        public PhotoViewBefore()
        {
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
