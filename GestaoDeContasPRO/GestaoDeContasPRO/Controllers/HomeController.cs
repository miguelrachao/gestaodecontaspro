using System.Diagnostics;
using GestaoDeContasPRO.Models;
using GestaoDeContasPRO.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoDeContasPRO.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserRepository _userRepo;

        public HomeController(ILogger<HomeController> logger, UserRepository userRepo)
        {
            _logger = logger;
            _userRepo = userRepo;
        }

     
        public IActionResult Index()
        {
            User user = new User();
            user.Email = "miguel_rachao.96@hotmail.com";
                
            _userRepo.GetByEmail(ref user);

            return View(user);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
