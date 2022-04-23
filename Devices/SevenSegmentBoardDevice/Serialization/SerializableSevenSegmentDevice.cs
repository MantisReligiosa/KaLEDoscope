using BaseDeviceSerialization;
using System.Collections.Generic;
using System.Linq;

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
            DisplayFrames = device.DisplayFrames.Select(f => (SerializableDisplayFrame)f).ToList();
            BoardType = (SerializableBoardType)device.BoardType;
            StopWatchParameters = (SerializableStopWatchParameters)device.StopWatchParameters;
            TimeSyncParameters = (SerializableTimeSyncParameters)device.TimeSyncParameters;
        }

        public List<SerializableAlarm> AlarmSchedule { get; set; }
        public List<SerializableDisplayFrame> DisplayFrames { get; set; }
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
            sevenSegmentBoard.DisplayFrames = serializableDevice.DisplayFrames?.Select(f => (DisplayFrame)f).ToList() ?? new List<DisplayFrame>();
            sevenSegmentBoard.BoardType = (BoardType)serializableDevice.BoardType;
            sevenSegmentBoard.StopWatchParameters = (StopWatchParameters)serializableDevice.StopWatchParameters;
            sevenSegmentBoard.TimeSyncParameters = (TimeSyncParameters)serializableDevice.TimeSyncParameters;
            return sevenSegmentBoard;
        }
    }
}
