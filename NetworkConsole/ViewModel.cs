using SmartTechnologiesM.Base.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using Input = System.Windows.Input;

namespace NetworkConsole
{
    public class ViewModel : INotifyPropertyChanged
    {
        private int _port = 30000;
        private IProvider _provider;
        private string _providerName;
        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
                OnPropertyChanged(nameof(Port));
            }
        }

        private bool _sendBroadcast = true;
        public bool SendBroadcast
        {
            get
            {
                return _sendBroadcast;
            }
            set
            {
                _sendBroadcast = value;
                OnPropertyChanged(nameof(SendBroadcast));
            }
        }

        private bool _sendToAddress = false;
        public bool SendToAddress
        {
            get
            {
                return _sendToAddress;
            }
            set
            {
                _sendToAddress = value;
                OnPropertyChanged(nameof(SendToAddress));
            }
        }

        private string _address = "192.168.0.71";
        public string Address
        {
            get
            {
                return _address;
            }
            set
            {
                _address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        private string _message;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        private StringBuilder _log = new StringBuilder();
        public string Log
        {
            get
            {
                return _log.ToString();
            }
            set
            {
                _log = new StringBuilder(value);
                OnPropertyChanged(nameof(Log));
            }
        }

        public ObservableCollection<ProviderItem> ProviderItems { get; set; }
            = new ObservableCollection<ProviderItem>(new List<ProviderItem>
        {
                new ProviderItem
                {
                    Name = "UDP",
                    GetProvider = ()=>new UdpProvider(),
                    AllowBroadCast = true
                },
                new ProviderItem
                {
                    Name="TCP",
                    GetProvider = ()=>new TcpProvider(),
                    AllowBroadCast= false
                }
            });

        private ProviderItem _providerItem;
        public ProviderItem ProviderItem
        {
            get
            {
                return _providerItem;
            }
            set
            {
                _providerItem = value;
                OnPropertyChanged(nameof(ProviderItem));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModel()
        {
            LogMessage("Запуск");
            ProviderItem = ProviderItems.First();
            StartListen();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void StartListen()
        {
            _provider = _providerItem.GetProvider();
            _providerName = _providerItem.Name;
            var endpoint = new IPEndPoint(IPAddress.Any, _port);

            LogMessage($"Начинаю слушать порт {_port} по {_providerName}");
            _provider.OnBytesRecieved += OnBytesRecieved;
            _provider.StartListen(endpoint);
        }

        private void OnBytesRecieved(object sender, BytesRecievedEventArgs e)
        {
            var receiveString = e.Bytes.ToStringExtend();
            LogMessage($"Получено: {receiveString} от {e.SenderAddress} по {_providerName}");
        }

        private void Close()
        {
            LogMessage($"Закрываю текущее подключение по {_providerName}");
            _provider.OnBytesRecieved -= OnBytesRecieved;
            _provider.Close();
        }

        private void LogMessage(string Message)
        {
            _log.AppendLine(Message);
            OnPropertyChanged(nameof(Log));
        }

        private DelegateCommand _changePort;
        public Input.ICommand ChangePort
        {
            get
            {
                if (_changePort == null)
                {
                    _changePort = new DelegateCommand((o) =>
                    {
                        Close();
                        StartListen();
                    });
                }
                return _changePort;
            }
        }

        private DelegateCommand _sendMessage;
        public Input.ICommand SendMessage
        {
            get
            {
                if (_sendMessage == null)
                {
                    _sendMessage = new DelegateCommand((o) =>
                    {
                        Send(_sendBroadcast, _address, _port, _message);
                    });
                }
                return _sendMessage;
            }
        }

        private void Send(bool isBroadcast, string address, int port, string msg)
        {
            if (string.IsNullOrEmpty(msg))
                return;
            if (msg.Length % 2 == 1)
            {
                msg = "0" + msg;
            }
            Close();
            string message = "Отправка ";
            IPEndPoint ipEndpoint;
            if (isBroadcast)
            {
                ipEndpoint = new IPEndPoint(IPAddress.Broadcast, port);
                message += $"широковещательного сообщения на порт {port} по {_providerItem.Name}";
            }
            else
            {
                ipEndpoint = new IPEndPoint(IPAddress.Parse(_address), port);
                message += $"сообщения на адрес {address}:{port} по {_providerItem.Name}";
            }
            var bytes = msg.GetBytesFromHexString();
            message += $": {bytes.ToStringExtend()}";
            LogMessage(message);
            _provider.Connect(ipEndpoint);


            _provider.Send(bytes, bytes.Length);
            Close();
            StartListen();
        }
    }
}
