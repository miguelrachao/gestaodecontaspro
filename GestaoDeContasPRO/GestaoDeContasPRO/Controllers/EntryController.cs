using GestaoDeContasPRO.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoDeContasPRO.Controllers
{
    [Authorize]
    public class EntryController : Controller
    {
        private readonly EntryRepository _entryRepo;

        public EntryController(EntryRepository entryRepo)
        {
            _entryRepo = entryRepo;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
