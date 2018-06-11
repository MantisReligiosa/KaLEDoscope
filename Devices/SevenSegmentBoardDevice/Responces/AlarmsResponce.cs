using CommandProcessing;
using System.Collections.Generic;
using System.Linq;

namespace SevenSegmentBoardDevice.Responces
{
    public class AlarmsResponce : Responce<List<Alarm>>
    {
        public override byte ResponceID => 0x11;

        public override List<Alarm> Cast()
        {
            return GetList().ToList();
        }

        public IEnumerable<Alarm> GetList()
        {
            for (int i = 0; i < _bytes[5]; i++)
            {
                yield return new Alarm
                {
                    IsActive = (_bytes[6 + i * 5] != 0),
                    StartTimeSpan = new System.TimeSpan(_bytes[7 + i * 5], _bytes[8 + i * 5], 0),
                    Period = new System.TimeSpan(_bytes[9 + i * 5], _bytes[10 + i * 5], 0)
                };
            }
        }
    }
}
