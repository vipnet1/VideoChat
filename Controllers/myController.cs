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

        private bool wasOnMainPage() {
            if (Session["wasOnMainPage"] == null) return false;
            return (bool)Session["wasOnMainPage"];
        }
        private void setWasOnMainPage() { Session["wasOnMainPage"] = true; }
        private HttpCookie CreateCookie(string name)
        {
            HttpCookie cookie = new HttpCookie(name);
            cookie.Value = (Session["usr"] as User).UserName;
            cookie.Expires = DateTime.Now.AddDays(1);
            return cookie;
        }

        public bool isLoggedIn() { return Session["usr"] != null; }
        public bool isRoomReady() { return Session["room"] != null; }

        private async Task<bool> ValidateLoginCookie(HttpCookie usrCookie)
        {
            if (usrCookie != null)
            {
                User u = await dal.GetUser(usrCookie.Value);
                if (u != null)
                {
                    Session["usr"] = u;
                    return true;
                }
            }
            return false;
        }

        public ActionResult Room_To_Index()
        {
            Session["room"] = null;
            return new RedirectResult("Index");
        }
        
        // GET: My
        public async Task<ActionResult> Index()
        {
            if(!wasOnMainPage() && !isLoggedIn())
            {
                setWasOnMainPage();
                await ValidateLoginCookie(Request.Cookies.Get("usr"));
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
                return Room();
            else
            {
                return Content("");
            }
        }
    }
}