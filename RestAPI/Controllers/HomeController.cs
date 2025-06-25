using Microsoft.AspNetCore.Mvc;
using RestAPI.Models;
using RestAPI.Services.Interfaces;
using System.Diagnostics;

namespace RestAPI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
