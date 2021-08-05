using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace VideoChat.Models
{
    public class User
    {
        [Key]
        public string UserName { get; set; }
        public string Password { get; set; }

        public User() {}

        public User(string UserName, string Password)
        {
            this.UserName = UserName;
            this.Password = Password;
        }
    }
}