using System;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Message;

namespace QuoteFlow.Core.Diagnostics.Glimpse
{
    public class TimelineMessage : ITimelineMessage
    {
        private static readonly TimelineCategoryItem DefaultCategory = new TimelineCategoryItem("MyApp", "green", "blue");

        public TimelineMessage(string eventName, TimerResult result)
        {
            Id = Guid.NewGuid();
            EventName = eventName;
            EventCategory = DefaultCategory;
            Offset = result.Offset;
            StartTime = result.StartTime;
            Duration = result.Duration;
        }

        public Guid Id { get; private set; }
        public TimeSpan Offset { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime StartTime { get; set; }
        public string EventName { get; set; }
        public TimelineCategoryItem EventCategory { get; set; }
        public string EventSubText { get; set; }
    }
}