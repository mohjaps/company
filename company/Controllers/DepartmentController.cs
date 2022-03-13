using company.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace company.Controllers
{
    [Authorize]
    public class DepartmentController : Controller
    {
        private readonly AppDatabase db;

        public DepartmentController(AppDatabase db)
        {
            this.db = db;
        }
        // GET: DepartmentController
        public ActionResult Index()
        {
            var data = db.Department.ToList();
            return View(data);
        }

        // GET: DepartmentController/Details/5
        public ActionResult Details(int id)
        {

            var dep = db.Department.Where(d => d.Id == id).FirstOrDefault();
            return View(dep);
        }

        // GET: DepartmentController/Create
        public ActionResult Create()
        {
            return View(new Department());
        }

        // POST: DepartmentController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([FromForm] Department formDep)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Department.Add(formDep);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                TempData["Alert"] = "Create Done...";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DepartmentController/Edit/5
        public ActionResult Edit(int id)
        {
            var dep = db.Department.Where(d => d.Id == id).FirstOrDefault();
            return View(dep);
        }

        // POST: DepartmentController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [FromForm] Department formDep)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    formDep.Id = id;
                    db.Department.Update(formDep);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewData["Alert"] = "Edit Done...";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DepartmentController/Delete/5
        public ActionResult Delete(int id)
        {
            var dep = db.Department.Where(d => d.Id == id).FirstOrDefault();
            return View(dep);
        }

        // POST: DepartmentController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Department formDep)
        {
            try
            {
                if (formDep != null)
                {
                    db.Department.Remove(formDep);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                TempData["Alert"] = "Delete Done...";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
