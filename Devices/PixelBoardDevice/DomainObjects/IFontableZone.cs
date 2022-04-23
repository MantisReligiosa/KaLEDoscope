using System.Windows;

namespace PixelBoardDevice.DomainObjects
{
    interface IFontableZone
    {
        byte? FontId { get; set; }
        string Alphabet { get; }
        TextAlignment? Alignment { get; set; }
    }
}
