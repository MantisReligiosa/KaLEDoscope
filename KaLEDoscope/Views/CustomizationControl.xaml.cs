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

namespace KaLEDoscope.Views
{
    /// <summary>
    /// Interaction logic for CustomizationControl.xaml
    /// </summary>
    public partial class CustomizationControl : UserControl
    {
        public CustomizationControl()
        {
            InitializeComponent();
        }

        internal void SetPreviewControl(UserControl previewControl)
        {
            grid.Children.Add(previewControl);
            Grid.SetRow(previewControl, 0);
        }

        internal void SetCustomizationControl(UserControl customizationControl)
        {
            grid.Children.Add(customizationControl);
            Grid.SetRow(customizationControl, 3);
        }

        internal void AddToolbarItems(IEnumerable<object> toolbarItems)
        {
            if (toolbarItems != null)
            {
                foreach (var item in toolbarItems)
                {
                    toolbar.Items.Add(item);
                }
            }
        }
    }
}
