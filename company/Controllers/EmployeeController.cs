using cloudscribe.Pagination.Models;
using company.Models;
using company.Models.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace company.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IRepo<Employee> emps;
        private readonly AppDatabase db;

        public EmployeeController(IRepo<Employee> emps, AppDatabase db)
        {
            this.emps = emps;
            this.db = db;
        }
        // GET: EmployeeController
        public IActionResult Index(string searchText, string sortText, int pageNumber = 1, int pageSize = 3)
        {
            ViewData["SearchText"] = searchText;
            

            ViewData["sortFName"] = (string.IsNullOrEmpty(sortText) || sortText == "FirstName_Asc") ? "FirstName_Desc" : "FirstName_Asc";
            ViewData["sortSalary"] = sortText == "Salary_Asc" ? "Salary_Desc" : "Salary_Asc";
            ViewData["sortHireDate"] = sortText == "HireDate_Asc" ? "HireDate_Desc" : "HireDate_Asc";

            var emps = this.emps.List(pageNumber, pageSize).AsQueryable();
            if (!string.IsNullOrEmpty(searchText))
            {
                emps = db.Employee.Where
                    (e =>
                        e.First_Name.Contains(searchText) ||
                        e.Last_Name.Contains(searchText)
                    ).AsQueryable();
            }


            if (sortText == "FirstName_Asc")
            {
                emps = emps.OrderBy(s => s.First_Name);
            }
            else if (sortText == "FirstName_Desc")
            {
                emps = emps.OrderByDescending(s => s.First_Name);
            }

            else if (sortText == "Salary_Asc")
            {
                emps = emps.OrderBy(s => s.Salary);
            }
            else if (sortText == "Salary_Desc")
            {
                emps = emps.OrderByDescending(s => s.Salary);
            }

            else if (sortText == "HireDate_Asc")
            {
                emps = emps.OrderBy(s => s.Hire_Date);
            }
            else if (sortText == "HireDate_Desc")
            {
                emps = emps.OrderByDescending(s => s.Hire_Date);
            }


            var result = new PagedResult<Employee>()
            {
                Data = emps.ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = this.emps.List().Count()

            };
            return View(result);
        }

        // GET: EmployeeController/Details/5
        public ActionResult Details(int id)
        {
            return View(emps.Find(id));
        }

        // GET: EmployeeController/Create
        public ActionResult Create()
        {
            return View(new Employee());
        }

        // POST: EmployeeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([FromForm]Employee formEmp, List<IFormFile> files)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (emps.Add(formEmp, files))
                    {
                        return RedirectToAction("Index");
                    }
                }
                return View(formEmp);
             }
            catch
            {
                return View(formEmp);
            }
}

        // GET: EmployeeController/Edit/5
        public ActionResult Edit(int id)
        {
            var emp = emps.Find(id);
            ViewData["ImagePath"] = emp.Image_Path;
            return View(emp);
        }

        // POST: EmployeeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Employee formEmp, List<IFormFile> files)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (emps.Update(formEmp, files))
                    {
                        return RedirectToAction("Index");
                    }
                }
                return View();
            }
            catch(Exception err)
            {
                var error = err.Message;
                return View();
            }
        }

        // GET: EmployeeController/Delete/5
        public ActionResult Delete(int id)
        {
            return View(emps.Find(id));
        }

        // POST: EmployeeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Employee formEmp)
        {
            try
            {
                if (formEmp != null)
                {
                    if (emps.Delete(formEmp.Id))
                    {
                        return RedirectToAction("Index");
                    }
                }
                return View(formEmp);
            }
            catch
            {
                return View();
            }
        }
    }
}

//using company.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;

//namespace company.Controllers
//{
//    [Authorize]
//    public class EmployeeController : Controller
//    {
//        private readonly AppDatabase db;
//        private readonly IWebHostEnvironment env;

//        public EmployeeController(AppDatabase db, IWebHostEnvironment env)
//        {
//            this.db = db;
//            this.env = env;
//        }
//        // GET: EmployeeController
//        public ActionResult Index()
//        {
//            var emp = db.Employee.ToList();
//            return View(emp);
//        }

//        // GET: EmployeeController/Details/5
//        public ActionResult Details(int id)
//        {
//            var emp = db.Employee.Where(e => e.Id == id).FirstOrDefault();
//            return View(emp);
//        }

//        // GET: EmployeeController/Create
//        public ActionResult Create()
//        {
//            return View(new Employee());
//        }

//        // POST: EmployeeController/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create([FromForm] Employee formEmp, List<IFormFile> files)
//        {
//            try
//            {
//                if (ModelState.IsValid)
//                {
//                    if (files.Count > 0)
//                    {
//                        var formFile = files[0];
//                        var filePath = this.env.WebRootPath + "/img/" + formFile.FileName;
//                        using (var stream = new FileStream(filePath, FileMode.Create))
//                        {
//                            formFile.CopyTo(stream);
//                        }
//                        formEmp.Image_Path = "/img/" + formFile.FileName;
//                    }
//                    formEmp.Department = db.Department.Where(d => d.Id == formEmp.DepId).FirstOrDefault();

//                    if (formEmp.Department != null)
//                    {
//                        db.Employee.Add(formEmp);
//                        db.SaveChanges();
//                        return RedirectToAction("Index");
//                    }
//                }
//                else
//                {
//                    return View(formEmp);
//                }
//                return RedirectToAction(nameof(Index));
//            }
//            catch
//            {
//                return View(formEmp);
//            }
//        }

//        // GET: EmployeeController/Edit/5
//        public ActionResult Edit(int id)
//        {
//            var emp = db.Employee.Where(e => e.Id == id).FirstOrDefault();
//            return View(emp);
//        }

//        // POST: EmployeeController/Edit/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit(int id, Employee formEmp, List<IFormFile> files)
//        {
//            try
//            {
//                if (ModelState.IsValid)
//                {
//                    if (files.Count > 0)
//                    {
//                        var formFile = files[0];
//                        var filePath = this.env.WebRootPath + "/img/" + formFile.FileName;
//                        using (var stream = new FileStream(filePath, FileMode.Create))
//                        {
//                            formFile.CopyTo(stream);
//                        }
//                        formEmp.Image_Path = "/img/" + formFile.FileName;
//                    }
//                    db.Employee.Update(formEmp);
//                    db.SaveChanges();
//                    return RedirectToAction("Index");
//                }
//                else
//                {
//                    return View();
//                }
//                return RedirectToAction(nameof(Index));
//            }
//            catch
//            {
//                return View();
//            }
//        }

//        // GET: EmployeeController/Delete/5
//        public ActionResult Delete(int id)
//        {
//            var emp = db.Employee.Where(e => e.Id == id).FirstOrDefault();
//            return View(emp);
//        }

//        // POST: EmployeeController/Delete/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Delete(int id, Employee formEmp)
//        {
//            try
//            {
//                if (formEmp != null)
//                {
//                    db.Employee.Remove(formEmp);
//                    db.SaveChanges();

//                }
//                return RedirectToAction(nameof(Index));
//            }
//            catch
//            {
//                return View();
//            }
//        }
//    }
//}

