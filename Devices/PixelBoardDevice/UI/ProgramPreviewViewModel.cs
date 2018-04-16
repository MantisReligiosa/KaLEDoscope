using Abstractions;
using Extensions;
using System.Windows.Controls;

namespace PixelBoardDevice.UI
{
    public class ProgramPreviewViewModel : Notified
    {
        private readonly PixelDeviceViewModel _model;
        public Grid PreviewContent { get; set; }

        public ProgramPreviewViewModel(PixelDeviceViewModel pixelDeviceViewModel)
        {
            _model = pixelDeviceViewModel;
            _model.PropertyChanged += (s, e) => Redraw();
            Redraw();
        }

        private void Redraw()
        {
            if (_model.SelectedProgram.IsNull())
            {
                return;
            }
        }
    }
}