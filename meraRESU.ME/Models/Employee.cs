using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace meraRESU.ME.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public string Logo { get; set; }

        public string UserName { get; set; }

        public string Designation { get; set; }

        public int? WorkExYear { get; set; }

        public int? WorkExMonth { get; set; }

        public string Title { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string FacebookId { get; set; }

        public string Gender { get; set; }

        public string Password { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Website { get; set; }

        public string Aboutme { get; set; }

        public string FacebookLink	{ get; set; }

        public string TwitterLink	{ get; set; }

        public string GooglePlusLink	{ get; set; }

        public string LinkedInLink{ get; set; }

        public string SkypeId	{ get; set; }

        public string Objective { get; set; }

        public string Nationality { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public ICollection<Employeement> Employeements { get; set; }

        public ICollection<Education> Educations { get; set; }

        public ICollection<EmployeeSkill> EmployeeSkills { get; set; }

        public ICollection<EmployeeLanguage> EmployeeLanguages { get; set; }

        public ICollection<EmployeePortfolio> EmployeePortfolies { get; set; }

        public ICollection<EmployeeProject> EmployeeProjects { get; set; }

        public ICollection<EmployeeAchievement> EmployeeAchivements { get; set; }

        public ICollection<EmployeeSeminar> EmployeeSeminars { get; set; }

        public ICollection<EmployeePublication> EmployeePublications { get; set; }

        public ICollection<EmployeeReference> EmployeeReferences { get; set; }

        userDbContext db = new userDbContext();


        public int checkLogInOrSignUp(string email, string pwd)
        {
            IList<String> checkEmail = db.EmployeeDB.Where(x => x.Email == email.Trim()).Select(x=>x.Email).ToList();
            pwd = encryptPwd(pwd);
            if (checkEmail.Count() > 0)
            {
                Employee e = db.EmployeeDB.Where(x => x.Email == email.Trim()).FirstOrDefault();
                if (e != null)
                {
                    if (e.FacebookId != null)
                    {
                        e.Password = pwd;
                        db.SaveChanges();
                    }
                    else
                    {                        
                        if (e.Password == pwd)
                        {
                            return e.Id;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    return e.Id;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                this.Email = email;
                this.Password = pwd;
                this.Created = DateTime.Now;
                this.Updated = DateTime.Now;
                db.EmployeeDB.Add(this);
                db.SaveChanges();
                return this.Id;
            }            
        }

        public bool UpdateByAction(string action)
        {
            bool flag = false;

            if (this.Id != 0)
            {
                Employee emp = db.EmployeeDB.Where(x => x.Id == this.Id).FirstOrDefault();
                if (emp != null)
                {
                    if (action.Equals("photo"))
                    {

                        emp.Logo = this.Logo;
                        db.SaveChanges();
                        flag = true;
                    }
                }
            }
            return flag;
        }

        public string encryptPwd(string Password)
        {
            String npwd = Password;
            md5 md = new md5();
            md.pwd = npwd;

            return md.calculateMD5();
        }
    }
}