using GestaoDeContasPRO.Models;
using GestaoDeContasPRO.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;

namespace GestaoDeContasPRO.Controllers
{
    [Authorize]
    public class EntryController : Controller
    {
        private readonly ProfileRepository _profileRepo;
        private readonly CategoryRepository _categoryRepo;
        private readonly EntryRepository _entryRepo;

        public EntryController(ProfileRepository profileRepo, CategoryRepository categoryRepo, EntryRepository entryRepo)
        {
            _profileRepo = profileRepo;
            _categoryRepo = categoryRepo;
            _entryRepo = entryRepo;
        }


        public IActionResult Index(int profileId, int categoryId, DateTime? startDate, DateTime? endDate)
        {
            bool error = false;
            dynamic model = new ExpandoObject();

            User currentUser = new User();
            currentUser.Id = int.Parse(User.FindFirst("UserID")?.Value ?? "0");

            List<Profile> profiles = new List<Profile>();
            _profileRepo.GetUserProfiles(ref profiles, currentUser.Id, active: true, ref error);

            if (profileId == 0)
            {
                profileId = profiles.First().Id;
            }

            // DATE VALIDATIONS ------------------------------
            // DEFAULT VALUES = CURRENT MONTH
            DateTime today = DateTime.Today;

            DateTime monthStart = new(today.Year, today.Month, 1);
            DateTime monthEnd = monthStart.AddMonths(1).AddTicks(-1);

            startDate ??= monthStart;
            endDate ??= monthEnd;
            // DATE VALIDATIONS ------------------------------

            List<Category> categories = new List<Category>();
            _categoryRepo.GetProfileCategories(ref categories, profileId, ref error);

            List<Entry> entries = new List<Entry>();
            _entryRepo.GetProfileEntries(ref entries, profileId, categoryId, startDate, endDate, currentUser.Id, ref error);

            model.profiles = profiles;
            model.profileId = profileId;
            model.categoryId = categoryId;
            model.startDate = startDate?.ToString("yyyy-MM-dd");
            model.endDate = endDate?.ToString("yyyy-MM-dd");
            model.categories = categories;
            model.entries = entries;

            if (error)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                return View(model);
            }

        }
    }
}
