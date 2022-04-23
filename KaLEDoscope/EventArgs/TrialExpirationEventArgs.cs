using System;

namespace KaLEDoscope
{
    public class TrialExpirationEventArgs : System.EventArgs
    {
        public DateTime ExpirationDate { get; set; }
    }
}
