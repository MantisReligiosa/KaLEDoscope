using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using Font = System.Windows.Media;

namespace BitmapProcessing
{
    public static class BitmapProcessor
    {
        public static string GenerateBase64FontMono(string text, Font.FontFamily newFont, bool itallic, bool bold, int newFontSize)
        {
            var bitmapChars = new List<bool[,]>();
            foreach (var c in text)
            {
                var style = System.Drawing.FontStyle.Regular;
                if (itallic && bold)
                {
                    style = System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Bold;
                }
                else if (itallic)
                {
                    style = System.Drawing.FontStyle.Italic;
                }
                else if (bold)
                {
                    style = System.Drawing.FontStyle.Bold;
                }
                var font = new System.Drawing.Font(newFont.Source, (float)newFontSize, style, GraphicsUnit.Pixel);
                var image = DrawTextImage(c.ToString(), font, Color.White, Color.Black, Size.Empty) as Bitmap;
                var trimmedToHeightImage = image.Clone(new Rectangle(0, image.Height - newFontSize, image.Width, newFontSize),
                    image.PixelFormat);
                var bitmap = BitmapToMonochrome(trimmedToHeightImage);
                var trimmedbitmap = TrimSpaces(bitmap);
                bitmapChars.Add(trimmedbitmap);
            }
            var separatorLength = newFontSize * 2;
            var separator = new bool[separatorLength];
            var indicator = 1;
            for (var i = 0; i < separatorLength; i++)
            {
                separator[i] = (indicator > 0);
                if (indicator == 2)
                {
                    indicator = 0;
                }
                else
                {
                    indicator++;
                }
            }
            var bitList = new List<bool>();
            foreach (var bitmapChar in bitmapChars)
            {
                var rows = bitmapChar.GetLength(0);
                var columns = bitmapChar.GetLength(1);
                for (var column = 0; column < columns; column++)
                {
                    for (var row = 0; row < rows; row++)
                    {
                        var value = bitmapChar[row, column];
                        bitList.Add(value);
                    }
                }
                bitList.AddRange(separator);
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
            var base64String = Convert.ToBase64String(resultBytes);
            return base64String;
        }

        private static bool[,] TrimSpaces(bool[,] bitmap)
        {
            var rows = bitmap.GetLength(0);
            var columns = bitmap.GetLength(1);
            int column;
            for (column = 0; column < columns; column++)
            {
                var hasValue = false;
                for (int row = 0; row < rows; row++)
                {
                    hasValue |= bitmap[row, column];
                }
                if (hasValue)
                {
                    break;
                }
            }
            var firstNotEmptyColumn = (column == columns) ? 0 : column;
            for (column = columns - 1; column > 0; column--)
            {
                var hasValue = false;
                for (int row = 0; row < rows; row++)
                {
                    hasValue |= bitmap[row, column];
                }
                if (hasValue)
                {
                    break;
                }
            }
            var lastNotEmptyColumn = (column == 0) ? (columns - 1) : column;
            var notEmptyColumnsAmount = lastNotEmptyColumn - firstNotEmptyColumn + 1;
            var trimmedBitmap = new bool[rows, notEmptyColumnsAmount];
            for (int row = 0; row < rows; row++)
            {
                int destColumn = 0;
                for (var sourceColumn = firstNotEmptyColumn; sourceColumn <= lastNotEmptyColumn; sourceColumn++)
                {
                    trimmedBitmap[row, destColumn] = bitmap[row, sourceColumn];
                    destColumn++;
                }
            }
            return trimmedBitmap;
        }

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
                            *grayPointer = (Byte)(0.3f * (float)(*redPointer) + 0.59f * (float)(*greenPointer) + 0.11f * (float)(*bluePointer));
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

        private unsafe static bool[,] BitmapToMonochrome(Bitmap bmp)
        {
            BitmapToByte(bmp, out byte[,,] rgb, out byte[,] grayscale, out bool[,] monochrome);
            return monochrome;
        }

        public static Image DrawTextImage(String text, System.Drawing.Font font, Color textColor, Color backColor, Size minSize)
        {
            //first, create a dummy bitmap just to get a graphics object
            SizeF textSize;
            using (Image img = new Bitmap(1, 1))
            {
                using (Graphics drawing = Graphics.FromImage(img))
                {
                    //measure the string to see how big the image needs to be
                    textSize = drawing.MeasureString(text, font);
                    if (!minSize.IsEmpty)
                    {
                        textSize.Width = textSize.Width > minSize.Width ? textSize.Width : minSize.Width;
                        textSize.Height = textSize.Height > minSize.Height ? textSize.Height : minSize.Height;
                    }
                }
            }
            Image retImg = new Bitmap((int)textSize.Width, (int)textSize.Height);
            using (var drawing = Graphics.FromImage(retImg))
            {
                drawing.Clear(backColor);
                using (Brush textBrush = new SolidBrush(textColor))
                {
                    drawing.DrawString(text, font, textBrush, 0, 0);
                    drawing.Save();
                }
            }
            return retImg;
        }

        public static string GenerateBase64ImageMono(Bitmap image)
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
            var base64String = Convert.ToBase64String(resultBytes);
            return base64String;
        }

        public static Bitmap GetMonochromeBitmap(string base64String,int height, Color color)
        {
            var bytes = Convert.FromBase64String(base64String);
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
            return FromMonochromeBitmap(bits,255,0,0);
        }

        public static string InvertBase64String(string base64String)
        {
            var bytes = Convert.FromBase64String(base64String);
            var bitArray = new BitArray(bytes);
            bitArray.Not();
            bitArray.CopyTo(bytes, 0);
            var invertedString = Convert.ToBase64String(bytes);
            return invertedString;
        }
    }
}
