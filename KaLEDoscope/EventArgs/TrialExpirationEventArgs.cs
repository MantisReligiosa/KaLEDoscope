using System;

namespace KaLEDoscope
{
    public class TrialExpirationEventArgs : EventArgs
    {
        public DateTime ExpirationDate { get; set; }
    }
}
