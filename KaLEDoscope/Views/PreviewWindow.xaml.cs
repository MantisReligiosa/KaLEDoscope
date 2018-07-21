using System.Windows;
using System.Windows.Controls;

namespace KaLEDoscope.Views
{
    /// <summary>
    /// Interaction logic for PreviewWindow.xaml
    /// </summary>
    public partial class PreviewWindow : Window
    {
        public PreviewWindow()
        {
            InitializeComponent();
        }

        public void LoadPreviewControl(UserControl previewControl)
        {
            previewBorder.Child = previewControl;
        }
    }
}
