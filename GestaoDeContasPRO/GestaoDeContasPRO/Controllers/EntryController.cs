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

            double credit = entries
            .Where(e => e.Category.Type == ActionType.CREDIT)
            .Sum(e => e.Amount);

            double debit = entries
                .Where(e => e.Category.Type == ActionType.DEBIT)
                .Sum(e => e.Amount);

            double balance = credit - debit;

            model.profiles = profiles;
            model.profileId = profileId;
            model.categoryId = categoryId;
            model.startDate = startDate?.ToString("yyyy-MM-dd");
            model.endDate = endDate?.ToString("yyyy-MM-dd");
            model.categories = categories;
            model.entries = entries;
            model.credit = credit;
            model.debit = debit;
            model.balance = balance;

            if (error)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                return View(model);
            }

        }

        [HttpGet]
        public IActionResult Add(int profileId, int categoryId)
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

            List<Category> categories = new List<Category>();
            _categoryRepo.GetProfileCategories(ref categories, profileId, ref error);

            model.profileId = profileId;
            model.categoryId = categoryId;
            model.profiles = profiles;
            model.categories = categories;
            
            if (error)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult Add(Entry entry)
        {
            Profile profile = new Profile();
            profile.Id = entry.ProfileId;
            profile.UserId = int.Parse(User.FindFirst("UserID")?.Value ?? "0");

            bool error = false;

            _profileRepo.GetProfile(ref profile, ref error);

            if (profile.Id != 0 && error != true)
            {
                if (_entryRepo.PostEntry(entry))
                {
                    return Ok();
                }
            }

            return StatusCode(500);
            
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            bool error = false;
            dynamic model = new ExpandoObject();

            User currentUser = new User();
            currentUser.Id = int.Parse(User.FindFirst("UserID")?.Value ?? "0");


            Entry entry = new Entry();
            entry.Id = id;

            if(_entryRepo.GetEntry(ref entry, currentUser.Id))
            {
                List<Category> categories = new List<Category>();
                _categoryRepo.GetProfileCategories(ref categories, entry.ProfileId, ref error);

                model.categories = categories;
                model.entry = entry;
            }
            else
            {
                error = true;
            }

            
            if (error)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                return View(model);
            }
        }

        [HttpPut]
        public IActionResult Edit(Entry entry)
        {
            User currentUser = new User();
            currentUser.Id = int.Parse(User.FindFirst("UserID")?.Value ?? "0");

            Entry checkEntry = new Entry();
            checkEntry.Id = entry.Id;

            if (_entryRepo.GetEntry(ref checkEntry, currentUser.Id))
            {
                if (_entryRepo.UpdateEntry(entry))
                {
                    return Ok();
                }
            }

            return StatusCode(500);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            User currentUser = new User();
            currentUser.Id = int.Parse(User.FindFirst("UserID")?.Value ?? "0");

            Entry entry = new Entry();
            entry.Id = id;

            if (_entryRepo.GetEntry(ref entry, currentUser.Id))
            {
                if (_entryRepo.DeleteEntry(id))
                {
                    return Ok();
                }
            }

            return StatusCode(500);
        }
    }
}
