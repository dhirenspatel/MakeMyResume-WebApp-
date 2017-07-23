using meraRESU.ME.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.IO;
using System.Drawing;
using System.Net;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;

namespace meraRESU.ME.Controllers
{
    public class ProfileController : Controller
    {
        //
        // GET: /Profile/
        userDbContext db = new userDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Resume()
        {
            if (Session["userId"] != null)
            {
                int userId = Convert.ToInt32(Session["userId"]);
                Employee e = db.EmployeeDB.Include(x => x.EmployeeSkills).Include(x => x.Employeements).Include(x => x.Educations)
                    .Include(x => x.EmployeeAchivements).Include(x => x.EmployeeSeminars).Include(x => x.EmployeeProjects)
                    .Include(x => x.EmployeePublications).Include(x => x.EmployeeReferences)
                    .Where(x => x.Id == userId).FirstOrDefault();

                if (e.UserName == null || e.UserName == "")
                {
                    Session["nousername"] = true;
                }
                else
                {
                    Session["nousername"] = false;
                }
                BindMonths();
                return View(e);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult Resume(string pusername)
        {
            Employee emp = new Employee();
            if (Session["userId"] != null)
            {
                int id = Convert.ToInt32(Session["userId"]);
                emp = db.EmployeeDB.Find(id);
                if (emp != null)
                {
                    emp.UserName = pusername.ToLower().Trim();
                    db.SaveChanges();
                    Session.Remove("nousername");
                    return RedirectToAction("Resume");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            return View(emp);
        }

        public ActionResult View(string uname)
        {
            Employee e = new Employee();
            e=db.EmployeeDB.Include(x => x.EmployeeSkills).Include(x => x.Employeements).Include(x => x.Educations)
                    .Include(x => x.EmployeeAchivements).Include(x => x.EmployeeSeminars).Include(x => x.EmployeeProjects)
                    .Include(x => x.EmployeePublications).Include(x => x.EmployeeReferences)
                //.Where(x => x.Id == id).FirstOrDefault();
                    .Where(x => x.UserName == uname).FirstOrDefault();
            if (e != null)
            {
                return View(e);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            
        }
               
        public ActionResult Privacy()
        {
            return View();
        }

        public JsonResult GetEmployeement(int id)
        {
            Employeement es = null;
            try
            {
                es = db.EmployeementDB.Find(id);
            }
            catch (Exception ex)
            {
                es = null;
            }
            return Json(es, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSkill(int id)
        {
            EmployeeSkill es = null;
            try
            {
                es = db.EmployeeSkillDB.Find(id);
            }
            catch (Exception ex)
            {
                es = null;
            }
            return Json(es, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GetEducation(int id)
        {
            Education ed = null;
            try
            {
                ed = db.EducationDB.Find(id);
            }
            catch (Exception ex)
            {
                ed = null;
            }
            return Json(ed, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReference(int id)
        {
            EmployeeReference er = null;
            try
            {
                er = db.EmployeeReferenceDB.Find(id);
            }
            catch (Exception ex)
            {
                er = null;
            }
            return Json(er, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPublication(int id)
        {
            EmployeePublication ep = null;
            try
            {
                ep = db.EmployeePublicationDB.Find(id);
            }
            catch (Exception ex)
            {
                ep = null;
            }
            return Json(ep, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWorkshop(int id)
        {
            EmployeeSeminar es = null;
            try
            {
                es = db.EmployeeSeminarDB.Find(id);
            }
            catch (Exception ex)
            {
                es = null;
            }
            return Json(es, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAchievement(int id)
        {
            EmployeeAchievement ea = null;
            try
            {
                ea = db.EmployeeAchivementDB.Find(id);
            }
            catch (Exception ex)
            {
                ea = null;
            }
            return Json(ea, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProject(int id)
        {
            EmployeeProject ep = null;
            try
            {
                ep = db.EmployeeProjectDB.Find(id);
            }
            catch (Exception ex)
            {
                ep = null;
            }
            return Json(ep, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProfileDetail(int id)
        {
            Employee emp = null;
            try
            {
                emp = db.EmployeeDB.Find(id);
            }
            catch (Exception ex)
            {
            }
            return Json(emp,JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWorkType()
        {
            var temp = (dynamic)null;
            int userId = Convert.ToInt32(Session["userId"]);
            Employee emp = db.EmployeeDB.Find(userId);
            if (emp != null)
            {
                temp = new {Year=emp.WorkExYear,Month=emp.WorkExMonth };
            }

            return Json(temp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetObjective()
        {
            string objective = string.Empty;
            int userId = Convert.ToInt32(Session["userId"]);
            Employee emp = db.EmployeeDB.Find(userId);
            if(emp!=null)
            {
                objective = emp.Objective;
            }
            return Json(objective, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateProfile(int id, string fname, string lname, string phone, string email, string address, string dob, string nationlity, string gender, string title, string mname)
        {
            Employee emp =null;
            try
            {
                emp = db.EmployeeDB.Find(id);
                if (emp != null)
                {
                    emp.Title = title;
                    emp.FirstName = fname;
                    emp.LastName = lname;
                    emp.MiddleName = mname;
                    emp.Phone = phone;
                    emp.Email = email;
                    emp.Address = address;

                    //IFormatProvider format = new System.Globalization.CultureInfo("en-US", true);
                    //DateTime _dob = DateTime.Parse(dob, format, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
                    DateTime date = DateTime.ParseExact(dob, "dd/MM/yyyy", null);
                    emp.DateOfBirth = date;
                    emp.Nationality = nationlity;
                    emp.Gender = gender;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                emp = null;
            }
            return Json(emp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckForEmail(string email)
        {
            bool flag = false;
            if (email != null)
            {
                if (email != "")
                {
                    int id = Convert.ToInt32(Session["userId"]);
                    Employee emp = db.EmployeeDB.Where(x => x.Email == email && x.Id!=id).FirstOrDefault();
                    if (emp != null)
                    {
                        flag = true;
                    }
                }
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddEmployeement1(int id, string cname, string designation, int fromyear, int toyear, string des, string frommonth, string tomonth)
        {
            string s = string.Empty;
            int userId = Convert.ToInt32(Session["userId"]);
            string check = string.Empty;
            Employeement em = new Employeement();
            try
            {
                if (id == 0)
                {
                    em.Designation = designation;
                    em.CompanyName = cname;
                    em.EmployeeId = userId;
                    em.FromYear = fromyear;
                    em.ToYear = toyear;
                    em.FromMonth = frommonth;
                    em.ToMonth = tomonth;
                    em.Description = des;
                    em.Created = DateTime.Now;
                    em.Updated = DateTime.Now;
                    db.EmployeementDB.Add(em);
                    db.SaveChanges();
                    check = "1";
                }
                else
                {
                    em = db.EmployeementDB.Find(id);
                    em.Designation = designation;
                    em.CompanyName = cname;
                    em.FromYear = fromyear;
                    em.ToYear = toyear;
                    em.FromMonth = frommonth;
                    em.ToMonth = tomonth;
                    em.Description = des;
                    em.Updated = DateTime.Now;
                    db.SaveChanges();
                    check = "2";
                }
                s = bindEmployeementData(em.Id);
                s = s + "#" + check;

            }
            catch (Exception ex)
            {
                s = string.Empty;
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        //add json

        public JsonResult AddSkill(int id, string skill)
        {
            string s = string.Empty;
            string check = string.Empty;
            int userId = Convert.ToInt32(Session["userId"]);
            EmployeeSkill es = new EmployeeSkill();
            try
            {
                if (id == 0)
                {
                    es.EmployeeId = userId;
                    es.Skill = skill;
                    es.Created = DateTime.Now;
                    es.Updated = DateTime.Now;
                    db.EmployeeSkillDB.Add(es);
                    db.SaveChanges();
                    check = "1";
                }
                else
                {
                    es = db.EmployeeSkillDB.Find(id);
                    es.Skill = skill;
                    es.Updated = DateTime.Now;
                    db.SaveChanges();
                    check = "2";
                }
                s = bindSkillData(es.Id);
                s = s + "|^|" + check;
            }
            catch (Exception ex)
            {
                s = string.Empty;
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult AddEducation(int id, string cname, string degree, int fromyear, int toyear, string des)
        {
            string s = string.Empty;
            Education ed = new Education();
            string check = string.Empty;
            int userId = Convert.ToInt32(Session["userId"]);
            try
            {
                if (id == 0)
                {
                    ed.College = cname;
                    ed.Degree = degree;
                    ed.EmployeeId = userId;
                    ed.FromYear = fromyear;
                    ed.ToYear = toyear;
                    ed.Description = des;
                    ed.Created = DateTime.Now;
                    ed.Updated = DateTime.Now;
                    db.EducationDB.Add(ed);
                    db.SaveChanges();
                    check = "1";
                }
                else
                {
                    ed = db.EducationDB.Find(id);
                    ed.College = cname;
                    ed.Degree = degree;
                    ed.FromYear = fromyear;
                    ed.ToYear = toyear;
                    ed.Description = des;
                    ed.Updated = DateTime.Now;
                    db.SaveChanges();
                    check = "2";
                }
                s = bindEducationData(ed.Id);
                s = s + "#" + check;
            }
            catch (Exception ex)
            {
                s = string.Empty;
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddReference(int id, string name, string designation, string email, string phone)
        {
            string s = string.Empty;
            string check = string.Empty;
            int userId = Convert.ToInt32(Session["userId"]);
            EmployeeReference er = new EmployeeReference();
            try
            {
                if (id == 0)
                {
                    er.Reference = name;
                    er.EmployeeId = userId;
                    er.Designation = designation;
                    er.Email = email;
                    er.Phone = phone;
                    er.Created = DateTime.Now;
                    er.Updated = DateTime.Now;
                    db.EmployeeReferenceDB.Add(er);
                    db.SaveChanges();
                    check = "1";
                }
                else
                {
                    er = db.EmployeeReferenceDB.Find(id);
                    er.Reference = name;
                    er.Designation = designation;
                    er.Email = email;
                    er.Phone = phone;
                    er.Updated = DateTime.Now;
                    db.SaveChanges();
                    check = "2";

                }
                s = bindReferenceData(er.Id);
                s = s + "#" + check;
            }
            catch (Exception ex)
            {
                s = string.Empty;
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult AddPublication(int id, string des)
        //{
        //    string s = string.Empty;
        //    string check = string.Empty;
        //    int userId = Convert.ToInt32(Session["userId"]);
        //    EmployeePublication ep = new EmployeePublication();
        //    try
        //    {
        //        if (id == 0)
        //        {
        //            ep.Publication = des;
        //            ep.EmployeeId = userId;
        //            ep.Created = DateTime.Now;
        //            ep.Updated = DateTime.Now;
        //            db.EmployeePublicationDB.Add(ep);
        //            db.SaveChanges();
        //            check = "1";
        //        }
        //        else
        //        {
        //            ep = db.EmployeePublicationDB.Find(id);
        //            ep.Publication = des;
        //            ep.Updated = DateTime.Now;
        //            db.SaveChanges();
        //            check = "2";
        //        }
        //        s = bindPublicationData(ep.Id);
        //        s = s + "#" + check;
        //    }
        //    catch (Exception ex)
        //    {
        //        s = string.Empty;
        //    }
        //    return Json(s, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult AddWorkshop(int id, string des)
        //{
        //    string s = string.Empty;
        //    string check = string.Empty;
        //    int userId = Convert.ToInt32(Session["userId"]);
        //    EmployeeSeminar es = new EmployeeSeminar();
        //    try
        //    {
        //        if (id == 0)
        //        {
        //            es.Seminar = des;
        //            es.EmployeeId = userId;
        //            es.Created = DateTime.Now;
        //            es.Updated = DateTime.Now;
        //            db.EmployeeSeminarDB.Add(es);
        //            db.SaveChanges();
        //            check = "1";
        //        }
        //        else
        //        {
        //            es = db.EmployeeSeminarDB.Find(id);
        //            es.Seminar = des;
        //            es.Updated = DateTime.Now;
        //            db.SaveChanges();
        //            check = "2";
        //        }
        //        s = bindSeminarData(es.Id);
        //        s = s + "#" + check;
        //    }
        //    catch (Exception ex)
        //    {
        //        s = string.Empty;
        //    }
        //    return Json(s, JsonRequestBehavior.AllowGet);
        //}
        [ValidateInput(false)]
        public JsonResult AddPublication(FormCollection fc)
        {
            int id = Convert.ToInt32(fc["PublicationId"]);
            string des = fc["Description6"].ToString();
            string s = string.Empty;
            string check = string.Empty;
            int userId = Convert.ToInt32(Session["userId"]);
            EmployeePublication ep = new EmployeePublication();
            try
            {
                if (id == 0)
                {
                    ep.Publication = des;
                    ep.EmployeeId = userId;
                    ep.Created = DateTime.Now;
                    ep.Updated = DateTime.Now;
                    db.EmployeePublicationDB.Add(ep);
                    db.SaveChanges();
                    check = "1";
                }
                else
                {
                    ep = db.EmployeePublicationDB.Find(id);
                    ep.Publication = des;
                    ep.Updated = DateTime.Now;
                    db.SaveChanges();
                    check = "2";
                }
                s = bindPublicationData(ep.Id);
                s = s + "#" + check;
            }
            catch (Exception ex)
            {
                s = string.Empty;
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public JsonResult AddWorkshop(FormCollection fc)
        {
            int id= Convert.ToInt32(fc["WorkshopId"]);
            string des = fc["Description5"].ToString();
            string s = string.Empty;
            string check = string.Empty;
            int userId = Convert.ToInt32(Session["userId"]);
            EmployeeSeminar es = new EmployeeSeminar();
            try
            {
                if (id == 0)
                {
                    es.Seminar = des;
                    es.EmployeeId = userId;
                    es.Created = DateTime.Now;
                    es.Updated = DateTime.Now;
                    db.EmployeeSeminarDB.Add(es);
                    db.SaveChanges();
                    check = "1";
                }
                else
                {
                    es = db.EmployeeSeminarDB.Find(id);
                    es.Seminar = des;
                    es.Updated = DateTime.Now;
                    db.SaveChanges();
                    check = "2";
                }
                s = bindSeminarData(es.Id);
                s = s + "#" + check;
            }
            catch (Exception ex)
            {
                s = string.Empty;
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult AddAchievement(int id, string des)
        //{
        //    string s = string.Empty;
        //    int userId = Convert.ToInt32(Session["userId"]);
        //    string check = string.Empty;
        //    EmployeeAchievement ea = new EmployeeAchievement();
        //    try
        //    {
        //        if (id == 0)
        //        {
        //            ea.Achievement = des;
        //            ea.EmployeeId = userId;
        //            ea.Created = DateTime.Now;
        //            ea.Updated = DateTime.Now;
        //            db.EmployeeAchivementDB.Add(ea);
        //            db.SaveChanges();
        //            check = "1";
        //        }
        //        else
        //        {
        //            ea = db.EmployeeAchivementDB.Find(id);
        //            ea.Achievement = des;
        //            ea.Updated = DateTime.Now;
        //            db.SaveChanges();
        //            check = "2";
        //        }
        //        s = bindAchievementData(ea.Id);
        //        s = s + "#" + check;
        //    }
        //    catch (Exception ex)
        //    {
        //        s = string.Empty;
        //    }
        //    return Json(s, JsonRequestBehavior.AllowGet);
        //}
        [ValidateInput(false)]
        public JsonResult AddAchievement(FormCollection fc)
        {
            int id = Convert.ToInt32(fc["AchievementId"]);
            string des = fc["Description4"].ToString();
            string s = string.Empty;
            int userId = Convert.ToInt32(Session["userId"]);
            string check = string.Empty;
            EmployeeAchievement ea = new EmployeeAchievement();
            try
            {
                if (id == 0)
                {
                    ea.Achievement = des;
                    ea.EmployeeId = userId;
                    ea.Created = DateTime.Now;
                    ea.Updated = DateTime.Now;
                    db.EmployeeAchivementDB.Add(ea);
                    db.SaveChanges();
                    check = "1";
                }
                else
                {
                    ea = db.EmployeeAchivementDB.Find(id);
                    ea.Achievement = des;
                    ea.Updated = DateTime.Now;
                    db.SaveChanges();
                    check = "2";
                }
                s = bindAchievementData(ea.Id);
                s = s + "#" + check;
            }
            catch (Exception ex)
            {
                s = string.Empty;
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult AddProject(int id, string title, string des)
        //{
        //    string s = string.Empty;
        //    int userId = Convert.ToInt32(Session["userId"]);
        //    string check = string.Empty;
        //    EmployeeProject ep = new EmployeeProject();
        //    try
        //    {
        //        if (id == 0)
        //        {
        //            ep.Title = title;
        //            ep.Description = des;
        //            ep.EmployeeId = userId;
        //            ep.Created = DateTime.Now;
        //            ep.Updated = DateTime.Now;
        //            db.EmployeeProjectDB.Add(ep);
        //            db.SaveChanges();
        //            check = "1";
        //        }
        //        else
        //        {
        //            ep = db.EmployeeProjectDB.Find(id);
        //            ep.Title = title;
        //            ep.Description = des;
        //            ep.Updated = DateTime.Now;
        //            db.SaveChanges();
        //            check = "2";
        //        }
        //        s = bindProjectData(ep.Id);
        //        s = s + "#" + check;
        //    }
        //    catch (Exception ex)
        //    {
        //        s = string.Empty;
        //    }
        //    return Json(s, JsonRequestBehavior.AllowGet);


        //}
        [ValidateInput(false)]
        public JsonResult AddProject(FormCollection fc)
        {
            int id = Convert.ToInt32(fc["ProjectId"]);
            string title = fc["Title"].ToString();
            string des = fc["Description3"].ToString();

            string s = string.Empty;
            int userId = Convert.ToInt32(Session["userId"]);
            string check = string.Empty;
            EmployeeProject ep = new EmployeeProject();
            try
            {
                if (id == 0)
                {
                    ep.Title = title;
                    ep.Description = des;
                    ep.EmployeeId = userId;
                    ep.Created = DateTime.Now;
                    ep.Updated = DateTime.Now;
                    db.EmployeeProjectDB.Add(ep);
                    db.SaveChanges();
                    check = "1";
                }
                else
                {
                    ep = db.EmployeeProjectDB.Find(id);
                    ep.Title = title;
                    ep.Description = des;
                    ep.Updated = DateTime.Now;
                    db.SaveChanges();
                    check = "2";
                }
                s = bindProjectData(ep.Id);
                s = s + "#" + check;
            }
            catch (Exception ex)
            {
                s = string.Empty;
            }
            return Json(s, JsonRequestBehavior.AllowGet);


        }

        public JsonResult EditWorkType(int wexYear, int wexMonth)
        {
            int userId = Convert.ToInt32(Session["userId"]);
            bool flag = false;
            Employee emp = db.EmployeeDB.Find(userId);
            try
            {
                if (emp != null)
                {
                    emp.WorkExYear = wexYear;
                    emp.WorkExMonth = wexMonth;
                    db.SaveChanges();
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                flag = false;
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }


        public JsonResult EditObjective(string objective)
        {
            int userId = Convert.ToInt32(Session["userId"]);
            bool flag = false;
            Employee emp = db.EmployeeDB.Find(userId);
            try
            {
                if (emp != null)
                {
                    emp.Objective = objective;
                    db.SaveChanges();
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                flag = false;
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        //bind 

        private string bindSkillData(int id)
        {
            EmployeeSkill es = db.EmployeeSkillDB.Where(x => x.Id == id).FirstOrDefault();
            StringBuilder sb = new StringBuilder();
            if (es != null)
            {
                sb.Append("<li  id='skill-" + es.Id + "'>" + es.Skill + "");
                sb.Append("<span style='cursor:pointer' class='deleteSkill pull-right' id='" + es.Id + "'><i class='glyphicon glyphicon-remove-circle'></i></span>");
                sb.Append("<span style='cursor:pointer;margin-right:7px' class='editSkill pull-right' id='" + es.Id + "'><i class='glyphicon glyphicon-edit'></i></span>");
            }
            return sb.ToString();
        }

        private string bindEmployeementData(int id)
        {
            Employeement em = db.EmployeementDB.Where(x => x.Id == id).FirstOrDefault();
            StringBuilder sb = new StringBuilder();
            if (em != null)
            {
                sb.Append("<div id='employment-" + em.Id + "'>");
                sb.Append("<h4 class='text-primary'><i class='glyphicon glyphicon-chevron-right'></i>");
                sb.Append("" + em.Designation + "<small>(" + em.CompanyName + ")</small>");
                sb.Append("<span style='cursor:pointer' class='deleteEmployment pull-right' id='" + em.Id + "'><i class='glyphicon glyphicon-remove-circle'></i></span>");
                sb.Append("<span style='cursor:pointer;margin-right:7px' class='editEmployment pull-right' id='" + em.Id + "'><i class='glyphicon glyphicon-edit'></i></span>");
                sb.Append("<small class='pull-right' style='margin-right:15px'>" + em.FromYear + "-" + em.ToYear + "</small></h4>");
                sb.Append("<p>" + em.Description + "</p><hr />");
                sb.Append("</div>");
            }
            return sb.ToString();
        }

        private string bindEducationData(int id)
        {
            Education ed = db.EducationDB.Where(x => x.Id == id).FirstOrDefault();
            StringBuilder sb = new StringBuilder();
            if (ed != null)
            {
                sb.Append("<div id='education-" + ed.Id + "'>");
                sb.Append("<div class='col-md-12'>");
                sb.Append("<h4 class='text-primary'>" + ed.Degree + "(<small>" + ed.FromYear + "-" + ed.ToYear + "</small>)");
                sb.Append("<span style='cursor:pointer' class='deleteEducation pull-right' id='" + ed.Id + "'><i class='glyphicon glyphicon-remove-circle'></i></span>");
                sb.Append("<span style='cursor:pointer;margin-right:7px' class='editEducation pull-right' id='" + ed.Id + "'><i class='glyphicon glyphicon-edit'></i></span>");
                sb.Append("</h4>");
                sb.Append("<small>" + ed.College + "</small>");
                sb.Append("</div><hr />");
                sb.Append("</div>");
            }
            return sb.ToString();
        }

        private string bindReferenceData(int id)
        {
            EmployeeReference er = db.EmployeeReferenceDB.Where(x => x.Id == id).FirstOrDefault();
            StringBuilder sb = new StringBuilder();
            if (er != null)
            {
                sb.Append("<div id='reference-" + er.Id + "'>");
                sb.Append("<div class='col-md-4'>");
                sb.Append("<h4 class='text-primary'><i class='glyphicon glyphicon-user'></i>" + er.Reference + "");
                sb.Append("<span style='cursor:pointer' class='deleteReference pull-right' id='" + er.Id + "'><i class='glyphicon glyphicon-remove-circle'></i></span>");
                sb.Append("<span style='cursor:pointer;margin-right:7px' class='editReference pull-right' id='" + er.Id + "'><i class='glyphicon glyphicon-edit'></i></span>");
                sb.Append("</h4><h4><small>" + er.Designation + "</small></h4>");
                sb.Append("<address><i class='glyphicon glyphicon-envelope'></i>" + er.Email + "<br />");
                sb.Append("<i class='glyphicon glyphicon-earphone'></i>" + er.Phone + "</address></div></div>");
            }
            return sb.ToString();
        }

        private string bindSeminarData(int id)
        {
            EmployeeSeminar es = db.EmployeeSeminarDB.Where(x => x.Id == id).FirstOrDefault();
            StringBuilder sb = new StringBuilder();
            if (es != null)
            {
                sb.Append("<li id='workshop-" + es.Id + "'>" + es.Seminar + "");
                sb.Append("<span style='cursor:pointer' class='deleteWorkshop pull-right' id='" + es.Id + "'><i class='glyphicon glyphicon-remove-circle'></i></span>");
                sb.Append("<span style='cursor:pointer;margin-right:7px'class='editWorkshop pull-right' id='" + es.Id + "'><i class='glyphicon glyphicon-edit'></i></span>");
                sb.Append("</li>");
            }
            return sb.ToString();
        }

        private string bindPublicationData(int id)
        {
            EmployeePublication ep = db.EmployeePublicationDB.Where(x => x.Id == id).FirstOrDefault();
            StringBuilder sb = new StringBuilder();
            if (ep != null)
            {
                sb.Append("<li id='publication-" + ep.Id + "'>" + ep.Publication + "");
                sb.Append("<span style='cursor:pointer' class='deletePublication pull-right' id='" + ep.Id + "'><i class='glyphicon glyphicon-remove-circle'></i></span>");
                sb.Append("<span style='cursor:pointer;margin-right:7px' class='editPublication pull-right' id='" + ep.Id + "'><i class='glyphicon glyphicon-edit'></i></span>");
                sb.Append("</li>");
            }
            return sb.ToString();
        }

        private string bindAchievementData(int id)
        {
            EmployeeAchievement ea = db.EmployeeAchivementDB.Where(x => x.Id == id).FirstOrDefault();
            StringBuilder sb = new StringBuilder();
            if (ea != null)
            {
                sb.Append("<li id='achievement-" + ea.Id + "'>" + ea.Achievement + "");
                sb.Append("<span style='cursor:pointer' class='deleteAchievement pull-right' id='" + ea.Id + "'><i class='glyphicon glyphicon-remove-circle'></i></span>");
                sb.Append("<span style='cursor:pointer;margin-right:7px' class='editAchievement pull-right' id='" + ea.Id + "'><i class='glyphicon glyphicon-edit'></i></span>");
                sb.Append("</li>");
            }
            return sb.ToString();
        }

        private string bindProjectData(int id)
        {
            EmployeeProject ep = db.EmployeeProjectDB.Where(x => x.Id == id).FirstOrDefault();
            StringBuilder sb = new StringBuilder();
            if (ep != null)
            {
                sb.Append("<div id='project-" + ep.Id + "'>");
                sb.Append("<h4 class='text-primary'>" + ep.Title + "");
                sb.Append("<span style='cursor:pointer' class='deleteProject pull-right' id='" + ep.Id + "'><i class='glyphicon glyphicon-remove-circle'></i></span>");
                sb.Append("<span style='cursor:pointer;margin-right:7px' class='editProject pull-right' id='" + ep.Id + "'><i class='glyphicon glyphicon-edit'></i></span>");
                sb.Append("</h4>");
                sb.Append("<p>" + ep.Description + "</p><hr />");
                sb.Append("</div>");
            }
            return sb.ToString();
        }

        //delete

        public JsonResult DeleteSkill(int id)
        {
            bool flag = true;
            try
            {
                EmployeeSkill es = db.EmployeeSkillDB.Find(id);
                db.EmployeeSkillDB.Remove(es);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                flag = false;
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteEmployeement(int id)
        {
            bool flag = true;
            try
            {
                Employeement em = db.EmployeementDB.Find(id);
                db.EmployeementDB.Remove(em);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteEducation(int id)
        {
            bool flag = true;
            try
            {
                Education ed = db.EducationDB.Find(id);
                db.EducationDB.Remove(ed);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteLanguageSkill(int id)
        {
            bool flag = true;
            try
            {
                EmployeeLanguage el = db.EmployeeLanguageDB.Find(id);
                db.EmployeeLanguageDB.Remove(el);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeletePortfolio(int id)
        {
            bool flag = true;
            try
            {
                EmployeePortfolio ep = db.EmployeePortfolioDB.Find(id);
                db.EmployeePortfolioDB.Remove(ep);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeletePublication(int id)
        {
            bool flag = true;
            try
            {
                EmployeePublication ep = db.EmployeePublicationDB.Find(id);
                db.EmployeePublicationDB.Remove(ep);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteWorkshop(int id)
        {
            bool flag = true;
            try
            {
                EmployeeSeminar es = db.EmployeeSeminarDB.Find(id);
                db.EmployeeSeminarDB.Remove(es);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }


        public JsonResult DeleteAchievement(int id)
        {
            bool flag = true;
            try
            {
                EmployeeAchievement ea = db.EmployeeAchivementDB.Find(id);
                db.EmployeeAchivementDB.Remove(ea);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteReference(int id)
        {
            bool flag = true;
            try
            {
                EmployeeReference er = db.EmployeeReferenceDB.Find(id);
                db.EmployeeReferenceDB.Remove(er);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteProject(int id)
        {
            bool flag = true;
            try
            {
                EmployeeProject ep = db.EmployeeProjectDB.Find(id);
                db.EmployeeProjectDB.Remove(ep);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckForUsername(string term)
        {
            bool flag = false;
            if (term != null)
            {
                if (term != "")
                {
                    int id = Convert.ToInt32(Session["userId"]);
                    Employee emp = db.EmployeeDB.Where(x => x.UserName == term.ToLower()).FirstOrDefault();
                    if (emp != null)
                    {
                            flag = true;
                    }
                }
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadPic(HttpPostedFileBase file)
        {
            try
            {
                if (Session["userId"] != null)
                {
                    Employee emp = new Employee();

                    int UserId = Convert.ToInt32(Session["userId"]);
                    emp.Id = UserId;
                    file = Request.Files["uploadPic"];
                    if (file.FileName != "")
                    {
                        if (file.ContentLength > 0)
                        {
                            var ext = Path.GetExtension(file.FileName);

                            if (ext.ToLower() == ".jpg" || ext.ToLower() == ".png" || ext.ToLower() == ".gif" || ext.ToLower() == ".bmp")
                            {
                                string fname = DateTime.Now.ToString("yyyyMMddhhmmss");
                                var path = Path.Combine(Server.MapPath("/Document/" + UserId + "/"));
                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }
                                string fullpath = path + fname + ext;
                                file.SaveAs(fullpath);

                                ResizeImage img = new ResizeImage();
                                Bitmap image = new Bitmap(fullpath);

                                
                                string file_81x81 = path + fname + "_81x81" + ext;
                                Bitmap b_81x81 = img.ResizeBitmap(image, 81, 81);
                                img.saveJpg(file_81x81, b_81x81, 100);

                                emp.Logo = fname + ext;

                                if (emp.UpdateByAction("photo"))
                                {
                                    return RedirectToAction("resume");
                                }
                            }
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("resume");
            }
            finally
            { }
            return View();
        }

        public JsonResult FetchSocialLinks()
        {
            int UserId = Convert.ToInt32(Session["userId"]);
            Employee emp = db.EmployeeDB.Find(UserId);
            var temp = new { 
                FacebookLink = emp.FacebookLink,
                GooglePlusLink = emp.GooglePlusLink,
                LinkedInLink = emp.LinkedInLink,
                TwitterLink = emp.TwitterLink
            };
            return Json(temp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FetchMySkillSet()
        {
            int UserId = Convert.ToInt32(Session["userId"]);
            //List<string> mySkillList = new List<string>();
            List<SelectListItem> mySkillList = new List<SelectListItem>();
            IList<EmployeeSkill> emp = db.EmployeeSkillDB.Where(x => x.EmployeeId == UserId).ToList();
            foreach(var e in emp)
            {
                mySkillList.Add(new SelectListItem { Text=e.Skill, Value=e.Id.ToString()});
            }
            
            
            return Json(mySkillList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateSocialLinks(string FacebookLink, string GoogleLink, string LinkedInLink, string TwitterLink)
        {
            int UserId = Convert.ToInt32(Session["userId"]);
            Employee emp = db.EmployeeDB.Find(UserId);
            bool flag = false;

            if (emp != null)
            {
                emp.FacebookLink = FacebookLink == "" ? null : FacebookLink;
                emp.GooglePlusLink = GoogleLink == "" ? null : GoogleLink;
                emp.LinkedInLink = LinkedInLink == "" ? null : LinkedInLink;
                emp.TwitterLink = TwitterLink == "" ? null : TwitterLink;
                db.SaveChanges();
                flag = true;
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExportResume(int id)
        {
            bool flag = false;
            try
            {
                HTMLToPdf(strBuildPDF());
                flag = true;
            }
            catch (Exception ex)
            {
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        protected string strBuildPDF()
        {
            int userId = Convert.ToInt32(Session["userId"]);
            Employee e = db.EmployeeDB.Include(x => x.EmployeeSkills).Include(x => x.Employeements).Include(x => x.Educations)
                .Include(x => x.EmployeeAchivements).Include(x => x.EmployeeSeminars).Include(x => x.EmployeeProjects)
                .Include(x => x.EmployeePublications).Include(x => x.EmployeeReferences)
                .Where(x => x.Id == userId).FirstOrDefault();
            string fileName = string.Empty;
            string extension = string.Empty;
            string profileImage = string.Empty;
            StringBuilder sb = new StringBuilder();
            //string InvoiceNumber = pjis.GetType().GetProperty("Tran_ID").GetValue(pjis, null).ToString("0000") + "-" + pjis.GetType().GetProperty("User_ID").GetValue(pjis, null).ToString("000");
            sb.Append("<html xmlns='http://www.w3.org/1999/xhtml'>");

            sb.Append("<body>");

            sb.Append("<table>");

            #region Top

            sb.Append("<tr>");
            sb.Append("<td rowspan='6' width='5%'>");
            if (e.Logo != null)
            {
                fileName = Path.GetFileNameWithoutExtension(e.Logo);
                extension = Path.GetExtension(e.Logo);
                profileImage = Server.MapPath(@"\\Document\\" + e.Id + "\\" + fileName + "_81x81" + extension);
                sb.Append("<img  src='" + profileImage + "' />");
            }
            else
            {
                sb.Append("<img width='81' src='" + Server.MapPath(@"\\Content\\images\\thumb.jpg") + "' />");
            }
            sb.Append("</td>");

            sb.Append("<td width='20%'>");
            sb.Append("<b>" + e.FirstName + " " + e.LastName + "<b>");
            sb.Append("</td>");

            sb.Append("<td width='75%'>Email ID : " + e.Email + "</td>");
            sb.Append("</tr>");

            sb.Append("<tr>");

            sb.Append("<td rowspan='5' width='20%'>");
            sb.Append("Work Exp.: ");
            sb.Append("<b>");
            if (e.WorkExYear != null)
            {
                sb.Append(e.WorkExYear + " year");
            }
            if (e.WorkExMonth != null)
            {
                sb.Append(e.WorkExMonth + " month");
            }

            sb.Append("</b>");
            sb.Append("</td>");

            sb.Append("<td width='75%'>");
            sb.Append("Phone:" + e.Phone);
            sb.Append("</td>");

            sb.Append("</tr>");

            sb.Append("<tr>");

            sb.Append("<td width='75%'>");
            if (e.DateOfBirth != null)
            {
                sb.Append("Date of Birth:" + e.DateOfBirth.Value.ToString("dd-MM-yyyy"));
            }
            sb.Append("</td>");

            sb.Append("</tr>");
            sb.Append("<tr>");

            sb.Append("<td width='75%'>");
            sb.Append("Gender:" + e.Gender);
            sb.Append("</td>");

            sb.Append("</tr>");
            sb.Append("<tr>");

            sb.Append("<td width='75%'>");
            sb.Append("Nationality:" + e.Nationality);
            sb.Append("</td>");

            sb.Append("</tr>");

            sb.Append("<tr>");

            sb.Append("<td width='75%'>");
            sb.Append("Address:" + e.Address);
            sb.Append("</td>");

            sb.Append("</tr>");

            #endregion

            sb.Append("<tr colspan='3'>");
            sb.Append("<td><u>Objective</u></td>");
            sb.Append("</tr>");

            sb.Append("<tr colspan='3'>");
            sb.Append("<td>" + e.Objective + "</td>");
            sb.Append("</tr>");
            if (e.EmployeeSkills != null)
            {
                if (e.EmployeeSkills.Count > 0)
                {
                    sb.Append("<tr colspan='3'>");
                    sb.Append("<td><u>Skill and Expertise</u></td>");
                    sb.Append("</tr>");


                    sb.Append("<tr colspan='3'>");
                    sb.Append("<td><ul>");

                    foreach (var skill in e.EmployeeSkills)
                    {
                        sb.Append("<li>" + skill.Skill + "</li>");
                    }

                    sb.Append("</ul></td>");
                    sb.Append("</tr>");
                }
            }


            if (e.Employeements != null)
            {
                if (e.Employeements.Count > 0)
                {
                    sb.Append("<tr colspan='3'>");
                    sb.Append("<td><u>Employment</u></td>");
                    sb.Append("</tr>");

                    sb.Append("<tr colspan='3'>");
                    sb.Append("<td><ul>");

                    foreach (var emp in e.Employeements)
                    {
                        sb.Append("<li><b>" + emp.Designation + " (" + emp.CompanyName + ")</b> [" + emp.FromYear + " - " + emp.ToYear + "]<br/>" + emp.Description + "<br /></li>");
                    }

                    sb.Append("</ul></td>");
                    sb.Append("</tr>");
                }
            }

            if (e.Educations != null)
            {
                if (e.Educations.Count > 0)
                {
                    sb.Append("<tr colspan='3'>");
                    sb.Append("<td><u>Education</u></td>");
                    sb.Append("</tr>");

                    sb.Append("<tr colspan='3'>");
                    sb.Append("<td><ul>");

                    foreach (var edu in e.Educations)
                    {
                        sb.Append("<li><b>" + edu.Degree + "</b> (" + edu.FromYear + " - " + edu.ToYear + ") <br/>" + edu.Description + "<br /></li>");
                    }

                    sb.Append("</ul></td>");
                    sb.Append("</tr>");
                }
            }

            if (e.EmployeeProjects != null)
            {
                if (e.EmployeeProjects.Count > 0)
                {
                    sb.Append("<tr colspan='3'>");
                    sb.Append("<td><u>Projects</u></td>");
                    sb.Append("</tr>");

                    sb.Append("<tr colspan='3'>");
                    sb.Append("<td><ul>");

                    foreach (var proj in e.EmployeeProjects)
                    {
                        sb.Append("<li><b>" + proj.Title + "</b> <br/>" + proj.Description + "<br /></li>");
                    }

                    sb.Append("</ul></td>");
                    sb.Append("</tr>");
                }
            }

            if (e.EmployeeAchivements != null)
            {
                if (e.EmployeeAchivements.Count > 0)
                {
                    sb.Append("<tr colspan='3'>");
                    sb.Append("<td><u>Achievements / Awards / Honors</u></td>");
                    sb.Append("</tr>");

                    sb.Append("<tr colspan='3'>");
                    sb.Append("<td><ul>");

                    foreach (var ach in e.EmployeeAchivements)
                    {
                        sb.Append("<li>" + ach.Achievement + "</li>");
                    }

                    sb.Append("</ul></td>");
                    sb.Append("</tr>");
                }
            }

            if (e.EmployeeSeminars != null)
            {
                if (e.EmployeeSeminars.Count > 0)
                {
                    sb.Append("<tr colspan='3'>");
                    sb.Append("<td><u>Workshop / Conference / Seminar / Symposium</u></td>");
                    sb.Append("</tr>");

                    sb.Append("<tr colspan='3'>");
                    sb.Append("<td><ul>");

                    foreach (var sem in e.EmployeeSeminars)
                    {
                        sb.Append("<li>" + sem.Seminar + "</li>");
                    }

                    sb.Append("</ul></td>");
                    sb.Append("</tr>");
                }
            }

            if (e.EmployeePublications != null)
            {
                if (e.EmployeePublications.Count > 0)
                {
                    sb.Append("<tr colspan='3'>");
                    sb.Append("<td><u>Publications</u></td>");
                    sb.Append("</tr>");

                    sb.Append("<tr colspan='3'>");
                    sb.Append("<td><ul>");

                    foreach (var pub in e.EmployeePublications)
                    {
                        sb.Append("<li>" + pub.Publication + "</li>");
                    }

                    sb.Append("</ul></td>");
                    sb.Append("</tr>");
                }
            }

            if (e.EmployeeReferences != null)
            {
                if (e.EmployeeReferences.Count > 0)
                {
                    sb.Append("<tr colspan='3'>");
                    sb.Append("<td><u>References</u></td>");
                    sb.Append("</tr>");

                    sb.Append("<tr colspan='3'>");
                    sb.Append("<td><ul>");

                    foreach (var refr in e.EmployeeReferences)
                    {
                        sb.Append("<li><b>" + refr.Reference + "</b> (" + refr.Designation + ")<br />");
                        if (refr.Email != null)
                        {
                            sb.Append("<b>Email ID : </b>" + refr.Email + "<br />");
                        }
                        if (refr.Phone != null)
                        {
                            sb.Append("<b>Phone : </b>" + refr.Phone + "<br />");
                        }
                        sb.Append("</li>");
                    }

                    sb.Append("</ul></td>");
                    sb.Append("</tr>");
                }
            }
            
            sb.Append("</table>");

            return sb.ToString();
        }

        protected bool HTMLToPdf(string HTML)
        {
            bool flag = false;
            StringReader reader = new StringReader(HTML.ToString());
            int UseID = Convert.ToInt32(Session["userid"]);
            iTextSharp.text.Rectangle rect = PageSize.LETTER;
            Document document = new Document(PageSize.A4, 20, 20, 20, 20);
            HTMLWorker parser = new HTMLWorker(document);

            string PDF_FileName = Server.MapPath("/Document/" + UseID + "/pdf/Resume.pdf");
            Directory.CreateDirectory(Path.GetDirectoryName(PDF_FileName));
            PdfWriter.GetInstance(document, new FileStream(PDF_FileName, FileMode.Create));
            document.Open();

            try
            {
                parser.StartDocument();
                parser.Parse(reader);
                parser.EndDocument();
                parser.Close();
                flag = true;
            }
            catch (Exception ex)
            {
                Paragraph paragraph = new Paragraph("Error!" + ex.Message);
                Chunk text = paragraph.Chunks[0] as Chunk;
                if (text != null)
                {
                    text.Font.Color = BaseColor.RED;
                }
                document.Add(paragraph);
            }
            finally
            {
                document.Close();
               // DownLoadPdf(PDF_FileName);

            }

            return flag;
        }

        private void DownLoadPdf(string s)
        {
            WebClient client = new WebClient();
            Byte[] buffer = client.DownloadData(s);
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-length", buffer.Length.ToString());
            Response.BinaryWrite(buffer);
        }

        public void BindMonths()
        {
            var month = new List<SelectListItem>();

            month.Add(new SelectListItem {Text="Jan",Value="JAN" });
            month.Add(new SelectListItem { Text = "Feb", Value = "FEB" });
            month.Add(new SelectListItem { Text = "Mar", Value = "MAR" });
            month.Add(new SelectListItem { Text = "Apr", Value = "APR" });
            month.Add(new SelectListItem { Text = "May", Value = "MAY" });
            month.Add(new SelectListItem { Text = "Jun", Value = "JUN" });
            month.Add(new SelectListItem { Text = "Jul", Value = "JUL" });
            month.Add(new SelectListItem { Text = "Aug", Value = "AUG" });
            month.Add(new SelectListItem { Text = "Sep", Value = "SEP" });
            month.Add(new SelectListItem { Text = "Oct", Value = "OCT" });
            month.Add(new SelectListItem { Text = "Nov", Value = "NOV" });
            month.Add(new SelectListItem { Text = "Dec", Value = "DEC" });

            ViewBag.Months = month;
        }
    }
}
