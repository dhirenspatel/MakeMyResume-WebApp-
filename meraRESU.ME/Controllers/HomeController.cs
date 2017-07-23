using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using meraRESU.ME.Models;
using System.Net;
using System.IO;
using System.Data.Entity;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Drawing;
using Facebook;

namespace meraRESU.ME.Controllers
{
    public class HomeController : Controller
    {
        userDbContext db = new userDbContext();

        public ActionResult Index()
        {
            if (Session["userId"] != null)
            {
                return RedirectToAction("Resume", "Profile");
            }
            
            return View();
        }

        [HttpPost]
        public ActionResult Index(Employee emp)
        {
            Employee e = new Employee();
            int Id = e.checkLogInOrSignUp(emp.Email, emp.Password);

            if (Id > 0)
            {
                return RedirectToAction("SetSession", "Home", new { Id = Id });
            }
            else
            {
                ViewBag.error = "Username and password may be wrong";
            }
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Terms()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
        
        [HttpGet]
        public ActionResult LoginWithFacebook()
        {
            string req = "https://www.facebook.com/dialog/oauth?client_id=" + System.Web.Configuration.WebConfigurationManager.AppSettings["facebookappid"] + "&redirect_uri=http://" + System.Web.Configuration.WebConfigurationManager.AppSettings["DomainName"] + "/Home/returnfromfb&scope=user_about_me,user_hometown,user_location,email,offline_access";
            return Redirect(req);
        }

        public ActionResult returnfromfb()
        {
            Employee e = new Employee();
            string code = Request.QueryString["code"];
            if (code == null)
            {
                return RedirectToAction("LastWindow", "Login");
            }
            string AccessToken = "";
            try
            {
                if (code != null)
                {
                    string str = "https://graph.facebook.com/oauth/access_token?client_id=" + System.Web.Configuration.WebConfigurationManager.AppSettings["facebookappid"] + "&redirect_uri=http://" + Request.Url.Authority + "/Home/returnfromfb&client_secret=" + System.Web.Configuration.WebConfigurationManager.AppSettings["facebookappsecret"] + "&code=" + code;
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(str);
                    req.Method = "POST";
                    req.ContentType = "application/x-www-form-urlencoded";
                    byte[] Param = Request.BinaryRead(System.Web.HttpContext.Current.Request.ContentLength);
                    string strRequest = System.Text.Encoding.ASCII.GetString(Param);

                    req.ContentLength = strRequest.Length;

                    StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII);
                    streamOut.Write(strRequest);
                    streamOut.Close();
                    StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
                    string strResponse = streamIn.ReadToEnd();
                    if (strResponse.Contains("&expires"))
                        strResponse = strResponse.Substring(0, strResponse.IndexOf("&expires"));
                    AccessToken = strResponse.Replace("access_token=", "");
                    streamIn.Close();
                }

                Facebook.FacebookAPI api = new Facebook.FacebookAPI(AccessToken);
                string request = "/me";

                Facebook.JSONObject fbobject = api.Get(request);
                try
                {
                    string fbId = fbobject.Dictionary["id"].String;
                    string email = fbobject.Dictionary["email"].String;
                    Employee e2 = db.EmployeeDB.Where(x => x.Email == email).FirstOrDefault();
                    if (e2 == null)
                    {
                        Employee e1 = db.EmployeeDB.Where(x => x.FacebookId == fbId).FirstOrDefault();
                        if (e1 == null)
                        {
                            e.FirstName = fbobject.Dictionary["first_name"].String;
                            e.LastName = fbobject.Dictionary["last_name"].String;
                            e.Email = email;
                            e.Gender = fbobject.Dictionary["gender"].String;
                            if (fbobject.Dictionary.ContainsKey("birthday"))
                            {
                                e.DateOfBirth = Convert.ToDateTime(fbobject.Dictionary["birthday"].String);
                            }
                            e.FacebookId = fbId;
                            e.Created = DateTime.Now;
                            e.Updated = DateTime.Now;
                            db.EmployeeDB.Add(e);
                            db.SaveChanges();
                            Session["userId"] = e.Id;
                            Session["firstLogin"] = true;

                            string str = "http://graph.facebook.com/" + fbobject.Dictionary["id"].String + "?fields=picture.type(large)";
                            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(str);
                            StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
                            string strResponse = streamIn.ReadToEnd();
                            JObject je = new JObject();
                            je = JObject.Parse(strResponse);

                            string ProfilePicURL = je["picture"]["data"]["url"].ToString().Replace("\"", "");
                            UploadProfilePic(ProfilePicURL, e.Id);
                        }
                        else
                        {
                            return RedirectToAction("Resume","Profile");
                        }
                    }
                    else
                    {
                        e2.FacebookId = fbId;
                        db.SaveChanges();
                        Session["userId"] = e2.Id;
                        return RedirectToAction("Resume","Profile");
                    }
                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("Resume","Profile");
        }

        private bool UploadProfilePic(string ProfilePicURL, int? UserID)
        {
            bool flag = false;


            string fname = DateTime.Now.ToString("yyyyMMddhhmmss");
            string ext = Path.GetExtension(ProfilePicURL);
            var path = Path.Combine(Server.MapPath("/Document/" + UserID + "/"));

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(ProfilePicURL);
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream receiveStream = httpWebResponse.GetResponseStream();
            Image imageFB = Image.FromStream(receiveStream);
            string fileName = Path.Combine(path, fname + ext);
            imageFB.Save(fileName);

            ResizeImage img = new ResizeImage();
            Bitmap image = new Bitmap(path + fname + ext);

            string file_81x81 = path + fname + "_81x81" + ext;
            Bitmap b_81x81 = img.ResizeBitmap(image, 81, 81);
            img.saveJpg(file_81x81, b_81x81, 100);

            Employee emp = db.EmployeeDB.Find(UserID);

            emp.Logo = fname + ext;
            db.SaveChanges();
            flag = true;
            return flag;
        }

        public ActionResult SetSession(int Id)
        {
            if (Id != null)
            {
                Session["userId"] = Id;
                return RedirectToAction("Resume", "Profile");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
            
        }

        public JsonResult LogInOrSignUp(string email, string pwd)
        {
            Employee e = new Employee();
            int Id = e.checkLogInOrSignUp(email, pwd);
            return Json(Id, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShareOnFacebook()
        {
            Session.Remove("firstLogin");
            string req = "https://www.facebook.com/dialog/oauth?client_id=" + System.Web.Configuration.WebConfigurationManager.AppSettings["facebookappid"] + "&redirect_uri=http://" + System.Web.Configuration.WebConfigurationManager.AppSettings["DomainName"] + "/Home/ReturnFromFbToShare&scope=user_about_me,user_hometown,user_location,email,offline_access,publish_stream";
            return Redirect(req);
        }

        public ActionResult ReturnFromFbToShare()
        {
            string code = Request.QueryString["code"];
            if (code == null)
            {

            }

            string AccessToken = "";
            try
            {

                if (code != null)
                {
                    string str = "https://graph.facebook.com/oauth/access_token?client_id=" + System.Web.Configuration.WebConfigurationManager.AppSettings["facebookappid"] + "&redirect_uri=http://" + Request.Url.Authority + "/Home/ReturnFromFbToShare&client_secret=" + System.Web.Configuration.WebConfigurationManager.AppSettings["facebookappsecret"] + "&code=" + code;
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(str);
                    req.Method = "POST";
                    req.ContentType = "application/x-www-form-urlencoded";
                    byte[] Param = Request.BinaryRead(System.Web.HttpContext.Current.Request.ContentLength);
                    string strRequest = System.Text.Encoding.ASCII.GetString(Param);

                    req.ContentLength = strRequest.Length;

                    StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII);
                    streamOut.Write(strRequest);
                    streamOut.Close();
                    StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
                    string strResponse = streamIn.ReadToEnd();
                    if (strResponse.Contains("&expires"))
                        strResponse = strResponse.Substring(0, strResponse.IndexOf("&expires"));
                    AccessToken = strResponse.Replace("access_token=", "");
                    streamIn.Close();
                }

                var client = new FacebookClient(AccessToken);

                var parameters = new Dictionary<string, object>
                         {
                             { "message" , "meraRESU.ME" },
                             { "name" ,  "meraRESU.ME - Save Papers, Save Bandwidth" },
                             { "description" ,  "We are trying to make process of creating and applying resume online rather than pdf or word file" },
                             { "picture" ,  "http://meraresu.me/Content/images/logo.png" },
                             { "caption" ,  "meraRESU.ME" },
                             { "link" ,  "http://meraresu.me/" },
                             { "type" , "link" }
                         };

                client.Post("/me/feed", parameters);

            }
            catch (Exception ex)
            {
                return RedirectToAction("Resume", "Profile");
            }
            return RedirectToAction("Resume", "Profile");
        }

        public JsonResult DestroySessionByName(string sessionName)
        {
            Session.Remove(sessionName);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult encryptpwd()
        {
            IList<Employee> emp = db.EmployeeDB.ToList();
            if (emp != null)
            {
                if (emp.Count > 0)
                {
                    foreach (var e in emp)
                    {
                        e.Password = e.encryptPwd(e.Password);
                    }
                    db.SaveChanges();
                }
            }

            //return RedirectToAction("index", "home");
            return View();
        }
    }
}
