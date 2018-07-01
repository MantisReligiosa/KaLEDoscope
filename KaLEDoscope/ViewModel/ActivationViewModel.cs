using Abstractions;
using Activation;
using Extensions;
using System;
using UiCommands;
using Input = System.Windows.Input;

namespace KaLEDoscope.ViewModel
{
    public class ActivationViewModel : Notified
    {
        private readonly ActivationManager _activationManager;
        public event EventHandler FullyActivationSucceeded;
        public event EventHandler DuplicatedTrialKeyFound;
        public event EventHandler CopyToClipboard;
        public event EventHandler PasteFromClipboard;
        public event EventHandler<TrialExpirationEventArgs> TrialActivationSucceeded;

        private string _requesCode;
        public string RequestCode
        {
            get
            {
                return _requesCode;
            }
            set
            {
                _requesCode = value;
                OnPropertyChanged(nameof(RequestCode));
            }
        }

        public string ActivationKey { get; set; }

        public ActivationViewModel(ActivationManager activationManager)
        {
            _activationManager = activationManager;
        }

        public void GenerateRequestCode()
        {
            RequestCode = _activationManager.GetRequestCode();
        }

        private DelegateCommand _activate;
        public Input.ICommand Activate
        {
            get
            {
                if (_activate.IsNull())
                {
                    _activate = new DelegateCommand((o) =>
                    {
                        var activationKey = ActivationKey;
                        if (_activationManager.IsFullyActivationKeyValid(activationKey))
                        {
                            _activationManager.SetFullAccess();
                            FullyActivationSucceeded?.Invoke(this, EventArgs.Empty);
                        }
                        else if (_activationManager.IsTrialActivationKeyValid(activationKey))
                        {
                            if (!_activationManager.IsTrialKeyOriginal(activationKey))
                            {
                                DuplicatedTrialKeyFound.Invoke(this, EventArgs.Empty);
                            }
                            else
                            {
                                var expirationDate = DateTime.Now.Date.AddMonths(1);
                                _activationManager.AddTrial(activationKey, expirationDate);
                                TrialActivationSucceeded?.Invoke(this, new TrialExpirationEventArgs
                                {
                                    ExpirationDate = expirationDate
                                });
                            }
                        }
                    });
                }
                return _activate;
            }
        }

        private DelegateCommand _copyToClipboard;
        public Input.ICommand CopyToClipboardCommand
        {
            get
            {
                if (_copyToClipboard.IsNull())
                {
                    _copyToClipboard = new DelegateCommand((o) =>
                    {
                        CopyToClipboard?.Invoke(this, EventArgs.Empty);
                    });
                }
                return _copyToClipboard;
            }
        }

        private DelegateCommand _pasteFromClipboard;
        public Input.ICommand PasteFromClipboardCommand
        {
            get
            {
                if (_pasteFromClipboard.IsNull())
                {
                    _pasteFromClipboard = new DelegateCommand((o) =>
                    {
                        PasteFromClipboard?.Invoke(this, EventArgs.Empty);
                    });
                }
                return _pasteFromClipboard;
            }
        }
    }
}
