using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DeviceBuilding
{
    public class ControlsPack
    {
        public UserControl PreviewControl { get; set; }
        public UserControl CustomizationControl { get; set; }
        public IEnumerable<object> MenuItems { get; set; }
    }
}
