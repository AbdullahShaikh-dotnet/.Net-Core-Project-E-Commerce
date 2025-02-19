using ECom.DataAccess.Repository.IRepository;
using ECom.Models;
using ECom.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _UnitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Company> Companys = _UnitOfWork.Company.GetAll()
                .Where(data => !data.IsDeleted).ToList();
            return View(Companys);
        }

        public IActionResult Upsert(int? id)
        {
            if (id == 0 || id is null)
                return View(new Company());

            var CompanyObj = _UnitOfWork.Company.Get(data => data.ID == id);
            return View(CompanyObj);
        }

        [HttpPost]
        public IActionResult Upsert(Company CompanyObj)
        {
            if (ModelState.IsValid)
            {
                if (CompanyObj.ID == 0)
                    _UnitOfWork.Company.Add(CompanyObj);
                else
                    _UnitOfWork.Company.Update(CompanyObj);

                _UnitOfWork.Save();
                TempData["success"] = "Company Created Successfully";
                return RedirectToAction("Index");
            }
            else
            {
                return View(CompanyObj);
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> CompaniesList = _UnitOfWork.Company.GetAll()
                .Where(data => !data.IsDeleted).ToList();
            return Json(new { data = CompaniesList });
        }


        public IActionResult Delete(int? id)
        {
            var CompanyToBeDeleted = _UnitOfWork.Company.Get(d => d.ID == id);
            if (CompanyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error While Deletig" });
            }

            CompanyToBeDeleted.IsDeleted = true;
            CompanyToBeDeleted.DeletedAt = DateTime.Now;
            _UnitOfWork.Company.Update(CompanyToBeDeleted);
            _UnitOfWork.Save();

            return Json(new { success = true, message = "Deleted Successfully" });
        }
    }
}
