using SmartTechnologiesM.Base.Extensions;
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
            previewArea.Content = previewControl;
        }

        internal void SetCustomizationControl(UserControl customizationControl)
        {
            controlArea.Content = customizationControl;
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
