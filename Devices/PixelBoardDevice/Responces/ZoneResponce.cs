using CommandProcessing;
using Extensions;
using PixelBoardDevice.DomainObjects;
using PixelBoardDevice.DomainObjects.Zones;
using System;

namespace PixelBoardDevice.Responces
{
    public class ZoneResponce : Responce<Zone>
    {
        public override byte ResponceID => 0x25;

        public override Zone Cast()
        {
            var nameLength = _bytes[15];
            var zoneType = _bytes[16 + nameLength];
            Zone zone = null;
            switch (zoneType)
            {
                case 1:
                    var textLength = _bytes[19 + nameLength];
                    var textZone = new TextZone()
                    {
                        FontId = _bytes[17 + nameLength],
                        Alignment = _bytes[18 + nameLength],
                        Text = _bytes.ExtractString(20 + nameLength, textLength)
                    };
                    zone = textZone;
                    break;
                case 2:
                    var sensorZone = new SensorZone
                    {
                        FontId = _bytes[17 + nameLength],
                        Alignment = _bytes[18 + nameLength],
                    };
                    zone = sensorZone;
                    break;
                case 3:
                    var bitmapZone = new BitmapZone
                    {
                        BinaryImageId = _bytes[17 + nameLength]
                    };
                    zone = bitmapZone;
                    break;
                case 4:
                    var tagLength = _bytes[19 + nameLength];
                    var tagZone = new TagZone
                    {
                        FontId = _bytes[17 + nameLength],
                        Alignment = _bytes[18 + nameLength],
                        ExternalSourceTag = _bytes.ExtractString(20 + nameLength, tagLength)
                    };
                    zone = tagZone;
                    break;
                case 5:
                    var clockZone = new ClockZone();
                    var state = _bytes[17 + nameLength];
                    clockZone.ClockType = state.GetBit(3) ? 2 : 1;//1 - текст, 2 - графические
                    if (clockZone.ClockType == 1)
                    {
                        clockZone.ClockFormat = (state.GetBit(4) ? 2 : 0) + (state.GetBit(5) ? 1 : 0);
                    }
                    clockZone.AllowPeriodicTimeSync = state.GetBit(6);
                    clockZone.AllowScheduledSync = state.GetBit(7);
                    if (clockZone.AllowScheduledSync)
                    {
                        clockZone.ScheduledTimeSync = new TimeSpan(_bytes[18 + nameLength],
                            _bytes[19 + nameLength], 0);
                    }
                    if (clockZone.AllowPeriodicTimeSync)
                    {
                        clockZone.PeriodicSyncInterval = _bytes.ExtractUshort(18 + nameLength);
                    }
                    zone = clockZone;
                    break;
                case 6:
                    var tickerZone = new TickerZone
                    {
                        TickerType = (_bytes[17 + nameLength] == 0) ? 1 : 2,
                        TickerCountDownStartValue = new TimeSpan(0,
                        _bytes[18 + nameLength],
                        _bytes[19 + nameLength],
                        _bytes[20 + nameLength],
                        _bytes.ExtractUshort(21 + nameLength)
                        )
                    };
                    zone = tickerZone;
                    break;
            }
            zone.Id = _bytes[5];
            zone.ProgramId = _bytes[6];
            zone.X = _bytes.ExtractUshort(7);
            zone.Y = _bytes.ExtractUshort(9);
            zone.Width = _bytes.ExtractUshort(11);
            zone.Height = _bytes.ExtractUshort(13);
            zone.Name = _bytes.ExtractString(16, nameLength);
            return zone;
        }
    }
}
