using Abstractions;
using Extensions;
using PixelBoardDevice.DomainObjects;
using PixelBoardDevice.DomainObjects.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UiCommands;
using Input = System.Windows.Input;

namespace PixelBoardDevice.UI
{
    public class ProgramPreviewViewModel : Notified
    {
        private readonly PixelDeviceViewModel _model;
        private MouseState _currentMouseState;
        private MouseState _suggestedMouseState;

        public Canvas PreviewContent { get; set; }

        public int ViewHeight { get; set; }
        public int ViewWidht { get; set; }
        public Cursor Cursor { get; set; }

        public ProgramPreviewViewModel(PixelDeviceViewModel pixelDeviceViewModel)
        {
            PreviewContent = new Canvas();
            _model = pixelDeviceViewModel;
            _model.PropertyChanged += (s, e) => Redraw();
            Redraw();
            _currentMouseState = _suggestedMouseState = MouseState.Movement;
        }

        private void Redraw()
        {
            ViewHeight = Convert.ToInt32(_model.DeviceHeight * _model.PreviewScale);
            ViewWidht = Convert.ToInt32(_model.DeviceWidth * _model.PreviewScale);
            if (ViewHeight == 0 || ViewWidht == 0)
            {
                return;
            }
            if (_model.SelectedProgram.IsNull())
            {
                return;
            }
            PreviewContent.Children.Clear();
            foreach (var zone in _model.SelectedProgram.Zones)
            {
                var rect = new Rectangle
                {
                    StrokeDashArray = new DoubleCollection
                        {
                            4, 2
                        },
                    StrokeThickness = 1
                };
                if (zone.IsValid)
                {
                    if (zone.Id == (_model.SelectedZone?.Id ?? int.MinValue))
                    {
                        rect.Stroke = Brushes.Yellow;
                        rect.StrokeThickness = 2;
                    }
                    else
                    {
                        rect.Stroke = Brushes.Gray;
                    }
                    var renderer = zoneRenders.FirstOrDefault(kvp => kvp.Key(zone));
                    if (renderer.Key != null && renderer.Value != null)
                    {
                        BinaryFont binaryFont = null;
                        BinaryImage binaryImage = null;
                        if (zone is IFontableZone fontableZone && fontableZone.FontId.HasValue)
                        {
                            binaryFont = _model.Device.Fonts
                                .FirstOrDefault(f => f.Id == fontableZone.FontId);
                        }
                        if (zone is BitmapZone bitmapZone)
                        {
                            binaryImage = _model.Device.BinaryImages
                                .FirstOrDefault(i => i.Id == bitmapZone.BinaryImageId);
                        }
                        renderer.Value(zone,
                                       PreviewContent,
                                       binaryFont,
                                       binaryImage,
                                       _model.PreviewScale);
                    }
                }
                else
                {
                    rect.Stroke = Brushes.Red;
                }
                Canvas.SetTop(rect, zone.Y * _model.PreviewScale);
                Canvas.SetLeft(rect, zone.X * _model.PreviewScale);
                rect.Width = zone.Width * _model.PreviewScale;
                rect.Height = zone.Height * _model.PreviewScale;
                PreviewContent.Children.Add(rect);
            }
        }

        private DelegateCommand _mouseLeave;
        public Input.ICommand MouseLeave
        {
            get
            {
                if (_mouseLeave.IsNull())
                {
                    _mouseLeave = new DelegateCommand((o) =>
                      {
                          if (_currentMouseState == MouseState.Movement)
                          {
                              _suggestedMouseState = MouseState.Movement;
                              SetCursor(_suggestedMouseState);
                          }
                      });
                }
                return _mouseLeave;
            }
        }

        private DelegateCommand _mouseMove;
        public Input.ICommand MouseMove
        {
            get
            {
                if (_mouseMove.IsNull())
                {
                    _mouseMove = new DelegateCommand((o) =>
                    {
                        var args = (MouseEventArgs)o;
                        var cursorViewX = Convert.ToInt32(args.GetPosition(PreviewContent).X);
                        var cursorViewY = Convert.ToInt32(args.GetPosition(PreviewContent).Y);
                        if (_currentMouseState == MouseState.Movement)
                        {
                            SuggestMouseState(cursorViewX, cursorViewY, out _suggestedMouseState, out Zone zone);
                            SetCursor(_suggestedMouseState);
                        }
                        else if (_currentMouseState == MouseState.Drag)
                        {
                            SetCursor(MouseState.Drag);
                            var deltaX = Convert.ToInt32((cursorViewX - _capturedCursorX) / _model.PreviewScale);
                            var deltaY = Convert.ToInt32((cursorViewY - _capturedCursorY) / _model.PreviewScale);
                            var zoneLeft = _capturedZoneX + deltaX;
                            var zoneTop = _capturedZoneY + deltaY;
                            var zoneRight = zoneLeft + _capturedZoneWidth;
                            var zoneBottom = zoneTop + _capturedZoneHeight;
                            if (zoneTop >= 0 && zoneBottom <= _model.DeviceHeight)
                            {
                                _model.ZoneTop = zoneTop;
                            }
                            if (zoneLeft >= 0 && zoneRight <= _model.DeviceWidth)
                            {
                                _model.ZoneLeft = zoneLeft;
                            }
                        }
                        else if (_currentMouseState == MouseState.HorizontalResizing)
                        {
                            SetCursor(MouseState.HorizontalResizing);
                            var deltaX = Convert.ToInt32((cursorViewX - _capturedCursorX) / _model.PreviewScale);
                            var zoneWidth = _capturedZoneWidth + deltaX;
                            if (zoneWidth > 0)
                            {
                                _model.ZoneWidth = zoneWidth;
                            }
                        }
                        else if (_currentMouseState == MouseState.VerticalResizing)
                        {
                            SetCursor(MouseState.VerticalResizing);
                            var deltaY = Convert.ToInt32((cursorViewY - _capturedCursorY) / _model.PreviewScale);
                            var zoneHeight = _capturedZoneHeight + deltaY;
                            if (zoneHeight > 0)
                            {
                                _model.ZoneHeight = zoneHeight;
                            }
                        }
                        else if (_suggestedMouseState == MouseState.DiagonalResiging)
                        {
                            SetCursor(MouseState.DiagonalResiging);
                            var deltaX = Convert.ToInt32((cursorViewX - _capturedCursorX) / _model.PreviewScale);
                            var deltaY = Convert.ToInt32((cursorViewY - _capturedCursorY) / _model.PreviewScale);
                            var zoneHeight = _capturedZoneHeight + deltaY;
                            if (zoneHeight > 0)
                            {
                                _model.ZoneHeight = zoneHeight;
                            }
                            var zoneWidth = _capturedZoneWidth + deltaX;
                            if (zoneWidth > 0)
                            {
                                _model.ZoneWidth = zoneWidth;
                            }
                        }
                    });
                }
                return _mouseMove;
            }
        }

        private Zone _capturedZone;
        private int _capturedZoneX;
        private int _capturedZoneY;
        private int _capturedZoneHeight;
        private int _capturedZoneWidth;
        private int _capturedCursorX;
        private int _capturedCursorY;

        private DelegateCommand _mouseDown;
        public Input.ICommand MouseDown
        {
            get
            {
                if (_mouseDown.IsNull())
                {
                    _mouseDown = new DelegateCommand((o) =>
                    {
                        if (_suggestedMouseState == MouseState.Movement)
                        {
                            return;
                        }
                        _currentMouseState = _suggestedMouseState;
                        SetCursor(_currentMouseState);
                        var args = (MouseButtonEventArgs)o;
                        var cursorViewX = Convert.ToInt32(args.GetPosition(PreviewContent).X);
                        var cursorViewY = Convert.ToInt32(args.GetPosition(PreviewContent).Y);
                        SuggestMouseState(cursorViewX, cursorViewY, out MouseState suggestedMouseState, out Zone zone);
                        _capturedZone = zone;
                        _capturedZoneX = zone.X;
                        _capturedZoneY = zone.Y;
                        _capturedZoneHeight = zone.Height;
                        _capturedZoneWidth = zone.Width;
                        _capturedCursorX = cursorViewX;
                        _capturedCursorY = cursorViewY;
                        if (!_capturedZone.IsNull())
                        {
                            _model.SelectedZone = _capturedZone;
                        }
                    });
                }
                return _mouseDown;
            }
        }

        private void SetCursor(MouseState state)
        {
            Cursor = Cursors.Arrow;
            if (state == MouseState.HorizontalResizing)
            {
                Cursor = Cursors.SizeWE;
            }
            else if (state == MouseState.VerticalResizing)
            {
                Cursor = Cursors.SizeNS;
            }
            else if (state == MouseState.DiagonalResiging)
            {
                Cursor = Cursors.SizeNWSE;
            }
            else if (state == MouseState.Drag)
            {
                Cursor = Cursors.SizeAll;
            }
        }

        private DelegateCommand _mouseWheel;
        public Input.ICommand MouseWheel
        {
            get
            {
                if (_mouseWheel.IsNull())
                {
                    _mouseWheel = new DelegateCommand((o) =>
                    {
                        if (_currentMouseState != MouseState.Movement)
                            return;
                        var increment = .1;
                        var args = (MouseWheelEventArgs)o;
                        var delta = args.Delta;
                        if (delta < 0 && _model.PreviewScale > _model.PreviewScaleMinRate)
                        {
                            _model.PreviewScale -= increment;
                            Redraw();
                        }
                        else if (delta > 0 && _model.PreviewScale < _model.PreviewScaleMaxRate)
                        {
                            _model.PreviewScale += increment;
                            Redraw();
                        }
                    });

                }
                return _mouseWheel;
            }
        }

        private DelegateCommand _mouseUp;
        public Input.ICommand MouseUp
        {
            get
            {
                if (_mouseUp.IsNull())
                {
                    _mouseUp = new DelegateCommand((o) =>
                    {
                        OnMouseUp();
                    });
                }
                return _mouseUp;
            }
        }

        public void OnMouseUp()
        {
            if (_suggestedMouseState == MouseState.Movement)
            {
                return;
            }
            _currentMouseState = MouseState.Movement;
        }

        private readonly Dictionary<Func<Zone, bool>, Action<Zone, Canvas, BinaryFont, BinaryImage, double>> zoneRenders
            = new Dictionary<Func<Zone, bool>, Action<Zone, Canvas, BinaryFont, BinaryImage, double>>
            {
                {
                    (z) => z is TextZone,
                    (zone, canvas, font, image, scale) =>
                    {
                        RenderText(canvas, font, ((IFontableZone)zone).Alignment, ((TextZone)zone).Text, zone.X, zone.Y, zone.Width, zone.Height, scale);
                    }
                },
                {
                    (z) => z is SensorZone,
                    (zone, canvas, font, image, scale) =>
                    {
                        RenderText(canvas, font, ((IFontableZone)zone).Alignment, "[Sensor]", zone.X, zone.Y, zone.Width, zone.Height, scale);
                    }
                },
                {
                    (z) => z is TagZone,
                    (zone, canvas, font, image, scale) =>
                    {
                        RenderText(canvas, font, ((IFontableZone)zone).Alignment, "[MQTT]", zone.X, zone.Y, zone.Width, zone.Height, scale);
                    }
                },
                {
                    (z) => z is BitmapZone,
                    (zone, canvas, font, image, scale) =>
                    {
                        RenderBitmap(canvas, image?.Base64String ?? String.Empty, image?.Height ?? 0, zone.X, zone.Y, zone.Width, zone.Height, scale);
                    }
                },
                {
                    (z) => (z is ClockZone clockZone && clockZone.ClockType == 1), //Текстовые часы
                    (zone, canvas, font, image, scale) =>
                    {
                        var clockZone = zone as ClockZone;
                        RenderText(canvas, font, ((IFontableZone)zone).Alignment, clockZone.Sample, zone.X, zone.Y, zone.Width, zone.Height, scale);
                    }
                },
                {
                    (z) => (z is ClockZone clockZone && clockZone.ClockType == 2), //Графические часы
                    (zone, canvas, font, image, scale) =>
                    {
                        DrawClockPicture(canvas, zone.X, zone.Y, zone.Width, zone.Height, scale);
                    }
                },
                {
                    (z) => z.ZoneType==(int)ZoneTypes.Ticker,
                    (zone, canvas, font, image, scale) =>
                    {
                        RenderText(canvas, font, ((IFontableZone)zone).Alignment, "00:00.0000", zone.X, zone.Y, zone.Width, zone.Height, scale);
                    }
                },

            };

        private static void DrawClockPicture(Canvas canvas, int x, int y, int width, int height, double scale)
        {
            var diameter = ((width < height) ? width : height) - 4;
            if (diameter < 0)
                return;
            var circle = new Ellipse
            {
                Stroke = Brushes.Red,
                Height = diameter * scale,
                Width = diameter * scale
            };
            Canvas.SetTop(circle, (y + 2) * scale);
            Canvas.SetLeft(circle, (x + 2) * scale);

            var verticalLine = new Line
            {
                Stroke = Brushes.Red,
                X1 = (x + 2 + diameter / 2) * scale,
                Y1 = (y + 2 + diameter / 2) * scale,
                X2 = (x + 2 + diameter / 2) * scale,
                Y2 = (y + 3) * scale,
            };

            var horizontalLine = new Line
            {
                Stroke = Brushes.Red,
                X1 = (x + 2 + diameter / 2) * scale,
                Y1 = (y + 2 + diameter / 2) * scale,
                X2 = (x + 3) * scale,
                Y2 = (y + 2 + diameter / 2) * scale,
            };

            canvas.Children.Add(circle);
            canvas.Children.Add(verticalLine);
            canvas.Children.Add(horizontalLine);
        }

        private static void RenderBitmap(Canvas canvas, string bitmapBase64, int bitmapHeight, int x, int y, int width, int height, double scale)
        {
            if (String.IsNullOrEmpty(bitmapBase64))
            {
                return;
            }
            var bitmap = BitmapProcessing.BitmapProcessor.GetMonochromeBitmap(bitmapBase64, bitmapHeight, System.Drawing.Color.Red);
            PutBitmapOnCanvas(bitmap, canvas, x, y, width, height, scale);
        }

        private static void RenderText(Canvas canvas, BinaryFont font, int? alignment, string text, int x, int y, int width, int height, double scale)
        {
            if (font.IsNull())
            {
                return;
            }
            if (String.IsNullOrEmpty(text))
            {
                return;
            }
            System.Drawing.FontStyle style = System.Drawing.FontStyle.Regular;
            if (font.Italic && font.Bold)
            {
                style = System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Bold;
            }
            else if (font.Italic)
            {
                style = System.Drawing.FontStyle.Italic;
            }
            else if (font.Bold)
            {
                style = System.Drawing.FontStyle.Bold;
            }
            var drawingFont = new System.Drawing.Font(
                font.Source, font.Height, style, System.Drawing.GraphicsUnit.Pixel);
            var bitmap = BitmapProcessing.BitmapProcessor.DrawTextImage(
                text,
                drawingFont,
                System.Drawing.Color.Red,
                System.Drawing.Color.Transparent,
                System.Drawing.Size.Empty) as System.Drawing.Bitmap;
            var deltaY = (height - font.Height) / 2;
            if (deltaY < 0)
            {
                deltaY = 0;
            }
            var top = y + deltaY;

            var align = alignment ?? 0;
            int deltaX;
            if (align == 1)
            {
                deltaX = (width - bitmap.Width);
            }
            else if (align == 2)
            {
                deltaX = (width - bitmap.Width) / 2;
            }
            else
            {
                deltaX = 0;
            }
            if (deltaX < 0)
                deltaX = 0;

            var left = x + deltaX;
            PutBitmapOnCanvas(bitmap, canvas, left, top, width, height, scale);
        }

        private static void PutBitmapOnCanvas(System.Drawing.Bitmap bitmap, Canvas canvas, int x, int y, int width, int height, double scale)
        {
            var imageWidth = bitmap.Width;
            var imageHeight = bitmap.Height;
            var trimmedWidth = (imageWidth < width) ? imageWidth : width;
            var trimmedHeight = (imageHeight < height) ? imageHeight : height;
            bitmap = bitmap.Clone(new System.Drawing.Rectangle(0, 0, trimmedWidth, trimmedHeight), bitmap.PixelFormat);

            var hBitmap = bitmap.GetHbitmap();

            var imageControl = new Image
            {
                Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap,
                            IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()),
                SnapsToDevicePixels = true,
                UseLayoutRounding = false,
            };
            RenderOptions.SetBitmapScalingMode(imageControl, BitmapScalingMode.NearestNeighbor);
            var m = imageControl.LayoutTransform.Value;
            m.Scale(scale, scale);
            imageControl.LayoutTransform = new MatrixTransform(m);
            Canvas.SetTop(imageControl, y * scale);
            Canvas.SetLeft(imageControl, x * scale);
            canvas.Children.Add(imageControl);
        }

        private void SuggestMouseState(int cursorViewX, int cursorViewY, out MouseState suggestedMouseState, out Zone involvedZone)
        {
            var sensitiveBorderDelta = 2;
            suggestedMouseState = MouseState.Movement;
            involvedZone = null;
            foreach (var zone in _model.SelectedProgram.Zones)
            {
                var zoneViewX1 = (zone.X) * _model.PreviewScale;
                var zoneViewX2 = (zone.X + zone.Width) * _model.PreviewScale;
                var zoneViewY1 = (zone.Y) * _model.PreviewScale;
                var zoneViewY2 = (zone.Y + zone.Height) * _model.PreviewScale;

                if (cursorViewX.Between(zoneViewX2 - sensitiveBorderDelta, zoneViewX2 + sensitiveBorderDelta) &&
                    cursorViewY.Between(zoneViewY2 - sensitiveBorderDelta, zoneViewY2 + sensitiveBorderDelta))
                {
                    involvedZone = zone;
                    suggestedMouseState = MouseState.DiagonalResiging;
                    return;
                }

                if (cursorViewX.Between(zoneViewX2 - sensitiveBorderDelta, zoneViewX2 + sensitiveBorderDelta) &&
                cursorViewY.Between(zoneViewY1, zoneViewY2 + zone.Height))
                {
                    involvedZone = zone;
                    suggestedMouseState = MouseState.HorizontalResizing;
                    return;
                }
                if (cursorViewX.Between(zoneViewX1, zoneViewX2) &&
                    cursorViewY.Between(zoneViewY2 - sensitiveBorderDelta, zoneViewY2 + sensitiveBorderDelta))
                {
                    involvedZone = zone;
                    suggestedMouseState = MouseState.VerticalResizing;
                    return;
                }
                if (cursorViewX.Between(zoneViewX1, zoneViewX2) &&
                    cursorViewY.Between(zoneViewY1, zoneViewY2))
                {
                    involvedZone = zone;
                    suggestedMouseState = MouseState.Drag;
                    return;
                }
            }
        }
    }

    public enum MouseState
    {
        Movement,
        Drag,
        HorizontalResizing,
        VerticalResizing,
        DiagonalResiging
    }
}