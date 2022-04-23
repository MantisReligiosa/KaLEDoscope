using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Input = System.Windows.Input;

namespace BitmapConsole
{
    internal class ViewModel : INotifyPropertyChanged
    {
        public ViewModel()
        {
            Text = "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";
            BitmapHeight = 16;
        }

        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        private int _bitmapHeight;
        public int BitmapHeight
        {
            get
            {
                return _bitmapHeight;
            }
            set
            {
                _bitmapHeight = value;
                OnPropertyChanged(nameof(BitmapHeight));
            }
        }

        private object _image;
        public object Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                OnPropertyChanged(nameof(Image));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private DelegateCommand _decodeBase64;
        public Input.ICommand DecodeBase64
        {
            get
            {
                if (_decodeBase64 == null)
                {
                    _decodeBase64 = new DelegateCommand((o) =>
                    {
                        var bytes = Convert.FromBase64String(Text);
                        var bits = new BitArray(bytes);

                        PixelFormat pf = PixelFormats.Bgr32;
                        int width = bits.Length / BitmapHeight;
                        int height = BitmapHeight;
                        int rawStride = (width * pf.BitsPerPixel + 7) / 8;
                        byte[] rawImage = new byte[rawStride * height];

                        var x = 0;
                        var y = 0;
                        for (int i = 0; i < bits.Length + 1; i++)
                        {
                            var bit = bits[i];
                            var rawIndex = (width * y + x) * 4;
                            rawImage[rawIndex + 2] = (byte)(bit ? 255 : 0);
                            y++;
                            if (y >= BitmapHeight)
                            {
                                y = 0;
                                x++;
                            }
                            if (x >= width)
                            {
                                break;
                            }
                        }
                        // Create a BitmapSource.
                        BitmapSource bitmap = BitmapSource.Create(width, height,
                            96, 96, pf, null,
                            rawImage, rawStride);
                        Image = bitmap;
                    });
                }
                return _decodeBase64;
            }
        }
    }
}