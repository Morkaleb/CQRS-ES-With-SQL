using CQRSWITHES.Infra.EventStore;
using System;
using System.Linq;
using System.Reflection;


namespace CQRSWITHES.Infra.EventStore
{
    public class EventWorker
    {
        public static void Work(Events evt)
        {
            evt.TimeStamp = DateTime.Now;
            EventModel normalizedEvent = NormalizeEvent(evt);
            EventStore.Store(normalizedEvent);
            EventQueue.Queue(normalizedEvent);
        }

        private static EventModel NormalizeEvent(Events evt)
        {
            EventModel publishableEvent = new EventModel
            {
                Id = evt.Id,
                EventType = evt.GetType().ToString().Split('.').Last(),
                TimeStamp = DateTime.Now
            };
            Type typedEvent = evt.GetType();
            foreach (PropertyInfo propertyInfo in typedEvent.GetProperties())
            {
                if (propertyInfo.Name.ToString() != "Id" && 
                    propertyInfo.Name.ToString() != "TimeStamp" &&
                    propertyInfo.Name.ToString() != "EventType")
                {
                    var value = GetValues(evt, propertyInfo.Name).ToString();
                    publishableEvent.body.Add(propertyInfo.Name, value);
                }
            }
            return publishableEvent;
        }

        private static object GetValues(object anEvent, string name)
        {
            return anEvent.GetType().GetProperties()
                .Single(pi => pi.Name == name)
                .GetValue(anEvent, null);
        }
    }
}
