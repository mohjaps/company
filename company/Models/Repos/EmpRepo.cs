using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace company.Models.Repos
{
    public class EmpRepo : IRepo<Employee>
    {
        private readonly AppDatabase db;
        private readonly IWebHostEnvironment env;

        public EmpRepo(AppDatabase db, IWebHostEnvironment env)
        {
            this.db = db;
            this.env = env;
        }
        public bool Add(Employee formEmp)
        {
            if (formEmp.Department != null)
            {
                db.Employee.Add(formEmp);
                db.SaveChanges();
                return true;
            }
            return false;
        }
        public bool Add(Employee formEmp, List<IFormFile> files)
        {
            formEmp.Department = db.Department.Where(d => d.Id == formEmp.DepId).FirstOrDefault();
            if (formEmp.Department != null)
            {
                if (files.Count > 0)
                {
                    var formFile = files[0];
                    var filePath = this.env.WebRootPath + "/img/" + formFile.FileName;
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        formFile.CopyTo(stream);
                    }
                    formEmp.Image_Path = "/img/" + formFile.FileName;
                }

                formEmp.Department = db.Department.Where(d => d.Id == formEmp.DepId).FirstOrDefault();
                if (formEmp.Department != null)
                {
                    db.Employee.Add(formEmp);
                    db.SaveChanges();
                    return true;
                }
            }
            return false;
        }
        public bool Delete(int id)
        {
            if (Find(id) != null)
            {
                db.Employee.Remove(Find(id));
                db.SaveChanges();
                return true;
            }
            return false;
        }
        public Employee Find(int id)
        {
            var emp = db.Employee.Where(e => e.Id == id).FirstOrDefault();
            return emp;
        }
        public List<Employee> List(int pageNumber, int pageSize)
        {
            var emps = db.Employee.AsNoTracking().Skip(pageNumber * pageSize - pageSize).Take(pageSize).ToList();
            return emps;
        }
        public List<Employee> List()
        {
            var emps = db.Employee.ToList();
            return emps;
        }
        public bool Update(Employee entity)
        {
            if (IsExist(entity.Id))
            {
                db.Employee.Update(entity);
                db.SaveChanges();
                return true;
            }
            return false;
        }
        public bool Update(Employee formEmp, List<IFormFile> files)
        {
            if (IsExist(formEmp.Id))
            {
                if (files.Count > 0)
                {
                    var formFile = files[0];
                    var filePath = this.env.WebRootPath + "/img/" + formFile.FileName;
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        formFile.CopyTo(stream);
                    }
                    formEmp.Image_Path = "/img/" + formFile.FileName;
                }
                formEmp.Department = db.Department.Where(d => d.Id == formEmp.DepId).FirstOrDefault();
                db.Employee.Update(formEmp);
                db.SaveChanges();
                return true;
            }
            return false;
        }
        public bool IsExist(int id)
        {
            return Find(id) != null;
        }
    }
}
