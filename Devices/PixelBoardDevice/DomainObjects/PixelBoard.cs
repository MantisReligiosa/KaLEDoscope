using BaseDevice;
using PixelBoardDevice.DomainObjects.Zones;
using System.Collections.Generic;
using System.Linq;

namespace PixelBoardDevice.DomainObjects
{
    public class PixelBoard : Device
    {
        public List<BinaryFont> Fonts { get; set; }
        public BoardSize BoardSize { get; set; }
        public List<Program> Programs { get; set; }
        public List<BinaryImage> BinaryImages { get; set; }
        public BoardHardware Hardware { get; set; }

        internal void CleanupUnusedFonts()
        {
            var usedFontIds = Programs.SelectMany(p => p.Zones).OfType<IFontableZone>().Select(f => f.FontId).Distinct().ToList();
            var unusedFonts = Fonts.Where(f => !usedFontIds.Any(id => id == f.Id)).ToList();
            foreach (var unusedFont in unusedFonts)
            {
                Fonts.Remove(unusedFont);
            }
        }

        internal void CleanupUnusedImages()
        {
            var usedImagesIds = Programs.SelectMany(p => p.Zones).OfType<BitmapZone>().Select(f => f.BinaryImageId).Distinct().ToList();
            var unusedImages = BinaryImages.Where(f => !usedImagesIds.Any(id => id == f.Id)).ToList();
            foreach (var unusedImage in unusedImages)
            {
                BinaryImages.Remove(unusedImage);
            }
        }
    }
}
