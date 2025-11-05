using GestaoDeContasPRO.Models;
using GestaoDeContasPRO.Repositories;
using GestaoDeContasPRO.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestaoDeContasPRO.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserRepository _userRepo;
        private readonly Helpers _helpers;

        public AccountController(UserRepository userRepo, Helpers helpers)
        {
            _userRepo = userRepo;
            _helpers = helpers;
        }


        [HttpGet]
        public IActionResult Login()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
                
        }


        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            try
            {
                bool error = false;

                if(user.Email != string.Empty && user.Email != null)
                {
                    if(user.OtpCode != 0 && user.OtpCode != null)
                    {
                        if (_userRepo.ValidateOtp(ref user, ref error))
                        {
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, user.Name),
                                new Claim("UserID", user.Id.ToString())
                            };


                            var claimsIdentity = new ClaimsIdentity(
                                claims, CookieAuthenticationDefaults.AuthenticationScheme);


                            var authProperties = new AuthenticationProperties
                            {
                                IsPersistent = true,
                                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(180)
                            };


                            await HttpContext.SignInAsync(
                                CookieAuthenticationDefaults.AuthenticationScheme,
                                new ClaimsPrincipal(claimsIdentity),
                                authProperties);

                            user.OtpExpiration = DateTime.Now;
                            _userRepo.UpdateOtp(user);

                            return Ok();
                        }
                        else
                        {
                            if (error)
                            {
                                return StatusCode(500);
                            }
                            else
                            {
                                return StatusCode(403);
                            }   
                        }
                    }
                    else
                    {
                        if(_userRepo.GetByEmail(ref user, ref error))
                        {
                            if (_userRepo.HasAlreadyOtp(user, ref error))
                            {
                                return Ok();
                            }
                            else
                            {
                                if (error)
                                {
                                    return StatusCode(500);
                                }
                                else
                                {
                                    Random random = new Random();
                                    user.OtpCode = random.Next(100000, 1000000);
                                    user.OtpExpiration = DateTime.Now.AddMinutes(5);

                                    if (_userRepo.UpdateOtp(user))
                                    {
                                        _helpers.SendEmail(user.Email, "Gestão de contas - Autenticação: " + user.OtpCode, "O seu código de autenticação: " + user.OtpCode);

                                        return Ok();
                                    }
                                    else
                                    {
                                        return StatusCode(500);
                                    }
                                }
                            }     
                        }
                        else
                        {
                            if (error)
                            {
                                return StatusCode(500);
                            }
                            else
                            {
                                return NotFound();
                            }
                                
                        }
                    }
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }

}
