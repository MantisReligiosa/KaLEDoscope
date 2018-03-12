using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandProcessing
{
    public class Invoker
    {
        private readonly ILogger _logger;
        public Invoker(ILogger logger)
        {
            _logger = logger;
        }

        public void Invoke(ICommand command)
        {
            _logger.Info(this, $"{command.Name}" + ((command.Device != null) ? $" устройства {command.Device.Name}" : String.Empty));
            try
            {
                command.Execute();
            }
            catch(Exception exception)
            {
                if (command.Device != null)
                {
                    _logger.Error(this, $"Ошибка отправки команды \"{command.Name}\" устройству \"{command.Device.Name}\"", exception);
                }
                else
                {
                    _logger.Error(this, $"Ошибка выполнения команды \"{command.Name}\"", exception);
                }
            }
            finally
            {
                command.Finally();
            }
        }
    }
}
