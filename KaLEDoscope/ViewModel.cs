using CommandProcessing;
using DirectConnect;
using DomainData;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Input = System.Windows.Input;

namespace KaLEDoscope
{
    public class ViewModel
    {
        private ILogger _logger { get; set; }

        public ObservableCollection<ProtocolNode> ProtocolNodes { get; set; } = new ObservableCollection<ProtocolNode>();
        public ObservableCollection<LogItem> LogItems { get; set; } = new ObservableCollection<LogItem>();

        public object SelectedItem { get; set; }

        public ViewModel(ILogger logger)
        {
            _logger = logger;
            _logger.InfoRaised += (sender, message) => LogItems.Add(new LogItem { LogLevel = LogLevel.Info, Message = $"{sender}: {message}" });
            _logger.DebugRaised += (sender, message) => LogItems.Add(new LogItem { LogLevel = LogLevel.Debug, Message = $"{sender}: {message}" });
            _logger.WarnRaised += (sender, message) => LogItems.Add(new LogItem { LogLevel = LogLevel.Warn, Message = $"{sender}: {message}" });
            _logger.ErrorRaised += (sender, message) => LogItems.Add(new LogItem { LogLevel = LogLevel.Error, Message = $"{sender}: {message}" });
        }

        public void MakeNodes()
        {
            var directConnectDeviceScanner = new DeviceScanner<DirectConnection>(_logger);

            var devices = directConnectDeviceScanner.Search();

            var mqtt = new ProtocolNode
            {
                Name = "MQTT",
            };
            var directConnect = new ProtocolNode
            {
                Name = "DirectConnect",
            };

            foreach (var device in devices)
            {
                directConnect.Members.Add(new DeviceNode
                {
                    Device = device,
                });
            }

            ProtocolNodes.Add(mqtt);
            ProtocolNodes.Add(directConnect);
        }

        private DelegateCommand showDevicePlugin;
        public Input.ICommand ShowDevicePlugin
        {
            get
            {
                if (showDevicePlugin == null)
                {
                    showDevicePlugin = new DelegateCommand((o) =>
                    {
                        throw new NotImplementedException("Дописать тут");
                    });
                }
                return showDevicePlugin;
            }
        }
    }
}
