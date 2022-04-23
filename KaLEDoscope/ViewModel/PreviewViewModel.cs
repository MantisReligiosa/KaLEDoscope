using DeviceBuilding;
using SmartTechnologiesM.Base;
using SmartTechnologiesM.Base.Extensions;
using System.Timers;
using UiCommands;
using Input = System.Windows.Input;

namespace KaLEDoscope.ViewModel
{
    public class PreviewViewModel : Notified
    {
        private const string _playIconSource = @"/KaLEDoscope;component/Resources/play.png";
        private const string _pauseIconSource = @"/KaLEDoscope;component/Resources/pause.png";
        private const int _fps = 20;
        private readonly PreviewController _previewController;
        private PreviewState _previewState;
        private readonly Timer _timer;

        public int Duration { get; private set; }
        private int _position;
        public int Position
        {
            get
            {
                return _position;
            }
            set
            {
                if (_position == value)
                    return;
                _position = value;
                _previewController.Tick(value);
                OnPropertyChanged(nameof(Position));
            }
        }
        public string PlayPauseIconSource { get; set; } = _pauseIconSource;
        public bool RepeatEnabled { get; set; }

        public PreviewViewModel(PreviewController previewController)
        {
            _previewController = previewController;
            Duration = previewController.Duration;
            Position = 0;
            SetPlayPauseState(PreviewState.Play);
            _timer = new Timer(1000 / _fps)
            {
                AutoReset = true,
                Enabled = true
            };
            _timer.Elapsed += ((sender, args) =>
            {
                var timer = sender as Timer;
                var interval = (int)timer.Interval;
                if (Position + interval > Duration)
                {
                    Position = 0;
                    if (!RepeatEnabled)
                    {
                        Pause();
                        SetPlayPauseState(PreviewState.Pause);
                    }
                }
                else
                {
                    Position += interval;
                }
            });
        }

        private void SetPlayPauseState(PreviewState previewState)
        {
            _previewState = previewState;
            PlayPauseIconSource = (previewState != PreviewState.Play) ? _playIconSource : _pauseIconSource;
        }

        private enum PreviewState
        {
            Play, Pause
        }

        private DelegateCommand _playPause;
        public Input.ICommand PlayPause
        {
            get
            {
                if (_playPause.IsNull())
                {
                    _playPause = new DelegateCommand((o) =>
                    {
                        if (_previewState == PreviewState.Pause)
                        {
                            Start();
                            _previewState = PreviewState.Play;
                        }
                        else
                        {
                            Pause();
                            _previewState = PreviewState.Pause;
                        }
                        SetPlayPauseState(_previewState);
                    });
                }
                return _playPause;
            }
        }

        private DelegateCommand _stop;
        public Input.ICommand Stop
        {
            get
            {
                if (_stop.IsNull())
                {
                    _stop = new DelegateCommand((o) =>
                    {
                        Pause();
                        Position = 0;
                        _previewState = PreviewState.Pause;
                        SetPlayPauseState(_previewState);
                    });
                }
                return _stop;
            }
        }

        private void Start()
        {
            _timer.Start();
        }

        private void Pause()
        {
            _timer.Stop();
        }
    }
}
