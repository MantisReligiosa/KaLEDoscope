using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows;

namespace BitmapProcessing
{
    public static class BitmapProcessor
    {
        private static readonly Dictionary<int, int> _spaces = new Dictionary<int, int>
        {
            {0, 1},
            {9, 2},
            {18, 4},
            {26, 6},
            {36, 8},
            {48, 10},
            {72, 16}
        };

        private static unsafe Bitmap FromMonochromeBitmap(bool[,] monochromeBitmap, byte red, byte green, byte blue)
        {
            var height = monochromeBitmap.GetLength(0);
            var width = monochromeBitmap.GetLength(1);
            var bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            BitmapData bd = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            try
            {
                byte* curpos;
                fixed (bool* monochromeBitmapPointer = monochromeBitmap)
                {
                    bool* monochromePointer = monochromeBitmapPointer;
                    for (int h = 0; h < height; h++)
                    {
                        curpos = ((byte*)bd.Scan0) + h * bd.Stride;
                        for (int w = 0; w < width; w++)
                        {
                            *curpos++ = (*monochromePointer) ? blue : (byte)0;
                            *curpos++ = (*monochromePointer) ? green : (byte)0;
                            *curpos++ = (*monochromePointer) ? red : (byte)0;
                            ++monochromePointer;
                        }
                    }
                }
            }
            finally
            {
                bitmap.UnlockBits(bd);
            }
            return bitmap;
        }

        private static unsafe void BitmapToByte(Bitmap bmp, out byte[,,] rgbBitmap, out byte[,] grayScaleBitmap, out bool[,] monochromeBitmap)
        {
            var width = bmp.Width;
            var height = bmp.Height;
            rgbBitmap = new byte[3, height, width];
            grayScaleBitmap = new byte[height, width];
            monochromeBitmap = new bool[height, width];
            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);
            try
            {
                byte* curpos;
                fixed (byte* rbgBitmapPointer = rgbBitmap)
                fixed (byte* grayscaleBitmapPointer = grayScaleBitmap)
                fixed (bool* monochromeBitmapPointer = monochromeBitmap)
                {
                    byte* redPointer = rbgBitmapPointer,
                          greenPointer = rbgBitmapPointer + width * height,
                          bluePointer = rbgBitmapPointer + 2 * width * height,
                          grayPointer = grayscaleBitmapPointer;
                    bool* monochromePointer = monochromeBitmapPointer;
                    for (int h = 0; h < height; h++)
                    {
                        curpos = ((byte*)bd.Scan0) + h * bd.Stride;
                        for (int w = 0; w < width; w++)
                        {
                            *bluePointer = *(curpos++);
                            *greenPointer = *(curpos++);
                            *redPointer = *(curpos++);
                            *grayPointer = (Byte)(0.3f * *redPointer + 0.59f * *greenPointer + 0.11f * *bluePointer);
                            *monochromePointer = *grayPointer > 128;
                            ++bluePointer;
                            ++greenPointer;
                            ++redPointer;
                            ++grayPointer;
                            ++monochromePointer;
                        }
                    }
                }
            }
            finally
            {
                bmp.UnlockBits(bd);
            }
        }

        private static unsafe bool[,] BitmapToMonochrome(Bitmap bmp)
        {
            BitmapToByte(bmp, out _, out _, out bool[,] monochrome);
            return monochrome;
        }

        public static Image DrawTextImage(string text, Font font, TextAlignment? alignment, Color textColor, Color backColor, Size zoneSize)
        {
            //first, create a dummy bitmap just to get a graphics object
            SizeF textSize;
            if (!zoneSize.IsEmpty)
            {
                textSize = zoneSize;
            }
            else
            {
                using (Image img = new Bitmap(1, 1))
                {
                    using (Graphics drawing = Graphics.FromImage(img))
                    {
                        //measure the string to see how big the image needs to be
                        textSize = drawing.MeasureString(text, font);
                        textSize.Width = textSize.Width > zoneSize.Width ? textSize.Width : zoneSize.Width;
                        textSize.Height = textSize.Height > zoneSize.Height ? textSize.Height : zoneSize.Height;
                    }
                }
            }
            StringAlignment stringAlignment = StringAlignment.Near;
            if (alignment.HasValue)
            {
                if (alignment == TextAlignment.Left)
                {
                    stringAlignment = StringAlignment.Near;
                }
                else if (alignment == TextAlignment.Center)
                { 
                    stringAlignment = StringAlignment.Center; 
                }
                else 
                { 
                    stringAlignment = StringAlignment.Far; 
                }
            }
            Image retImg = new Bitmap((int)textSize.Width, (int)textSize.Height);
            using (var drawing = Graphics.FromImage(retImg))
            {
                drawing.Clear(backColor);
                using (Brush textBrush = new SolidBrush(textColor))
                {
                    drawing.DrawString(text, font, textBrush, new RectangleF(0, 0, textSize.Width, textSize.Height), new StringFormat
                    {
                        LineAlignment = StringAlignment.Center,
                        Alignment = stringAlignment
                    });
                    drawing.Save();
                }
            }
            return retImg;
        }

        public static byte[] GenerateByteImageMono(Bitmap image)
        {
            var bits = BitmapToMonochrome(image);
            var bitList = new List<bool>();
            var rows = bits.GetLength(0);
            var columns = bits.GetLength(1);
            for (var column = 0; column < columns; column++)
            {
                for (var row = 0; row < rows; row++)
                {
                    var value = bits[row, column];
                    bitList.Add(value);
                }
            }

            var bitsInIncompleteByte = bitList.Count % 8;
            var needBytesForOctet = bitsInIncompleteByte == 0 ? 0 : 8 - bitsInIncompleteByte;
            for (var i = 0; i < needBytesForOctet; i++)
            {
                bitList.Add(false);
            }
            var bitArray = new BitArray(bitList.ToArray());
            var bytesTotal = bitList.Count / 8;
            var resultBytes = new byte[bytesTotal];
            bitArray.CopyTo(resultBytes, 0);
            return resultBytes;
        }

        public static Bitmap GetMonochromeBitmap(byte[] bytes, int height)
        {
            var bitArray = new BitArray(bytes);
            var width = bitArray.Count / height;
            var bits = new bool[height, width];
            var index = 0;
            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    bits[h, w] = bitArray[index++];
                }
            }
            return FromMonochromeBitmap(bits, 255, 0, 0);
        }

        public static byte[] InvertByteImageMono(byte[] bytes)
        {
            var bitArray = new BitArray(bytes);
            bitArray.Not();
            bitArray.CopyTo(bytes, 0);
            return bytes;
        }

        public static void RenderCharMono(char c, FontInfo fontInfo, out byte[] resultBytes, out ushort width, out byte height)
        {
            var style = System.Drawing.FontStyle.Regular;
            if (fontInfo.Italic && fontInfo.Bold)
            {
                style = System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Bold;
            }
            else if (fontInfo.Italic)
            {
                style = System.Drawing.FontStyle.Italic;
            }
            else if (fontInfo.Bold)
            {
                style = System.Drawing.FontStyle.Bold;
            }
            var font = new Font(fontInfo.Source, fontInfo.Height, style, GraphicsUnit.Pixel);
            var image = DrawTextImage(c.ToString(), font, null, Color.White, Color.Black, Size.Empty) as Bitmap;
            width = (ushort)image.Width;
            height = (byte)image.Height;
            var bitmapChar = BitmapToMonochrome(image);
            var rows = bitmapChar.GetLength(0);
            var columns = bitmapChar.GetLength(1);
            var firstNotEmptyColumn = 0;
            var isEmpty = true;
            while (isEmpty)
            {
                for (int row = 0; row < rows; row++)
                {
                    isEmpty &= !bitmapChar[row, firstNotEmptyColumn];
                }
                if (isEmpty)
                    firstNotEmptyColumn++;
            }

            var lastNotEmptyColunm = columns - 1;
            isEmpty = true;
            while (isEmpty)
            {
                for (int row = 0; row < rows; row++)
                {
                    isEmpty &= !bitmapChar[row, lastNotEmptyColunm];
                }
                if (isEmpty)
                    lastNotEmptyColunm--;
            }

            var spaceColumnsAmount = _spaces.LastOrDefault(kvp => kvp.Key <= fontInfo.Height).Value;
            var lastColumn = lastNotEmptyColunm + spaceColumnsAmount + 1;
            if (lastColumn > columns)
                lastColumn = columns;

            var bitList = new List<bool>();
            for (var column = firstNotEmptyColumn; column < lastColumn; column++)
            {
                for (var row = 0; row < rows; row++)
                {
                    var value = bitmapChar[row, column];
                    bitList.Add(value);
                }
            }
            width = (ushort)(lastColumn - firstNotEmptyColumn);
            var bitsInIncompleteByte = bitList.Count % 8;
            var needBytesForOctet = bitsInIncompleteByte == 0 ? 0 : 8 - bitsInIncompleteByte;
            for (var i = 0; i < needBytesForOctet; i++)
            {
                bitList.Add(false);
            }
            var bitArray = new BitArray(bitList.ToArray());
            var bytesTotal = bitList.Count / 8;
            resultBytes = new byte[bytesTotal];
            bitArray.CopyTo(resultBytes, 0);

        }
    }
}
