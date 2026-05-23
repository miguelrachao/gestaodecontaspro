using GestaoDeContasPRO.Models;
using GestaoDeContasPRO.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Dynamic;
using System.Security.Claims;

namespace GestaoDeContasPRO.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProfileRepository _profileRepo;

        public HomeController(ProfileRepository profileRepo)
        {
            _profileRepo = profileRepo;
        }

        [Authorize]
        public IActionResult Index()
        {
            bool error = false;
            dynamic model = new ExpandoObject();

            User currentUser = new User();
            currentUser.Id = int.Parse(User.FindFirst("UserID")?.Value ?? "0");

            Profile favoriteProfile = new Profile();
            favoriteProfile.UserId = currentUser.Id;

            // GET USER FAVORITE PROFILE
            if (_profileRepo.GetUserFavoriteProfile(ref favoriteProfile, ref error)){

                // OBTEM CATEGORIAS DESSE PROFILE
                // OBTEM REGISTOS DESSE PROFILE CONSOANTE MES SELECIONADO?
            }
            else
            {
                if (!error)
                {
                    // GET PROFILES TO CHOOSE FAVORITE PROFILE
                    List<Profile> profiles = new List<Profile>();
                    _profileRepo.GetUserAllProfiles(ref profiles, currentUser.Id, ref error);

                    model.profiles = profiles;
                }
            }

            model.favoriteProfile = favoriteProfile;
            model.error = error;

            return View(model);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
