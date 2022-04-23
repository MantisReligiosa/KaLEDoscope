using System;
using System.Collections.Immutable;

namespace SevenSegmentBoardDevice
{
    public static class Refs
    {
        public static readonly ImmutableList<DisplayType> DisplayTypes = ImmutableList.Create
        (
            new DisplayType
            {
                Id = 0,
                Name = "Цифровая",
                IsFontEnabled = false
            },
            new DisplayType
            {
                Id = 1,
                Name = "Пиксельная",
                IsFontEnabled = true
            }
        );

        public static readonly ImmutableList<FontType> FontTypes = ImmutableList.Create
        (
            new FontType
            {
                Id = 1,
                Name = "0-Arial8Narrow"
            },
            new FontType
            {
                Id = 2,
                Name = "1-Другой"
            }
        );

        public static readonly ImmutableList<DisplayFormat> DisplayFormats = ImmutableList.Create
        (
            new DisplayFormat
            {
                Id = 1,
                Name = "000 (3 сегмента)",
                Capacity = 3
            },
            new DisplayFormat
            {
                Id = 2,
                Name = "00:00 (4 сегмента)",
                Capacity = 4
            },
            new DisplayFormat
            {
                Id = 3,
                Name = "00:00:00 (6 сегментов)",
                Capacity = 6
            },
            new DisplayFormat
            {
                Id = 4,
                Name = "00:00:00.000 (9 сегментов)",
                Capacity = 9
            }
        );

        public static readonly ImmutableList<CountdownType> CountdownTypes = ImmutableList.Create
        (
            new CountdownType
            {
                Id = 1,
                Name = "Секундомер"
            },
            new CountdownType
            {
                Id = 2,
                Name = "Обратный отсчет"
            }
        );

        public static readonly ImmutableList<DisplayFrame> DisplayFrames = ImmutableList.Create
        (
            new DisplayFrame
            {
                Id = 1,
                Name = "Текущее время",
                DisplayPeriod = 45,
                IsEnabled = true,
                IsChecked = true,
                CharLenght = 4,
                Preview = (capacity) =>
                {
                    switch (capacity)
                    {
                        case 6:
                            return DateTime.Now.ToString("HH:mm:ss");
                        case 9:
                            return DateTime.Now.ToString("HH:mm:ss.fff");
                        default:
                            return DateTime.Now.ToString("HH:mm");
                    }
                }
            },
            new DisplayFrame
            {
                Id = 2,
                Name = "Дата дд/мм/гг",
                DisplayPeriod = 35,
                IsEnabled = false,
                IsChecked = false,
                CharLenght = 6,
                Preview = (i) => DateTime.Now.ToString("dd.MM.yy")
            },
            new DisplayFrame
            {
                Id = 3,
                Name = "Дата дд/мм",
                DisplayPeriod = 35,
                IsEnabled = true,
                IsChecked = false,
                CharLenght = 4,
                Preview = (i) => DateTime.Now.ToString("dd.MM")
            },
            new DisplayFrame
            {
                Id = 4,
                Name = "гггг",
                DisplayPeriod = 20,
                IsEnabled = true,
                IsChecked = false,
                CharLenght = 4,
                Preview = (i) => DateTime.Now.ToString("yyyy")
            },
            new DisplayFrame
            {
                Id = 5,
                Name = "Температура",
                DisplayPeriod = 25,
                IsEnabled = true,
                IsChecked = false,
                CharLenght = 2,
                Preview = (i) => "-4"
            },
            new DisplayFrame
            {
                Id = 6,
                Name = "Влажность",
                DisplayPeriod = 25,
                IsEnabled = true,
                IsChecked = false,
                CharLenght = 3,
                Preview = (i) => "75"
            },
            new DisplayFrame
            {
                Id = 7,
                Name = "Давление",
                DisplayPeriod = 5,
                IsEnabled = true,
                IsChecked = false,
                CharLenght = 3,
                Preview = (i) => "760"
            },
            new DisplayFrame
            {
                Id = 8,
                Name = "Уровень радиации",
                DisplayPeriod = 5,
                IsEnabled = true,
                IsChecked = false,
                CharLenght = 4,
                Preview = (i) => "00000"
            },
            new DisplayFrame
            {
                Id = 9,
                Name = "Датчик XXXXX",
                DisplayPeriod = 5,
                IsEnabled = false,
                IsChecked = false,
                CharLenght = 5,
                Preview = (i) => "12345"
            }
        );

        public static readonly ImmutableList<SyncSource> SyncSources = ImmutableList.Create
        (
            new SyncSource
            {
                Id = 1,
                IsCutomized = false,
                Name = "Встроенный источник точного времени",
                AllowTimezones = false,
            },
            new SyncSource
            {
                Id = 2,
                IsCutomized = true,
                AllowTimezones = true,
                Name = "NTP сервер"
            },
            new SyncSource
            {
                Id = 3,
                IsCutomized = true,
                AllowTimezones = false,
                Name = "Альтернативный источник"
            }
        );
    }
}
