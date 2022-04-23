using SmartTechnologiesM.Base;
using System;

namespace SevenSegmentBoardDevice
{
    public class DisplayFrame : Notified
    {
        public int Id { get; set; }
        public string Name { get; set; }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                if (_isEnabled == value)
                {
                    return;
                }
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                if (_isChecked == value)
                {
                    return;
                }
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        public int CharLenght { get; set; }

        public int DisplayPeriod { get; set; }

        public Func<int, string> Preview { get; set; }
    }
}
