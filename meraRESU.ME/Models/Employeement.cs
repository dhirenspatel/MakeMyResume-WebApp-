using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace meraRESU.ME.Models
{
    public class Employeement
    {
        public int Id { get; set; }


        public string CompanyName { get; set; }

        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }

        public string Designation { get; set; }
        

        public int FromYear { get; set; }

        public int ToYear { get; set; }

        public string FromMonth { get; set; }
        public string ToMonth { get; set; }

        public string Description { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        userDbContext db = new userDbContext();

        internal void addEmployeement(string cname, string des1, int emId, int fromyear, int toyear, string des2)
        {           
            this.CompanyName = cname;
            this.Designation = des1;
            this.EmployeeId = emId;
            this.FromYear = fromyear;
            this.ToYear = toyear;
            this.Description = des2;
            this.Created = DateTime.Now;
            this.Updated = DateTime.Now;
            db.EmployeementDB.Add(this);
            db.SaveChanges();
            //s = bindEmployeementData(em.Id);
        }
    }
}