using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VideoChat.Controllers;
using VideoChat.Models;

namespace SignalR.Hubs
{
    public class ChatHub : Hub
    {
        public class ConnectionInfo
        {
            public string connectionId;
            public string sdp;
            public string identifier;

            public ConnectionInfo(string connectionId, string sdp, string identifier)
            {
                this.connectionId = connectionId;
                this.sdp = sdp;
                this.identifier = identifier;
            }
        }

        //key - username. value - connectioninfo
        public static System.Collections.Generic.Dictionary<string, ConnectionInfo> dict = new Dictionary<string, ConnectionInfo>();


        //retirieve username and sdp of initiator
        public void rtr_FirstStepInitiator(string uname, string sdp, string identifier)
        {
            dict[uname] = new ConnectionInfo(Context.ConnectionId, sdp, identifier);
        }

        //retirieve username and who to connect, send sdp of initiator to joiner
        public void rtr_SecondStepJoiner(string uname, string unameToConnect, string identifier)
        {
            dict[uname] = new ConnectionInfo(Context.ConnectionId, "", identifier);
            Clients.Caller.firstSDP(dict[unameToConnect].sdp);
        }

        //retirieve username, sdp and who to connect, send sdp of joiner and his uname to initiator
        public void rtr_ThirdStepJoiner(string uname, string sdp, string unameToConnect)
        {
            dict[uname].sdp = sdp;
            Clients.Client(dict[unameToConnect].connectionId).secondSDP(dict[uname].sdp, uname);

            UserContext db = new UserContext();
            Room roomToRemove = new Room(dict[uname].identifier);
            db.Rooms.Attach(roomToRemove);
            db.Entry(roomToRemove).State = System.Data.Entity.EntityState.Deleted;
            db.SaveChangesAsync();

            dict.Remove(uname);
            dict.Remove(unameToConnect);
        }
    }
}