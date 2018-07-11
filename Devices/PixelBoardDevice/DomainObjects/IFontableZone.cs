namespace PixelBoardDevice.DomainObjects
{
    interface IFontableZone
    {
        int? FontId { get; set; }
        bool UseWholeAlphabet { get; }
        string Alphabet { get; }
        int? Alignment { get; set; }
    }
}
