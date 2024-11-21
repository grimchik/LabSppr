using Bar.Domain.Models;
using Bar.UI.Services.AuthService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
namespace Lab1_sppr.Controllers
{
    public class AccountController : Controller
    {
        /// <summary>
        /// Отображает форму регистрации пользователя
        /// </summary>
        /// <returns>Представление с формой</returns>
        public IActionResult Register()
        {
            return View(new RegisterUserViewModel());
        }

        /// <summary>
        /// Регистрирует пользователя на сервере аутентификации
        /// </summary>
        /// <param name="user">Модель данных пользователя</param>
        /// <param name="authService">Сервис для регистрации</param>
        /// <returns>Перенаправление или сообщение об ошибке</returns>
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Register(
            RegisterUserViewModel user,
            [FromServices] IAuthService authService)
        {
            if (ModelState.IsValid)
            {
                if (user == null)
                {
                    return BadRequest("Invalid user data.");
                }

                var result = await authService.RegisterUserAsync(user.Email, user.Password, user.Avatar);

                if (result.Result)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage);
                    return View(user);
                }
            }

            return View(user);
        }
        public async Task Login()
        {
            await HttpContext.ChallengeAsync(
            OpenIdConnectDefaults.AuthenticationScheme,
            new AuthenticationProperties
            {
                RedirectUri = Url.Action("Index",
            "Home")
            });
        }
        [HttpPost]
        public async Task Logout()
        {
            await
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, 
                new AuthenticationProperties
            {
                RedirectUri = Url.Action("Index","Home")
            });
        }
    }
}
