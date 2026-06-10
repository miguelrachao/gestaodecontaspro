using GestaoDeContasPRO.Models;
using GestaoDeContasPRO.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoDeContasPRO.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly CategoryRepository _categoryRepo;
        private readonly ProfileRepository _profileRepo;

        public CategoryController(CategoryRepository categoryRepository, ProfileRepository profileRepo)
        {
            _categoryRepo = categoryRepository;
            _profileRepo = profileRepo;
        }

        [HttpPost]
        public IActionResult Add(Category category)
        {
            //VALIDATE PROFILE PERMISSION
            Profile profile = new Profile();
            profile.UserId = int.Parse(User.FindFirst("UserID")?.Value ?? "0");
            profile.Id = category.ProfileId;
            bool error = false;
            _profileRepo.GetProfile(ref profile, ref error);

            if (profile.Id != 0 && !error)
            {
                category.UserId = profile.UserId;
                category.Active = true;

                if (_categoryRepo.PostCategory(ref category))
                {
                    return Ok(category);
                }
                else
                {
                    return StatusCode(500);
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut]
        public IActionResult Edit(Category category)
        {

            //VALIDATE PROFILE PERMISSION
            Profile profile = new Profile();
            profile.UserId = int.Parse(User.FindFirst("UserID")?.Value ?? "0");
            profile.Id = category.ProfileId;
            bool error = false;
            _profileRepo.GetProfile(ref profile, ref error);


            if (profile.Id != 0 && !error)
            {
                if (_categoryRepo.UpdateCategory(category))
                {
                    return Ok(category);
                }
                else
                {
                    return StatusCode(500);
                }
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
