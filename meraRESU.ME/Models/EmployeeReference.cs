using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace meraRESU.ME.Models
{
    public class EmployeeReference
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }

        public string Reference { get; set; }

        public string Phone	{ get; set; }

        public string Email	{ get; set; }

        public string Designation { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }
    }
}