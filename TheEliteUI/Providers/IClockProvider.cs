using System;

namespace TheEliteUI.Providers
{
    public interface IClockProvider
    {
        DateTime Now { get; }

        DateTime Today { get; }
    }
}
