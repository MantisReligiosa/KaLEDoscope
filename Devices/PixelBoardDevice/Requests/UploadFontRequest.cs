using CommandProcessing;
using PixelBoardDevice.DomainObjects;
using SmartTechnologiesM.Base.Extensions;
using System.Collections.Generic;

namespace PixelBoardDevice.Requests
{
    public class UploadFontRequest : Request
    {
        public override byte RequestID => 0x26;

        public override byte[] MakeData(object o)
        {
            var binaryFont = (BinaryFont)o;
            var bitmaps = new List<byte>();
            var bytes = new List<byte>()
            {
                0x01,
                binaryFont.Id,
                binaryFont.GlyphHeight,
                (byte)((binaryFont.Italic ? 2 : 0) + (binaryFont.Bold ? 1 : 0)),
                (byte)binaryFont.Source.Length
            };
            bytes.AddRange(binaryFont.Source.ToBytes());
            bytes.AddRange(((ushort)(binaryFont.Alphabet?.Length ?? 0)).ToBytes());
            ushort offset = 0;
            foreach(var glyph in binaryFont.Alphabet ?? new Glyph[0])
            {
                bytes.Add(glyph.Symbol.ToByte());
                bytes.AddRange(offset.ToBytes());
                bytes.AddRange(glyph.Width.ToBytes());
                bitmaps.AddRange(glyph.Image);
                offset += (ushort)glyph.Image.Length;
            }
            bytes.AddRange(bitmaps);
            return bytes.ToArray();
        }
    }
}
