using System.Collections.Generic;
using BaseDevice;

namespace PixelBoardDevice
{
    public class PixelBoard : Device
    {
        public string Alphabet { get; set; } =
            "0123456789" +
            "abcdefghijklmnopqrstuvwxyz" +
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
            "абвгдеёжзийклмнопрстуфхцчшщъыъэюя" +
            "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЮЬЭЮЯ" +
            "`~!@#$%^&*()-_+=*:;\"',.<>/\\|";
        public List<BinaryFont> Fonts { get; set; }
        public BoardSize BoardSize { get; set; }
        public List<Screen> Screens { get; set; }
    }
}