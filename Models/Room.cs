using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VideoChat.Models
{
    public class Room
    {
        [Key]
        public string identifier { get; set; }
        public string creatorUsername { get; set; }
        public string joinerUsername { get; set; }

        public Room() {}
        public Room(string identifier, string creatorUsername)
        {
            this.identifier = identifier;
            this.creatorUsername = creatorUsername;
        }
    }
}