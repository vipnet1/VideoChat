using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VideoChat.Models;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace VideoChat.Controllers
{
    public class myController : Controller
    {
        UserContext db = new UserContext();
        DAL.DAL_allActions dal = new DAL.DAL_allActions();

        private HttpCookie CreateCookie(string name)
        {
            HttpCookie cookie = new HttpCookie(name);
            cookie.Value = (Session["usr"] as User).UserName;
            cookie.Expires = DateTime.Now.AddHours(1);
            return cookie;
        }

        public bool isLoggedIn() { return Session["usr"] != null; }
        public bool isRoomReady() { return Session["room"] != null; }
        
        // GET: My
        public ActionResult Index()
        {
            IEnumerable<User> Employees = db.Users;
            if (db.Users != null)
                Console.WriteLine("success");
            return View();

        }
        public ActionResult LogIn()
        {
            if (!isLoggedIn())
                return View();
            else
                return new RedirectResult("Index");
        }
        public ActionResult SignUp()
        {
            if (!isLoggedIn())
                return View();
            else
                return new RedirectResult("Index");
        }

        public ActionResult Connect()
        {
            if(isLoggedIn())
            {
                if(!isRoomReady())
                    return View();
                else
                    return new RedirectResult("Room");
            }
            else
                return new RedirectResult("Login");
        }

        public ActionResult Room()
        {
            if (isRoomReady())
                return View(Session["room"] as Room);
            else
                return new RedirectResult("Connect");
        }

        [HttpPost]
        public async Task<string> ActionLogIn(string UserName, string Password)
        {
            User u = new User();
            u.UserName = UserName;
            u.Password = Password;
            User curr = await dal.LogIn(u);
            if(curr != null)
            {
                Session["usr"] = curr;
                Response.Cookies.Add(CreateCookie("usr"));
                return "success";
            }
            return "error";
        }
        [HttpPost]
        public async Task<string> ActionSignUp(string UserName, string Password)
        {
            User u = new User();
            u.UserName = UserName;
            u.Password = Password;
            return await dal.SignUp(u);
        }

        [HttpGet]
        public async Task<ActionResult> ActionNewRoom()
        {
            if(isRoomReady())
                return new RedirectResult("Room");
            if (!isLoggedIn())
                return new RedirectResult("Login");

            string code =  DAL.DAL_allActions.RandomString(32);
            Session["room"] = await dal.CreateRoom(code, (Session["usr"] as User).UserName);
            return new RedirectResult("Room");
        }

        [HttpGet]
        public async Task<ActionResult> ActionJoinRoom(string code)
        {
            if (isRoomReady())
                return new RedirectResult("Room");
            if (!isLoggedIn())
                return new RedirectResult("Login");

            Session["room"] = await dal.JoinRoom(code, (Session["usr"] as User).UserName);
            if (isRoomReady())
                return Room();
            else
            {
                return Content("");
            }
        }
    }
}