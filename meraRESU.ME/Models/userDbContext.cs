using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace meraRESU.ME.Models
{
    public class userDbContext:DbContext
    {
        public DbSet<Employee> EmployeeDB { get; set; }

        public DbSet<Designation> DesignationDB { get; set; }

        public DbSet<Education> EducationDB { get; set; }

        public DbSet<EmployeeLanguage> EmployeeLanguageDB { get; set; }

        public DbSet<Employeement> EmployeementDB { get; set; }

        public DbSet<EmployeePortfolio> EmployeePortfolioDB { get; set; }

        public DbSet<EmployeeSkill> EmployeeSkillDB { get; set; }        
        
        public DbSet<Skill> SkillDB { get; set; }

        public DbSet<Language> LanguageDB { get; set; }

        public DbSet<EmployeeAchievement> EmployeeAchivementDB { get; set; }

        public DbSet<EmployeeReference> EmployeeReferenceDB { get; set; }

        public DbSet<EmployeeProject> EmployeeProjectDB { get; set; }

        public DbSet<EmployeePublication> EmployeePublicationDB { get; set; }

        public DbSet<EmployeeSeminar> EmployeeSeminarDB { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }       
    }
}