using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS3V.JSON;

namespace WS3V.Support
{
    public class PubSub_Channel
    {
        public string channel_name_or_uri { get; set; }
        public string channel_meta { get; set; }
        
        private int max_events = 1;
        private List<PubSub_Event> events = new List<PubSub_Event>();

        public bool historical
        {
            get
            {
                if (events.Count <= 0)
                    return false;
                else
                    return true;
            }
        }

        public int number_of_events
        {
            get
            {
                return events.Count;
            }
        }

        public int oldest_event
        {
            get
            {
                return events.OrderBy(e => e.timestamp).FirstOrDefault().timestamp;
            }
        }

        public void add_event(string message)
        {
            add_event(new PubSub_Event(message));
        }
        
        public void add_event(PubSub_Event _event)
        {
            if (max_events != 0 && events.Count >= max_events)
            {
                lock (events)
                {
                    if (max_events == 1)
                        events.Clear();

                    else
                        events.OrderBy(e => e.timestamp).ToList().RemoveAt(0);
                }
            }

            lock (events)
                events.Add(_event);
        }

        public PubSub_Channel(string channel_name_or_uri)
        {
            this.channel_name_or_uri = channel_name_or_uri;
            channel_meta = null;
        }

        public PubSub_Channel(string channel_name_or_uri, string channel_meta)
        {
            this.channel_name_or_uri = channel_name_or_uri;
            this.channel_meta = JSONEncoders.EncodeJsString(channel_meta);
        }

        public PubSub_Channel(string channel_name_or_uri, object channel_meta)
        {
            this.channel_name_or_uri = channel_name_or_uri;
            this.channel_meta = channel_meta.ToString();
        }

        public List<PubSub_Event> GetEvents(int count)
        {
            if (count == 0 || count > events.Count)
                return events;

            else
                return events.OrderBy(e => e.timestamp).Take(count).ToList();
        }

        public void Empty()
        {
            events.Clear();
        }

        public void Set_Max(int max)
        {
            max_events = max;
        }

        public void Set_Unlimited()
        {
            max_events = 0;
        }
    }
}
