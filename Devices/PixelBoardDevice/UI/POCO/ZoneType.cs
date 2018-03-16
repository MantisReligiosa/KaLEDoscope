using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelBoardDevice.UI.POCO
{
    public class ZoneType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool AllowText { get; set; }
        public bool AllowFont { get; set; }
        public bool AllowAnimation { get; set; }
        public bool AllowBitmap { get; set; }
    }
}
