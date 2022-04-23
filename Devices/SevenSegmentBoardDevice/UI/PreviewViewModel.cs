using SmartTechnologiesM.Base;
using System.Windows;

namespace SevenSegmentBoardDevice.UI
{
    public class PreviewViewModel : Notified
    {
        public string Text { get; set; }

        private bool _isDigit;
        public bool IsDigit
        {
            get
            {
                return _isDigit;
            }
            set
            {
                _isDigit = value;
                OnPropertyChanged(nameof(IsDigit));
                OnPropertyChanged(nameof(DigitalVisibility));
                OnPropertyChanged(nameof(PixelVisibility));
            }
        }

        public Visibility DigitalVisibility => IsDigit ? Visibility.Visible : Visibility.Collapsed;
        public Visibility PixelVisibility => !IsDigit ? Visibility.Visible : Visibility.Collapsed;
    }
}
