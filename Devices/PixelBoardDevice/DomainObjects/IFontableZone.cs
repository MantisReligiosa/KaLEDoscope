namespace PixelBoardDevice.DomainObjects
{
    interface IFontableZone
    {
        //int ZoneType { get; }
        int? FontId { get; set; }
        bool UseWholeAlphabet { get; }
        string Alphabet { get; }
    }
}
