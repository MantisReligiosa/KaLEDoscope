using BaseDevice;
using System.ComponentModel;

namespace KaLEDoscope
{
    public class DeviceNode : INotifyPropertyChanged
    {
        public Device Device { get; set; }

        private bool _allowUpload;
        public bool AllowUpload
        {
            get
            {
                return _allowUpload;
            }
                set
            {
                _allowUpload = value;
                OnPropertyChanged(nameof(AllowUpload));
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
