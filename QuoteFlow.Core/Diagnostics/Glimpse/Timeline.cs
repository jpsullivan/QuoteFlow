using System;
using Glimpse.Core.Framework;

namespace QuoteFlow.Core.Diagnostics.Glimpse
{
    public static class Timeline
    {
        public static IDisposable Capture(string eventName)
        {
#pragma warning disable 618
            var timer = GlimpseConfiguration.GetConfiguredTimerStrategy()();
            if (timer == null)
                return null;
            var broker = GlimpseConfiguration.GetConfiguredMessageBroker();
            if (broker == null)
                return null;
#pragma warning restore 618
            return new TimelineCapture(timer, broker, eventName);
        }
    }
}