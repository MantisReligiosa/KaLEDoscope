using Abstractions;
using Extensions;
using PixelBoardDevice.DomainObjects;
using System;
using System.Drawing.Drawing2D;
using System.Windows;
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