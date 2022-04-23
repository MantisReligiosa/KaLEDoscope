namespace PixelBoardDevice.DomainObjects.Zones
{
    public class BitmapZone : Zone
    {
        public override ZoneTypes ZoneType => ZoneTypes.Picture;
        public override string Name { get; set; } = "Изображение";
        public byte BinaryImageId { get; set; }
    }
}
