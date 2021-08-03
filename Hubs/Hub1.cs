using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalR.Hubs
{
    public class ChatHub : Hub
    {

        public class ConnectionInfo
        {
            public string connectionId;
            public string sdp;

            public ConnectionInfo(string connectionId, string sdp)
            {
                this.connectionId = connectionId;
                this.sdp = sdp;
            }
        }

        public static System.Collections.Generic.Dictionary<string, ConnectionInfo> dict = new Dictionary<string, ConnectionInfo>();
        public void Send(string fromUname, string toUname, string message)
        {
            Clients.All.getNewMessage("byebye");
            dict[fromUname] = new ConnectionInfo(Context.ConnectionId, message);
        }

        //retirieve username and sdp
        public void rtr_FirstStepInitiator(string uname, string sdp)
        {
            dict[uname] = new ConnectionInfo(Context.ConnectionId, sdp);
        }

        //retirieve username and sdp
        public void rtr_SecondStepJoiner(string uname, string unameToConnect)
        {
            dict[uname] = new ConnectionInfo(Context.ConnectionId, "");
            Clients.Caller.firstSDP(dict[unameToConnect].sdp);
        }

        //retirieve username and sdp
        public void rtr_ThirdStepJoiner(string uname, string sdp, string unameToConnect)
        {
            dict[uname].sdp = sdp;
            Clients.Client(dict[unameToConnect].connectionId).secondSDP(dict[uname].sdp);
        }
    }
}