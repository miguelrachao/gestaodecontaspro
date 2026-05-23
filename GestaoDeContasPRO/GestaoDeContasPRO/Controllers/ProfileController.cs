using GestaoDeContasPRO.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoDeContasPRO.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ProfileRepository _profileRepo;
        public ProfileController(ProfileRepository profileRepo)
        {
            _profileRepo = profileRepo;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
