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
        public string identifier { get; set; } //varchar(32) PRIMARY KEY
        public string creatorUsername { get; set; } //varchar(20) NOT NULL
        public string joinerUsername { get; set; } //varchar(20)

        public Room() {}

        public Room(string identifier) { this.identifier = identifier; }
        public Room(string identifier, string creatorUsername)
        {
            this.identifier = identifier;
            this.creatorUsername = creatorUsername;
        }
    }
}