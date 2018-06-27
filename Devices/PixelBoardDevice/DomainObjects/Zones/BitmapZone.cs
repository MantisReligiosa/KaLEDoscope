namespace PixelBoardDevice.DomainObjects.Zones
{
    public class BitmapZone : Zone
    {
        public override int ZoneType => 3;
        public override string Name { get; set; } = "Изображение";
        public int BinaryImageId { get; set; }
    }
}
