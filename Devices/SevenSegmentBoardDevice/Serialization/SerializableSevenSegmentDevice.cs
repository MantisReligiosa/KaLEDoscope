using BaseDeviceSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SevenSegmentBoardDevice.Serialization
{
    public class SerializableSevenSegmentDevice : SerializableBaseDevice
    {
        public SerializableSevenSegmentDevice()
            : base()
        { }

        public SerializableSevenSegmentDevice(SevenSegmentBoard device)
            : base(device)
        {
            AlarmSchedule = device.AlarmSchedule.Select(a => (SerializableAlarm)a).ToList();
            BoardType = (SerializableBoardType)device.BoardType;
            StopWatchParameters = (SerializableStopWatchParameters)device.StopWatchParameters;
            TimeSyncParameters = (SerializableTimeSyncParameters)device.TimeSyncParameters;
        }

        public List<SerializableAlarm> AlarmSchedule { get; set; }
        public SerializableBoardType BoardType { get; set; }
        public SerializableStopWatchParameters StopWatchParameters { get; set; }
        public SerializableTimeSyncParameters TimeSyncParameters { get; set; }

        public static explicit operator SerializableSevenSegmentDevice(SevenSegmentBoard device)
        {
            return new SerializableSevenSegmentDevice(device);
        }

        public static explicit operator SevenSegmentBoard(SerializableSevenSegmentDevice serializableDevice)
        {
            var sevenSegmentBoard = new SevenSegmentBoard();
            serializableDevice.FillBasicParameters(sevenSegmentBoard);
            sevenSegmentBoard.AlarmSchedule = serializableDevice.AlarmSchedule.Select(a => (Alarm)a).ToList();
            sevenSegmentBoard.BoardType = (BoardType)serializableDevice.BoardType;
            sevenSegmentBoard.StopWatchParameters = (StopWatchParameters)serializableDevice.StopWatchParameters;
            sevenSegmentBoard.TimeSyncParameters = (TimeSyncParameters)serializableDevice.TimeSyncParameters;
            return sevenSegmentBoard;
        }
    }
}
