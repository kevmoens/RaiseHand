using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace RaiseHand.Core.SignalR
{
    public class ConnectEvent : PubSubEvent<Connect>
    {

        private static EventAggregator _eventAggregator;
        private static ConnectEvent _event;

        static ConnectEvent() {
            _eventAggregator = new EventAggregator();
            _event = _eventAggregator.GetEvent<ConnectEvent>();
        }
        public static ConnectEvent Instance {
            get {
                return _event;
            }
        }

    }
    public class Connect
    {
        public string Method { get; set; }
        public Dictionary<string, object> Parms { get; set; }
    }
}
