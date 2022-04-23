using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SevenSegmentBoardDevice.Serialization
{
    public class SerializableDisplayFrame
    {
        public int CharLenght { get; set; }
        public int DisplayPeriod { get; set; }
        public int Id { get; set; }
        public bool IsChecked { get; set; }
        public bool IsEnabled { get; set; }
        public string Name { get; set; }

        public static explicit operator SerializableDisplayFrame(DisplayFrame frame)
        {
            return new SerializableDisplayFrame
            {
                CharLenght = frame.CharLenght,
                DisplayPeriod = frame.DisplayPeriod,
                Id = frame.Id,
                IsChecked = frame.IsChecked,
                IsEnabled = frame.IsEnabled,
                Name = frame.Name
            };
        }

        public static explicit operator DisplayFrame(SerializableDisplayFrame frame)
        {
            return new DisplayFrame
            {
                CharLenght = frame.CharLenght,
                DisplayPeriod = frame.DisplayPeriod,
                Id = frame.Id,
                IsChecked = frame.IsChecked,
                IsEnabled = frame.IsEnabled,
                Name = frame.Name
            };
        }
    }
}
