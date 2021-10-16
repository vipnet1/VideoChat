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
        private const int MAX_USERNAME_LENGTH = 20;
        private const int MIN_USERNAME_LENGTH = 1;
        private const int MAX_PASSWORD_LENGTH = 50;
        private const int MIN_PASSWORD_LENGTH = 6;

        UserContext db = new UserContext();
        DAL.DAL_allActions dal = new DAL.DAL_allActions();

        private bool wasOnMainPage() {
            if (Session["wasOnMainPage"] == null) return false;
            return (bool)Session["wasOnMainPage"];
        }
        private void setWasOnMainPage() { Session["wasOnMainPage"] = true; }
        private HttpCookie CreateCookie(string name)
        {
            HttpCookie cookie = new HttpCookie(name);
            cookie.Value = (Session["usr"] as User).UserName;
            return cookie;
        }

        public bool isLoggedIn() { return Session["usr"] != null; }
        public bool isRoomReady() { return Session["room"] != null; }

        public ActionResult Room_To_Index()
        {
            Session["room"] = null;
            return new RedirectResult("Index");
        }
        
        // GET: My
        public ActionResult Index()
        {
            if(!wasOnMainPage() && !isLoggedIn())
            {
                setWasOnMainPage();
            }
            return View();
        }
        public ActionResult LogIn()
        {
            if (!wasOnMainPage()) return new RedirectResult("Index");
            return View();
        }
        public ActionResult SignUp()
        {
            if (!wasOnMainPage()) return new RedirectResult("Index");
            return View();
        }

        public ActionResult Connect()
        {
            if (!wasOnMainPage()) return new RedirectResult("Index");
            if (isLoggedIn())
            {
                return View();
            }
            else
                return new RedirectResult("Login");
        }

        public ActionResult Room()
        {
            if (!wasOnMainPage()) return new RedirectResult("Index");
            if (isRoomReady())
                return View(Session["room"] as Room);
            else
                return new RedirectResult("Connect");
        }

        [HttpPost]
        public async Task<string> ActionLogIn(string UserName, string Password)
        {
            if (UserName.Length > MAX_USERNAME_LENGTH || Password.Length > MAX_PASSWORD_LENGTH)
                return "error - too long password/username, hacker :)";
            else if(UserName.Length < MIN_USERNAME_LENGTH || Password.Length < MIN_PASSWORD_LENGTH)
                return "error - too short password/username, hacker :)";

            User u = new User(UserName, Password);
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
            if (UserName.Length > MAX_USERNAME_LENGTH || Password.Length > MAX_PASSWORD_LENGTH)
                return "error - too long password/username, hacker :)";
            else if (UserName.Length < MIN_USERNAME_LENGTH || Password.Length < MIN_PASSWORD_LENGTH)
                return "error - too short password/username, hacker :)";

            User u = new User(UserName, Password);
            string res = await dal.SignUp(u);
            if(res == "success")
            {
                Session["usr"] = u;
                Response.Cookies.Add(CreateCookie("usr"));
            }
            return res;
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
                return new RedirectResult("Room");
            else
            {
                return Content("");
            }
        }
    }
}