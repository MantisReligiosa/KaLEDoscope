using PixelBoardDevice.DomainObjects.Zones;
using PixelBoardDevice.POCO;

namespace PixelBoardDevice.BLL
{
    static class References
    {
        public static readonly ZoneType[] ZoneTypes = new ZoneType[]
        {
            new ZoneType
            {
                Id = 1,
                Name = "Текст",
                AllowAnimation = true,
                AllowBitmap = false,
                AllowText = true,
                AllowTextEditing = true,
                AllowClock = false,
                ZoneCondition = (z) => z is TextZone,
                Customize = () =>
                {
                    return new TextZone
                    {
                        IsValid = true,
                        Alignment = 0,
                        AnimationId = 0,
                        Height = 8
                    };
                }
            },
            new ZoneType
            {
                Id = 2,
                Name = "Датчик",
                AllowAnimation = false,
                AllowBitmap = false,
                AllowText = true,
                AllowMQTT = false,
                AllowTextEditing = false,
                AllowClock = false,
                ZoneCondition = (z) => z is SensorZone,
                Customize = () => new SensorZone{
                    IsValid = true,
                    Alignment = 0,
                    Height = 8
                }
            },
            new ZoneType
            {
                Id = 3,
                Name = "Тэг внешнего сервера",
                AllowAnimation = false,
                AllowBitmap = false,
                AllowText = true,
                AllowMQTT = true,
                AllowTextEditing = false,
                AllowClock = false,
                ZoneCondition = (z) => z is TagZone,
                Customize = () => new TagZone{
                    IsValid = true,
                    Alignment = 0,
                    Height = 8
                }
            },
            new ZoneType
            {
                Id = 4,
                Name = "Изображение",
                AllowAnimation = false,
                AllowBitmap = true,
                AllowText = false,
                AllowClock = false,
                ZoneCondition = (z) => z is BitmapZone,
                Customize = () => new BitmapZone{
                    IsValid = true,
                }
            },
            new ZoneType
            {
                Id = 5,
                Name = "Часы",
                AllowAnimation = false,
                AllowBitmap = false,
                AllowText = true,
                AllowTextEditing = false,
                AllowClock = true,
                ZoneCondition = (z) => z is ClockZone,
                Customize = () => new ClockZone{
                    IsValid = true,
                    ClockType = DomainObjects.Zones.ClockTypes.Analog
                },

            },
            new ZoneType
            {
                Id = 6,
                Name = "Таймер",
                AllowAnimation = false,
                AllowBitmap = false,
                AllowText = true,
                AllowTextEditing = false,
                AllowClock = false,
                AllowTicker = true,
                ZoneCondition = (z) => z is TickerZone,
                Customize = () => new TickerZone{
                    IsValid = true,
                }
            },
        };

        public static readonly AnimationType[] AnimationTypes = new AnimationType[]
        {
            new AnimationType
            {
                Id = 0,
                Name = "Нет",
                NoAnimationType = true
            },
            new AnimationType
            {
                Id = 1,
                Name = "Бегущая строка справа налево"
            },
            new AnimationType
            {
                Id = 2,
                Name = "Бегущая строка справа налево"
            }
        };

        public static readonly TickerType[] TickerTypes = new TickerType[]
        {
            new TickerType
            {
                Id = 1,
                Name = "Секундомер",
                AllowStartValue = false
            },
            new TickerType
            {
                Id = 2,
                Name = "Обратный отсчёт",
                AllowStartValue = true
            }
        };

        public static readonly ClockType[] ClockTypes = new ClockType[]
        {
            new ClockType
            {
                Type = DomainObjects.Zones.ClockTypes.Digital,
                Name = "Текстовый",
                AllowFormat = true
            },
            new ClockType
            {
                Type = DomainObjects.Zones.ClockTypes.Digital,
                Name = "Графический",
                AllowFormat = false
            }
        };

        public static readonly ClockFormat[] ClockFormats = new ClockFormat[]
        {
            new ClockFormat
            {
                Id = 1,
                Name = "ЧЧ(24):ММ",
                Sample="13:45"
            },
            new ClockFormat
            {
                Id = 2,
                Name = "ЧЧ(24):ММ:CC",
                Sample = "13:45:30"
            },
            new ClockFormat
            {
                Id = 3,
                Name = "ЧЧ(12):ММ",
                Sample="1:45"
            },
            new ClockFormat
            {
                Id = 4,
                Name = "ЧЧ(12):ММ:CC",
                Sample="1:45:30"
            }
        };

        public static readonly BoardHardwareType[] BoardHardwareTypes = new BoardHardwareType[]
        {
            new BoardHardwareType
            {
                Id = (int)DomainObjects.BoardHardwareType.Hub12,
                Name = "HUB12"
            },
            new BoardHardwareType
            {
                Id = (int)DomainObjects.BoardHardwareType.Hub08,
                Name = "HUB08"
            },
            new BoardHardwareType
            {
                Id = (int)DomainObjects.BoardHardwareType.RsPanel16x16,
                Name = "RS panel 16x16"
            },
            new BoardHardwareType
            {
                Id = (int)DomainObjects.BoardHardwareType.RsPanel8x16,
                Name = "RS panel 8x16"
            },
            new BoardHardwareType
            {
                Id = (int)DomainObjects.BoardHardwareType.RsPanel8x8,
                Name = "RS panel 8x8"
            },
            new BoardHardwareType
            {
                Id = (int)DomainObjects.BoardHardwareType.RsPanel8x4,
                Name = "RS panel 8x4"
            },
            new BoardHardwareType
            {
                Id = (int)DomainObjects.BoardHardwareType.RsPanel12x12,
                Name = "RS panel 12x12"
            },
            new BoardHardwareType
            {
                Id = (int)DomainObjects.BoardHardwareType.RsPanel9x28,
                Name = "RS panel 9x28"
            },
            new BoardHardwareType
            {
                Id = (int)DomainObjects.BoardHardwareType.Rs7Segment,
                Name = "RS 7 segment"
            },
        };

        public static readonly int[] FontSizes = new int[]
        {
            8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72
        };
    }
}
