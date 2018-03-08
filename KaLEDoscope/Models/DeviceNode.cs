using BaseDevice;
using System.ComponentModel;

namespace KaLEDoscope
{
    public class DeviceNode : INotifyPropertyChanged
    {
        public Device Device { get; set; }

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

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
