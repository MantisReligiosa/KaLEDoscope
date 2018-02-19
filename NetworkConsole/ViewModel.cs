using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Input = System.Windows.Input;

namespace NetworkConsole
{
    public class ViewModel : INotifyPropertyChanged
    {
        private int _port = 30000;
        private IProvider _provider;
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
                    GetProvider = ()=>new UdpProvider()
                },
                new ProviderItem
                {
                    Name="TCP",
                    GetProvider = ()=>new TcpProvider()
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
            logMessage("Запуск");
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
            var endpoint = new IPEndPoint(IPAddress.Any, _port);

            logMessage($"Начинаю слушать порт {_port} по {_providerItem.Name}");
            _provider.OnBytesRecieved += OnBytesRecieved;
            _provider.StartListen(endpoint);
        }

        private void OnBytesRecieved(object sender, BytesRecievedEventArgs e)
        {
            var receiveString = Encoding.UTF8.GetString(e.Bytes);
            logMessage($"Получено: {receiveString} от {e.SenderAddress} по {_providerItem.Name}");
        }

        private void Close()
        {
            logMessage($"Закрываю текущее подключение по {_providerItem.Name}");
            _provider.OnBytesRecieved -= OnBytesRecieved;
            _provider.Close();
        }

        private void logMessage(string Message)
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
                        Close();
                        string message = "Отправка ";
                        IPEndPoint _ipEndpoint;
                        if (_sendBroadcast)
                        {
                            _ipEndpoint = new IPEndPoint(IPAddress.Broadcast, _port);
                            message += $"широковещательного сообщения на порт {_port} по {_providerItem.Name}";
                        }
                        else
                        {
                            _ipEndpoint = new IPEndPoint(IPAddress.Parse(_address), _port);
                            message += $"сообщения на адрес {_address}:{_port} по {_providerItem.Name}";
                        }
                        message += $": {_message}";
                        logMessage(message);
                        _provider.Connect(_ipEndpoint);
                        var bytes = Encoding.UTF8.GetBytes(_message);
                        _provider.Send(bytes, bytes.Length);
                        Close();
                        StartListen();
                    });
                }
                return _sendMessage;
            }
        }
    }
}
