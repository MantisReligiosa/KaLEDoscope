using BaseDevice;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace DeviceBuilding
{
    public class ControlsPack
    {
        public UserControl DesignPreviewControl { get; set; }
        public UserControl PreviewPreviewControl { get; set; }
        public Device Device { get; set; }
        public PreviewController PreviewController { get; set; }
        public UserControl CustomizationControl { get; set; }
        public IEnumerable<object> MenuItems { get; set; }
        public Action OnPreviewAreaMouseDown { get; set; }
        public event EventHandler DataChanged;
        public void NotifyThatModelChanged()
        {
            DataChanged?.Invoke(this, EventArgs.Empty);
        }
        public override bool Equals(object obj)
        {
            if (obj is ControlsPack controlsPack)
            {
                var result = controlsPack.Device.Id == Device.Id;
                return result;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Device.Id;
        }
    }
}
