namespace PixelBoardDevice.DomainObjects
{
    public class Picture : Zone
    {
        public string Base64Bitmap { get; set; }

        public override string Name => "Изображение";
    }
}
