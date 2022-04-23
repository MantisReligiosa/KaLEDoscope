using PixelBoardDevice.DomainObjects;
using PixelBoardDevice.DomainObjects.Zones;
using SmartTechnologiesM.Base;
using SmartTechnologiesM.Base.Extensions;
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

namespace PixelBoardDevice.UI
{
    public class ProgramPreviewViewModel : Notified
    {
        private readonly PixelDeviceViewModel _model;
        private MouseState _currentMouseState;
        private MouseState _suggestedMouseState;
        private readonly bool _previewMode;

        public Canvas PreviewContent { get; set; }

        public int ViewHeight { get; set; }
        public int ViewWidht { get; set; }
        public Cursor Cursor { get; set; }

        public ProgramPreviewViewModel(PixelDeviceViewModel pixelDeviceViewModel, bool previewMode = false)
        {
            PreviewContent = new Canvas();
            _model = pixelDeviceViewModel;
            _model.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_model.PreviewedProgram) && !_previewMode)
                    return;
                Redraw();
            };
            _previewMode = previewMode;
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
            Program program;
            if (!_previewMode)
            {
                program = _model.SelectedProgram;
            }
            else
            {
                program = _model.PreviewedProgram;
            }
            if (program.IsNull())
            {
                PreviewContent.Children.Clear();
                return;
            }
            Application.Current.Dispatcher.Invoke(delegate
            {
                PreviewContent.Children.Clear();
                foreach (var zone in program.Zones)
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
                    if (!_previewMode || (_previewMode && !zone.IsValid))
                    {
                        PreviewContent.Children.Add(rect);
                    }
                }
            });
        }

        private DelegateCommand _mouseLeave;
        public ICommand MouseLeave
        {
            get
            {
                if (_mouseLeave.IsNull())
                {
                    _mouseLeave = new DelegateCommand((o) =>
                      {
                          if (_previewMode)
                              return;
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

        private readonly Object _locker = new Object();

        private DelegateCommand _mouseMove;
        public ICommand MouseMove
        {
            get
            {
                if (_mouseMove.IsNull())
                {
                    _mouseMove = new DelegateCommand((o) =>
                    {
                        lock (_locker)
                        {
                            if (_previewMode)
                                return;
                            var args = (MouseEventArgs)o;
                            var cursorViewX = Convert.ToInt32(args.GetPosition(PreviewContent).X);
                            var cursorViewY = Convert.ToInt32(args.GetPosition(PreviewContent).Y);
                            if (_currentMouseState == MouseState.Movement)
                            {
                                SuggestMouseState(cursorViewX, cursorViewY, out _suggestedMouseState, out Zone zone);
                                SetCursor(_suggestedMouseState);
                                SuggestMouseState(cursorViewX, cursorViewY, out MouseState suggestedMouseState, out Zone capturedZone);
                                if (!capturedZone.IsNull())
                                {
                                    _capturedZone = capturedZone;
                                    _capturedZoneX = capturedZone.X;
                                    _capturedZoneY = capturedZone.Y;
                                    _capturedZoneHeight = capturedZone.Height;
                                    _capturedZoneWidth = capturedZone.Width;
                                    _capturedCursorX = cursorViewX;
                                    _capturedCursorY = cursorViewY;
                                }
                                return;
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
                                return;
                            }
                            
                            if (_currentMouseState == MouseState.HorizontalResizingRight 
                                || _currentMouseState == MouseState.DiagonalResigingNE 
                                || _currentMouseState == MouseState.DiagonalResigingSE)
                            {
                                var deltaX = Convert.ToInt32((cursorViewX - _capturedCursorX) / _model.PreviewScale);
                                var zoneWidth = _capturedZoneWidth + deltaX;
                                if (zoneWidth >= 0)
                                {
                                    _model.ZoneWidth = zoneWidth;
                                }
                            }

                           if (_currentMouseState == MouseState.HorizontalResizingLeft
                                || _currentMouseState == MouseState.DiagonalResigingNW
                                || _currentMouseState == MouseState.DiagonalResigingSW)
                            {
                                var deltaX = Convert.ToInt32((cursorViewX - _capturedCursorX) / _model.PreviewScale);
                                var zoneWidth = _capturedZoneWidth - deltaX;
                                var zoneX = _capturedZoneX + deltaX;
                                if (zoneWidth >= 0 && zoneX >= 0)
                                {
                                    _model.ZoneWidth = zoneWidth;
                                    _model.ZoneLeft = zoneX;
                                }
                            }

                            if (_currentMouseState == MouseState.VerticalResizingUp
                                || _currentMouseState == MouseState.DiagonalResigingNE
                                || _currentMouseState == MouseState.DiagonalResigingNW
                            )
                            {
                                var deltaY = Convert.ToInt32((cursorViewY - _capturedCursorY) / _model.PreviewScale);
                                var zoneHeight = _capturedZoneHeight - deltaY;
                                var zoneY = _capturedZoneY + deltaY;
                                if (zoneHeight >= 0 && zoneY >= 0)
                                {
                                    _model.ZoneHeight = zoneHeight;
                                    _model.ZoneTop = zoneY;
                                }
                            }

                            if (_currentMouseState == MouseState.VerticalResizingDown
                                || _currentMouseState == MouseState.DiagonalResigingSE
                                || _currentMouseState == MouseState.DiagonalResigingSW)
                            {
                                var deltaY = Convert.ToInt32((cursorViewY - _capturedCursorY) / _model.PreviewScale);
                                var zoneHeight = _capturedZoneHeight + deltaY;
                                if (zoneHeight >= 0)
                                {
                                    _model.ZoneHeight = zoneHeight;
                                }
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
        public ICommand MouseDown
        {
            get
            {
                if (_mouseDown.IsNull())
                {
                    _mouseDown = new DelegateCommand((o) =>
                    {
                        lock (_locker)
                        {
                            if (_previewMode)
                                return;
                            if (_suggestedMouseState == MouseState.Movement)
                            {
                                return;
                            }
                            _currentMouseState = _suggestedMouseState;
                            SetCursor(_currentMouseState);
                            if (!_capturedZone.IsNull())
                            {
                                _model.SelectedZone = _capturedZone;
                            }
                        }
                    });
                }
                return _mouseDown;
            }
        }

        private void SetCursor(MouseState state)
        {
            Cursor = Cursors.Arrow;
            if (state == MouseState.HorizontalResizingLeft || state == MouseState.HorizontalResizingRight)
            {
                Cursor = Cursors.SizeWE;
            }
            else if (state == MouseState.VerticalResizingUp || state == MouseState.VerticalResizingDown)
            {
                Cursor = Cursors.SizeNS;
            }
            else if (state == MouseState.DiagonalResigingNE || state == MouseState.DiagonalResigingSW)
            {
                Cursor = Cursors.SizeNESW;
            }
            else if (state == MouseState.DiagonalResigingNW || state == MouseState.DiagonalResigingSE)
            {
                Cursor = Cursors.SizeNWSE;
            }
            else if (state == MouseState.Drag)
            {
                Cursor = Cursors.SizeAll;
            }
        }

        private DelegateCommand _mouseWheel;
        public ICommand MouseWheel
        {
            get
            {
                if (_mouseWheel.IsNull())
                {
                    _mouseWheel = new DelegateCommand((o) =>
                    {
                        if (_previewMode)
                            return;
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
        public ICommand MouseUp
        {
            get
            {
                if (_mouseUp.IsNull())
                {
                    _mouseUp = new DelegateCommand((o) =>
                    {
                        if (_previewMode)
                            return;
                        OnMouseUp();
                    });
                }
                return _mouseUp;
            }
        }

        public void OnMouseUp()
        {
            if (_previewMode)
                return;
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
                        RenderBitmap(canvas, image?.Bytes ?? new byte[]{ }, image?.Height ?? 0, zone.X, zone.Y, zone.Width, zone.Height, scale);
                    }
                },
                {
                    (z) => (z is ClockZone clockZone && clockZone.ClockType == ClockTypes.Digital), //Текстовые часы
                    (zone, canvas, font, image, scale) =>
                    {
                        var clockZone = zone as ClockZone;
                        RenderText(canvas, font, ((IFontableZone)zone).Alignment, clockZone.Sample, zone.X, zone.Y, zone.Width, zone.Height, scale);
                    }
                },
                {
                    (z) => (z is ClockZone clockZone && clockZone.ClockType == ClockTypes.Analog), //Графические часы
                    (zone, canvas, font, image, scale) =>
                    {
                        DrawClockPicture(canvas, zone.X, zone.Y, zone.Width, zone.Height, scale);
                    }
                },
                {
                    (z) => z.ZoneType == ZoneTypes.Ticker,
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
            Canvas.SetTop(circle, (y + (height - diameter) / 2) * scale);
            Canvas.SetLeft(circle, (x + (width - diameter) / 2) * scale);

            var verticalLine = new Line
            {
                Stroke = Brushes.Red,
                X1 = (x + (width - diameter) / 2 + diameter / 2) * scale,
                Y1 = (y + (height - diameter) / 2) * scale,
                X2 = (x + (width - diameter) / 2 + diameter / 2) * scale,
                Y2 = (y + (height - diameter) / 2 + diameter / 2) * scale,
            };

            var horizontalLine = new Line
            {
                Stroke = Brushes.Red,
                X1 = (x + (width - diameter) / 2 + diameter / 2) * scale,
                Y1 = (y + (height - diameter) / 2 + diameter / 2) * scale,
                X2 = (x + (width - diameter) / 2) * scale,
                Y2 = (y + (height - diameter) / 2 + diameter / 2) * scale,
            };

            canvas.Children.Add(circle);
            canvas.Children.Add(verticalLine);
            canvas.Children.Add(horizontalLine);
        }

        private static void RenderBitmap(Canvas canvas, byte[] bytes, int bitmapHeight, int x, int y, int width, int height, double scale)
        {
            if (!bytes.Any())
            {
                return;
            }
            var bitmap = BitmapProcessing.BitmapProcessor.GetMonochromeBitmap(bytes, bitmapHeight);
            PutBitmapOnCanvas(bitmap, canvas, x, y, width, height, scale);
        }

        private static void RenderText(Canvas canvas, BinaryFont font, TextAlignment? alignment, string text, int x, int y, int width, int height, double scale)
        {
            if (font.IsNull())
            {
                return;
            }
            if (string.IsNullOrEmpty(text))
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
                text.Replace(Constants.LineSplitString, "\r\n"),
                drawingFont,
                alignment,
                System.Drawing.Color.Red,
                System.Drawing.Color.Transparent,
                new System.Drawing.Size
                {
                    Height = height,
                    Width = width
                }) as System.Drawing.Bitmap;

            PutBitmapOnCanvas(bitmap, canvas, x, y, width, height, scale);
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
            if (_model.SelectedProgram.IsNull())
            {
                return;
            }
            involvedZone = _model.SelectedProgram.Zones.FirstOrDefault(z =>
            z.X * _model.PreviewScale < cursorViewX &&
            (z.X + z.Width) * _model.PreviewScale + sensitiveBorderDelta > cursorViewX &&
            z.Y * _model.PreviewScale < cursorViewY &&
            (z.Y + z.Height) * _model.PreviewScale + sensitiveBorderDelta > cursorViewY
            );

            if (involvedZone.IsNull())
            {
                return;
            }

            var zoneViewX1 = involvedZone.X * _model.PreviewScale;
            var zoneViewX2 = (involvedZone.X + involvedZone.Width) * _model.PreviewScale;
            var zoneViewY1 = involvedZone.Y * _model.PreviewScale;
            var zoneViewY2 = (involvedZone.Y + involvedZone.Height) * _model.PreviewScale;

            if (cursorViewX.Between(zoneViewX1 - sensitiveBorderDelta, zoneViewX1 + sensitiveBorderDelta) &&
                cursorViewY.Between(zoneViewY2 - sensitiveBorderDelta, zoneViewY2 + sensitiveBorderDelta))
            {
                suggestedMouseState = MouseState.DiagonalResigingSW;
                return;
            }

            if (cursorViewX.Between(zoneViewX2 - sensitiveBorderDelta, zoneViewX2 + sensitiveBorderDelta) &&
                cursorViewY.Between(zoneViewY1 - sensitiveBorderDelta, zoneViewY1 + sensitiveBorderDelta))
            {
                suggestedMouseState = MouseState.DiagonalResigingNE;
                return;
            }

            if (cursorViewX.Between(zoneViewX1 - sensitiveBorderDelta, zoneViewX1 + sensitiveBorderDelta) &&
            cursorViewY.Between(zoneViewY1 - sensitiveBorderDelta, zoneViewY1 + sensitiveBorderDelta)
            )
            {
                suggestedMouseState = MouseState.DiagonalResigingNW;
                return;
            }

            if (cursorViewX.Between(zoneViewX2 - sensitiveBorderDelta, zoneViewX2 + sensitiveBorderDelta) &&
            cursorViewY.Between(zoneViewY2 - sensitiveBorderDelta, zoneViewY2 + sensitiveBorderDelta))
            {
                suggestedMouseState = MouseState.DiagonalResigingSE;
                return;
            }

            if (cursorViewX.Between(zoneViewX2 - sensitiveBorderDelta, zoneViewX2 + sensitiveBorderDelta) &&
                cursorViewY.Between(zoneViewY1, zoneViewY2))
            {
                suggestedMouseState = MouseState.HorizontalResizingRight;
                return;
            }

            if (cursorViewX.Between(zoneViewX1 - sensitiveBorderDelta, zoneViewX1 + sensitiveBorderDelta) &&
                cursorViewY.Between(zoneViewY1, zoneViewY2))
            {
                suggestedMouseState = MouseState.HorizontalResizingLeft;
                return;
            }

            if (cursorViewX.Between(zoneViewX1, zoneViewX2) &&
            cursorViewY.Between(zoneViewY2 - sensitiveBorderDelta, zoneViewY2 + sensitiveBorderDelta))
            {
                suggestedMouseState = MouseState.VerticalResizingDown;
                return;
            }

            if (cursorViewX.Between(zoneViewX1, zoneViewX2) &&
            cursorViewY.Between(zoneViewY1 - sensitiveBorderDelta, zoneViewY1 + sensitiveBorderDelta))
            {
                suggestedMouseState = MouseState.VerticalResizingUp;
                return;
            }

            if (cursorViewX.Between(zoneViewX1, zoneViewX2) &&
            cursorViewY.Between(zoneViewY1, zoneViewY2))
            {
                suggestedMouseState = MouseState.Drag;
            }
        }
    }

    public enum MouseState
    {
        Movement,
        Drag,
        DiagonalResigingNE,
        DiagonalResigingSW,
        DiagonalResigingNW,
        DiagonalResigingSE,
        HorizontalResizingRight,
        HorizontalResizingLeft,
        VerticalResizingDown,
        VerticalResizingUp
    }
}
