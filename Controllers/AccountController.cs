using Calcular.CoreApi.Models;
using Calcular.CoreApi.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace MovieAngularJSApp.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginViewModel login, string returnUrl = "/home")
        {
            var result = await signInManager.PasswordSignInAsync(login.UserName, login.Password, false, false);
            if (result.Succeeded)
            {
                return Redirect(returnUrl);
            }
            ModelState.AddModelError("", "Invalid username or password.");
            return View();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User { UserName = model.UserName, Email = model.Email, BirthDate = model.BirthDate };
                var result = await userManager.CreateAsync(user, "password123");
                if (result.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    var error = result.Errors.FirstOrDefault();
                    ModelState.AddModelError(error.Code, error.Description);
                    return BadRequest(result.Errors.FirstOrDefault().Description);
                }
            }

            return BadRequest(model);
        }

        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] UpdatePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(HttpContext.User);
                var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    var error = result.Errors.FirstOrDefault();
                    ModelState.AddModelError(error.Code, error.Description);
                    return BadRequest(result.Errors.FirstOrDefault().Description);
                }
            }

            return BadRequest(model);
        }

        [HttpGet("logout")]
        public async Task<IActionResult> SignOut()
        {
            await signInManager.SignOutAsync();
            return Ok();
        }

        [HttpGet("loggeduser")]
        public IActionResult GetLoggedUser()
        {
            return Ok(userManager.GetUserAsync(HttpContext.User));
        }
    }
}