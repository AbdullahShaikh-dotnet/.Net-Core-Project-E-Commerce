using System.Diagnostics;
using System.Security.Claims;
using ECom.DataAccess.Repository.IRepository;
using ECom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
