using Extensions;
using System.Collections.Generic;
using System.Windows.Controls;

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
            if (!toolbarItems.IsNull())
            {
                foreach (var item in toolbarItems)
                {
                    toolbar.Items.Add(item);
                }
            }
        }
    }
}
