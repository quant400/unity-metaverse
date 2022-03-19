using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CFC
{
    public class Chat
    {
        public string Id;
        private List<Message> _messages;
    }
    
    public class Message
    {
        public string Content;
        public string WriterId;
        public string ReceiverId;
    }
}
