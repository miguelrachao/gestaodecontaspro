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
        private readonly DashboardRepository _dashRepo;

        public HomeController(ProfileRepository profileRepo, DashboardRepository dashRepo)
        {
            _profileRepo = profileRepo;
            _dashRepo = dashRepo;
        }

        [Authorize]
        public IActionResult Index(int? profileId, DateTime? startDate, DateTime? endDate)
        {
            bool error = false;
            dynamic model = new ExpandoObject();

            User currentUser = new User();
            currentUser.Id = int.Parse(User.FindFirst("UserID")?.Value ?? "0");

            if (!error)
            {
                List<Profile> profiles = new List<Profile>();
                _profileRepo.GetUserProfiles(ref profiles, currentUser.Id, active:true, ref error);

                if(profileId == null)
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

                List<Entry> entriesAmount = new List<Entry>();
                _dashRepo.GetEntriesAmountByCategory(ref entriesAmount, profileId ?? 0, startDate, endDate, currentUser.Id, ref error);

                List<YearBalance> yearBalances = new List<YearBalance>();
                _dashRepo.GetYearBalance(ref yearBalances, profileId ?? 0, startDate?.ToString("yyyy") ?? DateTime.UtcNow.Year.ToString(), currentUser.Id, ref error);

                double credit = entriesAmount
                   .Where(e => e.Category.Type == ActionType.CREDIT)
                   .Sum(e => e.Amount);

                double debit = entriesAmount
                    .Where(e => e.Category.Type == ActionType.DEBIT)
                    .Sum(e => e.Amount);

                double balance = credit - debit;

                model.profiles = profiles;
                model.profileId = profileId;
                model.startDate = startDate?.ToString("yyyy-MM-dd");
                model.endDate = endDate?.ToString("yyyy-MM-dd");
                model.entriesAmount = entriesAmount;
                model.credit = credit;
                model.debit = debit;
                model.balance = balance;
                model.yearBalances = yearBalances;
            }

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
