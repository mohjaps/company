using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace company.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Address { get; set; }
        public long Tel { get; set; }
        public double Salary { get; set; }
        public DateTime Hire_Date { get; set; }
        public string Image_Path { get; set; }


        [ForeignKey("Department")]
        [Display(Name = "Department")]
        public int DepId { get; set; }
        public Department Department { get; set; }
    }
}
