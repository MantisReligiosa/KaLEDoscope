using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelBoardDevice.DTO
{
    public class ReformatTextResult
    {
        internal int NeededHeight;

        public bool NeedToResize { get; internal set; }
        public bool NeedUndo { get; internal set; }
        public string Text { get; internal set; }
    }
}
