using Abstractions;
using Extensions;
using PixelBoardDevice.DomainObjects;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using UiCommands;
using Input = System.Windows.Input;

namespace PixelBoardDevice.UI
{
    public class ProgramPreviewViewModel : Notified
    {
        private readonly PixelDeviceViewModel _model;
        private double _scale = 2;
        private MouseState _currentMouseState;
        private MouseState _suggestedMouseState;

        public Canvas PreviewContent { get; set; }

        public int ViewHeight { get; set; }
        public int ViewWidht { get; set; }

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
            ViewHeight = Convert.ToInt32(_model.DeviceHeight * _scale);
            ViewWidht = Convert.ToInt32(_model.DeviceWidth * _scale);
            if (_model.SelectedProgram.IsNull())
            {
                return;
            }
            PreviewContent.Children.Clear();
            foreach (var zone in _model.SelectedProgram.Zones)
            {
                Rectangle rect;
                if (zone.IsValid)
                {
                    if (zone.Id == (_model.SelectedZone?.Id ?? int.MinValue))
                    {
                        rect = new Rectangle
                        {
                            Stroke = Brushes.Green,
                            StrokeDashArray = new DoubleCollection
                        {
                            4, 2
                        },
                            StrokeThickness = 1
                        };
                    }
                    else
                    {
                        rect = new Rectangle
                        {
                            Stroke = Brushes.Gray,
                            StrokeDashArray = new DoubleCollection
                        {
                            4, 2
                        },
                            StrokeThickness = 1
                        };
                    }
                }
                else
                {
                    rect = new Rectangle
                    {
                        Stroke = Brushes.Red,
                        StrokeDashArray = new DoubleCollection
                        {
                            4, 2
                        },
                        StrokeThickness = 1
                    };
                }
                Canvas.SetTop(rect, zone.Y * _scale);
                Canvas.SetLeft(rect, zone.X * _scale);
                rect.Width = zone.Width * _scale;
                rect.Height = zone.Height * _scale;
                PreviewContent.Children.Add(rect);
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
                            if (_suggestedMouseState == MouseState.HorizontalResizing)
                            {
                                Mouse.SetCursor(Cursors.SizeWE);
                            }
                            else if (_suggestedMouseState == MouseState.VerticalResizing)
                            {
                                Mouse.SetCursor(Cursors.SizeNS);
                            }
                            else if (_suggestedMouseState == MouseState.DiagonalResiging)
                            {
                                Mouse.SetCursor(Cursors.SizeNWSE);
                            }
                            else if (_suggestedMouseState == MouseState.Drag)
                            {
                                Mouse.SetCursor(Cursors.SizeAll);
                            }
                            else
                            {
                                Mouse.SetCursor(Cursors.Arrow);
                            }
                        }
                        else if (_currentMouseState == MouseState.Drag)
                        {
                            var deltaX = Convert.ToInt32((cursorViewX - _capturedCursorX) / _scale);
                            var deltaY = Convert.ToInt32((cursorViewY - _capturedCursorY) / _scale);
                            _model.ZoneLeft = _capturedZoneX + deltaX;
                            _model.ZoneTop = _capturedZoneY + deltaY;
                            Redraw();
                        }
#error Обработать Resize, Обработать Scale
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

        private DelegateCommand _mouseUp;
        public Input.ICommand MouseUp
        {
            get
            {
                if (_mouseUp.IsNull())
                {
                    _mouseUp = new DelegateCommand((o) =>
                    {
                        if (_suggestedMouseState == MouseState.Movement)
                        {
                            return;
                        }
                        _currentMouseState = MouseState.Movement;
                    });
                }
                return _mouseUp;
            }
        }

        private void SuggestMouseState(int cursorViewX, int cursorViewY, out MouseState suggestedMouseState, out Zone involvedZone)
        {
            var sensitiveBorderDelta = 1;
            suggestedMouseState = MouseState.Movement;
            involvedZone = null;
            foreach (var zone in _model.SelectedProgram.Zones)
            {
                var zoneViewX1 = (zone.X) * _scale;
                var zoneViewX2 = (zone.X + zone.Width) * _scale;
                var zoneViewY1 = (zone.Y) * _scale;
                var zoneViewY2 = (zone.Y + zone.Height) * _scale;

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
                    cursorViewY.Between(zoneViewY1, zoneViewX2))
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