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

        //key - username. value - connectioninfo
        public static System.Collections.Generic.Dictionary<string, ConnectionInfo> dict = new Dictionary<string, ConnectionInfo>();


        //retirieve username and sdp of initiator
        public void rtr_FirstStepInitiator(string uname, string sdp)
        {
            dict[uname] = new ConnectionInfo(Context.ConnectionId, sdp);
        }

        //retirieve username and who to connect, send sdp of initiator to joiner
        public void rtr_SecondStepJoiner(string uname, string unameToConnect)
        {
            dict[uname] = new ConnectionInfo(Context.ConnectionId, "");
            Clients.Caller.firstSDP(dict[unameToConnect].sdp);
        }

        //retirieve username, sdp and who to connect, send sdp of joiner and his uname to initiator
        public void rtr_ThirdStepJoiner(string uname, string sdp, string unameToConnect)
        {
            dict[uname].sdp = sdp;
            Clients.Client(dict[unameToConnect].connectionId).secondSDP(dict[uname].sdp, uname);

            dict.Remove(uname);
            dict.Remove(unameToConnect);
        }
    }
}