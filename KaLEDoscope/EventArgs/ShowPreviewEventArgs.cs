using KaLEDoscope.ViewModel;
using System;
using System.Windows.Controls;

namespace KaLEDoscope
{
    public class ShowPreviewEventArgs : EventArgs
    {
        public PreviewViewModel ViewModel { get; internal set; }
        public UserControl PreviewControl { get; internal set; }
    }
}
