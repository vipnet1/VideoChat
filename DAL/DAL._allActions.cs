using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using VideoChat.Models;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using VideoChat2.Models;

namespace VideoChat.DAL
{
    public class DAL_allActions
    {
        UserContext db = new UserContext();
        private static Random random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";


        public static string RandomString(int length)
        {
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task<Room> CreateRoom(string code, string creatorUsername)
        {
            Room r = new Room(code, creatorUsername);
            db.Rooms.Add(r);
            await db.SaveChangesAsync();
            return r;
        }

        public async Task<Room> JoinRoom(string code, string joinerUsername)
        {
            Room r = await db.Rooms.FindAsync(code);
            if(r != null)
            {
                r.joinerUsername = joinerUsername;
                await db.SaveChangesAsync();
            }
            return r;
        }

        public async Task<string> SignUp(User u)
        {
            User findUser = await db.Users.FindAsync(u.UserName);
            if (findUser == null)
            {
                u.Salt = SecurityHelper.GenerateSalt(64);
                u.Password = SecurityHelper.HashPassword(u.Password + SecurityHelper.pepper, u.Salt, SecurityHelper.nrIterations, SecurityHelper.hashLen);
                db.Users.Add(u);
                await db.SaveChangesAsync();
                return "success";
            }
            return "error, it seems that the UserName already exist, please change your UserName";
        }

        public async Task<User> GetUser(string name)
        {
            return await db.Users.FindAsync(name);
        }

        public async Task<User> LogIn(User u)
        {
            User findUser = await db.Users.FindAsync(u.UserName);

            if (findUser != null)
            {
                string pwdHashed = SecurityHelper.HashPassword(u.Password + SecurityHelper.pepper, findUser.Salt, SecurityHelper.nrIterations, SecurityHelper.hashLen);
                if(pwdHashed.Equals(findUser.Password))
                    return findUser;
            }
            return null;
        }

        public async Task<string> RemoveRoomAsync(string roomId)
        {
            Room roomToRemove = new Room(roomId);
            db.Rooms.Attach(roomToRemove);
            db.Entry(roomToRemove).State = System.Data.Entity.EntityState.Deleted;
            await db.SaveChangesAsync();
            return "success";
        }
    }
}