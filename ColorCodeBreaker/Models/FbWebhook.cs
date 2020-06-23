using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ColorCodeBreaker.Models.FbCommon;

namespace ColorCodeBreaker.Models
{
    public class FbWebhook
    {
        public string @object { get; set; }
        public List<Entry> entry { get; set; }

        public class Entry
        {
            public string id { get; set; }
            public long time { get; set; }
            public List<Messaging> messaging { get; set; }
        }

        public class Messaging
        {
            public Sender sender { get; set; }
            public Recipient recipient { get; set; }
            public long timestamp { get; set; }
            public Message message { get; set; }
        }

        public class Message
        {
            public string mid { get; set; }
            public string text { get; set; }
        }
    }
}
