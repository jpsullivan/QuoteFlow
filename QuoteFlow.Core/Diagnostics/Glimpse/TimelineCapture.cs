using System;
using Glimpse.Core.Extensibility;

namespace QuoteFlow.Core.Diagnostics.Glimpse
{
    public class TimelineCapture : IDisposable
    {
        private readonly string _eventName;
        private readonly IExecutionTimer _timer;
        private readonly IMessageBroker _broker;
        private readonly TimeSpan _startOffset;

        public TimelineCapture(IExecutionTimer timer, IMessageBroker broker, string eventName)
        {
            _timer = timer;
            _broker = broker;
            _eventName = eventName;
            _startOffset = _timer.Start();
        }

        public void Dispose()
        {
            _broker.Publish(new TimelineMessage(_eventName, _timer.Stop(_startOffset)));
        }
    }
}