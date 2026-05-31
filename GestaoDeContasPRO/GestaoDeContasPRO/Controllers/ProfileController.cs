using GestaoDeContasPRO.Models;
using GestaoDeContasPRO.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;

namespace GestaoDeContasPRO.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ProfileRepository _profileRepo;
        private readonly CategoryRepository _categoryRepo;
        public ProfileController(ProfileRepository profileRepo, CategoryRepository categoryRepo)
        {
            _profileRepo = profileRepo;
            _categoryRepo = categoryRepo;
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(Profile profile)
        {
            try
            {
                profile.UserId = int.Parse(User.FindFirst("UserID")?.Value ?? "0");
                profile.Active = true;

                if (profile.UserId != 0)
                {
                    if (_profileRepo.PostProfile(ref profile))
                    {
                        return Ok(profile);
                    }
                    else
                    {
                        return StatusCode(500);
                    }
                }
                else
                {
                    return StatusCode(500);
                }
            }
            catch { return StatusCode(500); }
            
        }

        public IActionResult Details(int id)
        {
            Profile profile = new Profile();
            profile.Id = id;
            profile.UserId = int.Parse(User.FindFirst("UserID")?.Value ?? "0");

            bool error = false;
            _profileRepo.GetProfile(ref profile, ref error);

            if (profile.Id != 0 && error != true)
            {
                List<Category> categories = new List<Category>();
                _categoryRepo.GetProfileCategories(ref categories, profile.Id, ref error);

                if (!error)
                {
                    dynamic model = new ExpandoObject();
                    model.profile = profile;
                    model.categories = categories;

                    return View(model);
                }
                else
                {
                    return RedirectToAction("Error", "Home");
                }      
            }
            else
            {
                return RedirectToAction("Error", "Home");
            } 
        }

        [HttpPut]
        public IActionResult Details(Profile profile)
        {
            profile.UserId = int.Parse(User.FindFirst("UserID")?.Value ?? "0");

            if (_profileRepo.UpdateProfile(profile))
            {
                return Ok();
            }
            else
            {
                return StatusCode(500);
            }
        }
    }
}
