using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ColorCodeBreaker.Models.FbCommon;

namespace ColorCodeBreaker.Models
{
    public class FbMessengerResponse
    {
        public Recipient recipient { get; set; }
        public string messaging_type { get; set; }
        public Message message { get; set; }

        public class Quick_Replies
        {
            public string content_type { get; set; }
            public string title { get; set; }
            public string payload { get; set; }
            public string image_url { get; set; }
        }

        public class Message
        {
            public string text { get; set; }
            public List<Quick_Replies> quick_replies { get; set; }
        }
    }
}
