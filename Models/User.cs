
using System.ComponentModel.DataAnnotations;
using VideoChat2.Models;

namespace VideoChat.Models
{
    public class User
    {
        [Key]
        public string UserName { get; set; } //varchar(20)
        public string Password { get; set; } //varchar(64)
        public string Salt { get; set; } //varchar(64)

        public User() {}

        public User(string UserName, string Password)
        {
            this.UserName = UserName;
            this.Password = Password;
        }
    }
}