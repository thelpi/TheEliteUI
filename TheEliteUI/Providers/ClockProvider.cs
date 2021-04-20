using System;

namespace TheEliteUI.Providers
{
    public sealed class ClockProvider : IClockProvider
    {
        public DateTime Now => DateTime.Now;

        public DateTime Today => DateTime.Today;
    }
}
