using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace DeviceBuilding
{
    public class ControlsPack
    {
        public UserControl PreviewControl { get; set; }
        public UserControl CustomizationControl { get; set; }
        public IEnumerable<object> MenuItems { get; set; }
        public Action OnPreviewAreaMouseDown { get; set; }
        public event EventHandler DataChanged;
        public void NotifyThatModelChanged()
        {
            DataChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
