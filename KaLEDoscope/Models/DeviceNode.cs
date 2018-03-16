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

        private bool _allowDownload;
        public bool AllowDownload
        {
            get
            {
                return _allowDownload;
            }
            set
            {
                _allowDownload = value;
                OnPropertyChanged(nameof(AllowDownload));
            }
        }

        private bool _allowSave;
        public bool AllowSave
        {
            get
            {
                return _allowSave;
            }
            set
            {
                _allowSave = value;
                OnPropertyChanged(nameof(AllowSave));
            }
        }

        private bool _allowLoad;
        public bool AllowLoad
        {
            get
            {
                return _allowLoad;
            }
            set
            {
                _allowLoad = value;
                OnPropertyChanged(nameof(AllowLoad));
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
