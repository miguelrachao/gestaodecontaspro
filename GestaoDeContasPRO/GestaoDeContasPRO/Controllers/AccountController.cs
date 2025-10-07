using GestaoDeContasPRO.Models;
using GestaoDeContasPRO.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestaoDeContasPRO.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserRepository _userRepo;

        public AccountController(UserRepository userRepo)
        {
            _userRepo = userRepo;
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {

            try
            {
                
                if (user.Email == "miguel_rachao.96@hotmail.com" && user.OtpCode == 123456)
                {


                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, "Miguel Rachão"),
                        new Claim("UserID", "1")
                    };


                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);


                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                    };


                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);


                    return Ok();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }

}
