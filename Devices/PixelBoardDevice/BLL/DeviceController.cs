using BaseDevice;
using BitmapProcessing;
using PixelBoardDevice.DomainObjects;
using PixelBoardDevice.DomainObjects.Zones;
using PixelBoardDevice.DTO;
using PixelBoardDevice.Interfaces;
using SmartTechnologiesM.Base.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PixelBoardDevice.BLL
{
    public class DeviceController : IDeviceController
    {
        public DeviceController(Device device)
        {
            Device = device as PixelBoard;
        }

        public PixelBoard Device { get; }

        public void ValidateZones(ICollection<Zone> zones)
        {
            if (zones.IsNull())
                return;
            var incorrectZonesId = new List<int>();
            foreach (var zone in zones)
            {
                if (incorrectZonesId.Any(id => id == zone.Id))
                {
                    zone.IsValid = false;
                    continue;
                }
                zone.IsValid = true;
                foreach (var concurrentZone in zones.Except(new List<Zone> { zone }))
                {
                    if (zone.IntersectWith(concurrentZone))
                    {
                        zone.IsValid = false;
                        incorrectZonesId.Add(zone.Id);
                        incorrectZonesId.Add(concurrentZone.Id);
                    }
                }
            }
        }

        public Zone BindZone(Zone zone)
        {
            if (zone == null)
                return null;
            return Device?.Programs?.FirstOrDefault(s => s.Id == zone.ProgramId)?.Zones?.FirstOrDefault(z => z.Id == zone.Id);
        }

        public ReformatTextResult ReformatText(TextZone textZone, bool trimText)
        {
            var zoneFont = Device.Fonts.FirstOrDefault(f => f.Id == (textZone?.FontId ?? 0));
            if (zoneFont.IsNull())
                return null;
            if (string.IsNullOrEmpty(textZone.Text))
                return null;
            var formattingComplete = false;
            var text = textZone.Text.Replace(Constants.LineSplitString, "");
            ushort neededHeight = 0;
            var result = new ReformatTextResult();
            while (!formattingComplete)
            {
                formattingComplete = true;
                neededHeight = 0;
                var insertingPosition = 0;
                var style = FontStyle.Regular;
                if (zoneFont.Bold)
                {
                    if (zoneFont.Italic)
                        style = FontStyle.Bold | FontStyle.Italic;
                    else
                        style = FontStyle.Bold;
                }
                else if (zoneFont.Italic)
                    style = FontStyle.Italic;
                var font = new Font(zoneFont.Source, zoneFont.Height, style, GraphicsUnit.Pixel);
                foreach (var line in text.Split(new[] { "\r\n", Constants.LineSplitString }, StringSplitOptions.None))
                {
                    using (Image img = new Bitmap(1, 1))
                    {
                        using (Graphics drawing = Graphics.FromImage(img))
                        {
                            var textSize = drawing.MeasureString(line, font);
                            if (string.IsNullOrEmpty(line))
                            {
                                textSize = drawing.MeasureString(" ", font);
                            }
                            neededHeight += Convert.ToUInt16(textSize.Height);
                            if (neededHeight > Device.BoardSize.Height - textZone.Y)
                            {
                                if (trimText)
                                {
                                    var lastReturn = text.LastIndexOf("\r\n");
                                    if (lastReturn > 0)
                                    {
                                        text = text.Substring(0, lastReturn);
                                    }
                                }
                                else
                                {
                                    result.NeedUndo = true;
                                }
                            }
                            if (textSize.Width > textZone.Width)
                            {
                                if (neededHeight + Convert.ToUInt16(textSize.Height) >
                                    Device.BoardSize.Height - textZone.Y)
                                {
                                    //Некуда больше расширяться!
                                    if (trimText)
                                    {
                                        var lastReturn = text.LastIndexOf("\r\n");
                                        if (lastReturn > 0)
                                        {
                                            text = text.Substring(0, lastReturn);
                                            formattingComplete = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        result.NeedUndo = true;
                                    }
                                }
                                if (!result.NeedUndo && (textZone.Height < neededHeight + Convert.ToInt32(textSize.Height)))
                                    textZone.Height = (ushort)(neededHeight + Convert.ToUInt16(textSize.Height));
                                // Определяем где нужно разделить строку
                                var charAmount = 0;
                                float substLenght = 0;
                                while (substLenght < textZone.Width)
                                {
                                    charAmount++;
                                    substLenght = drawing.MeasureString(line.Substring(0, charAmount), font).Width;
                                }
                                insertingPosition += charAmount;
                                if (!string.IsNullOrEmpty(text))
                                {
                                    text = text.Insert(insertingPosition - 1, Constants.LineSplitString);
                                    formattingComplete = false;
                                }
                            }
                            else
                            {
                                insertingPosition += (line?.Length ?? 0) + 2;
                            }
                        }
                    }
                }
            }
            result.Text = text;
            result.NeedToResize = !result.NeedUndo && (neededHeight > textZone.Height && !string.IsNullOrEmpty(text));
            result.NeededHeight = neededHeight;
            return result;
        }

        public void RenderFont(BinaryFont binaryFont)
        {
            var zones = Device.Programs.SelectMany(p => p.Zones)
                .OfType<IFontableZone>().Where(z => (z.FontId == binaryFont.Id)).ToList();
            var chars = new List<char>();
            foreach (var zone in zones)
            {
                if (!string.IsNullOrEmpty(zone.Alphabet))
                {
                    foreach (var c in zone.Alphabet)
                    {
                        if (!chars.Contains(c))
                        {
                            chars.Add(c);
                        }
                    }
                }
                else if (zone is TextZone textZone)
                {
                    foreach (var c in textZone.Text?.Replace(" ", "")?.Replace("\r", "")?.Replace("\n", "")?.Replace("\0", "") ?? string.Empty)
                    {
                        if (!chars.Contains(c))
                        {
                            chars.Add(c);
                        }
                    }
                }
            }
            byte glyphHeight = 0;
            binaryFont.Alphabet = chars.Select(c =>
            {
                var fontInfo = new FontInfo
                {
                    Source = binaryFont.Source,
                    Italic = binaryFont.Italic,
                    Bold = binaryFont.Bold,
                    Height = binaryFont.Height
                };
                BitmapProcessor.RenderCharMono(c, fontInfo, out byte[] bytes, out ushort width, out glyphHeight);
                return new Glyph
                {
                    Symbol = c,
                    Image = bytes,
                    Width = width
                };
            }).ToArray();
            binaryFont.GlyphHeight = glyphHeight;
        }

        public void UpdateZoneFont(Zone zone, System.Windows.Media.FontFamily newFont, byte newFontSize, bool italic, bool bold, out byte newBinaryFontId)
        {
            newBinaryFontId = 0;
            var existBinaryFont = Device.Fonts.FirstOrDefault(bf => bf.Source == newFont.Source && bf.Italic == italic && bf.Bold == bold && bf.Height == newFontSize);
            if (!existBinaryFont.IsNull())
            {
                if (!Device.Programs.SelectMany(s => s.Zones.OfType<IFontableZone>()).Any())
                {
                    Device.Fonts.Remove(existBinaryFont);
                }
                else
                {
                    RenderFont(existBinaryFont);
                    return;
                }
            }
            var newBinaryFont = Device.Fonts
                .FirstOrDefault(bf => bf.Height == newFontSize && bf.Source == newFont.Source && bf.Italic == italic && bf.Bold == bold);
            if (newBinaryFont.IsNull())
            {
                newBinaryFontId = (byte)(Device.Fonts.Any() ? Device.Fonts.Max(f => f.Id) + 1 : 0);
                newBinaryFont = new BinaryFont
                {
                    Id = newBinaryFontId,
                    Source = newFont.Source,
                    Height = newFontSize,
                    Bold = bold,
                    Italic = italic,
                };
                Device.Fonts.Add(newBinaryFont);
            }
            (BindZone(zone) as IFontableZone).FontId = newBinaryFont.Id;
            RenderFont(newBinaryFont);
        }
    }
}
