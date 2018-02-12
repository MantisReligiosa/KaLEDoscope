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
        private UdpClient _udpClient;
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

        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModel()
        {
            logMessage("Запуск");
            StartListen();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void StartListen()
        {
            var _endpoint = new IPEndPoint(IPAddress.Any, _port);
            _udpClient = new UdpClient(_endpoint);

            var s = new UdpState();
            s.e = _endpoint;
            s.u = _udpClient;

            logMessage($"Начинаю слушать порт {_port}");
            _udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), s);
        }

        private void Close()
        {
            logMessage("Закрываю текущее подключение");
            _udpClient?.Close();
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                UdpClient u = ((UdpState)(ar.AsyncState)).u;
                IPEndPoint e = ((UdpState)(ar.AsyncState)).e;

                if (u == null)
                {
                    return;
                }
                var receiveBytes = u.EndReceive(ar, ref e);
                var receiveString = Encoding.UTF8.GetString(receiveBytes);
                logMessage($"Получено: {receiveString} от {e.Address}");

                UdpState s = new UdpState();
                s.e = e;
                s.u = u;
                u.BeginReceive(new AsyncCallback(ReceiveCallback), s);
            }
            catch (ObjectDisposedException) { }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения данных", ex);
            }
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
                            message += $"широковещательного сообщения на порт {_port}";
                        }
                        else
                        {
                            _ipEndpoint = new IPEndPoint(IPAddress.Parse(_address), _port);
                            message += $"сообщения на адрес {_address}:{_port}";
                        }
                        message += $": {_message}";
                        logMessage(message);
                        _udpClient = new UdpClient();
                        _udpClient.Connect(_ipEndpoint);
                        var bytes = Encoding.UTF8.GetBytes(_message);
                        _udpClient.Send(bytes, bytes.Length);
                        Close();
                        StartListen();
                    });
                }
                return _sendMessage;
            }
        }
    }
}
