using CommandProcessing;
using Extensions;
using PixelBoardDevice.DomainObjects;
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
            var zone = new Zone
            {
                Id = _bytes[5],
                ProgramId = _bytes[6],
                X = _bytes.ExtractUshort(7),
                Y = _bytes.ExtractUshort(9),
                Width = _bytes.ExtractUshort(11),
                Height = _bytes.ExtractUshort(13),
                Name = _bytes.ExtractString(16, nameLength),
                ZoneType = zoneType
            };
            switch (zoneType)
            {
                case 1:
                    var textLength = _bytes[18 + nameLength];
                    zone.FontId = _bytes[17 + nameLength];
                    zone.Text = _bytes.ExtractString(19 + nameLength, textLength);
                    break;
                case 2:
                    zone.FontId = _bytes[17 + nameLength];
                    break;
                case 3:
                    zone.BinaryImageId = _bytes[17 + nameLength];
                    break;
                case 4:
                    zone.FontId = _bytes[17 + nameLength];
                    var tagLength = _bytes[18 + nameLength];
                    zone.ExternalSourceTag = _bytes.ExtractString(19 + nameLength, tagLength);
                    break;
                case 5:
                    var state = _bytes[17 + nameLength];
                    zone.ClockType = state.GetBit(3) ? 2 : 1;//1 - текст, 2 - графические
                    if (zone.ClockType == 1)
                    {
                        zone.ClockFormat = (state.GetBit(4) ? 2 : 0) + (state.GetBit(5) ? 1 : 0);
                    }
                    zone.AllowPeriodicTimeSync = state.GetBit(6);
                    zone.AllowScheduledSync = state.GetBit(7);
                    if (zone.AllowScheduledSync)
                    {
                        zone.ScheduledTimeSync = new TimeSpan(_bytes[18 + nameLength],
                            _bytes[19 + nameLength], 0);
                    }
                    if (zone.AllowPeriodicTimeSync)
                    {
                        zone.PeriodicSyncInterval = _bytes.ExtractUshort(18 + nameLength);
                    }
                    break;
                case 6:
                    zone.TickerType = (_bytes[17 + nameLength] == 0) ? 1 : 2;
                    zone.TickerCountDownStartValue = new TimeSpan(0,
                        _bytes[18 + nameLength],
                        _bytes[19 + nameLength],
                        _bytes[20 + nameLength],
                        _bytes.ExtractUshort(21 + nameLength)
                        );
                    break;
            }

            return zone;
        }
    }
}
