using Calcular.CoreApi.Models;
using Calcular.CoreApi.Models.Business;
using Calcular.CoreApi.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MovieAngularJSApp.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly ApplicationDbContext db;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext db)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.db = db;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginViewModel login)
        {
            var result = await signInManager.PasswordSignInAsync(login.UserName, login.Password, false, false);
            if (result.Succeeded)
            {
                return Ok();
            }
            ModelState.AddModelError("Error", "Invalid username or password.");
            return BadRequest(ModelState);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User { Name = model.Name, UserName = model.UserName, Email = model.Email, BirthDate = model.BirthDate.Date.AddHours(12) };
                var result =
                    await userManager.CreateAsync(user, "senha123");
                
                foreach (var role in model.Roles)
                    await userManager.AddToRoleAsync(user, db.Roles.Single(x => x.Id == role).Name);

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

        [HttpPut("register")]
        public async Task<IActionResult> Put([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.UserName);
                var userRoles = await userManager.GetRolesAsync(user);

                await userManager.RemoveFromRolesAsync(user, userRoles);
                foreach (var role in model.Roles)
                    await userManager.AddToRoleAsync(user, db.Roles.Single(x => x.Id == role).Name);

                user.Email = model.Email;
                user.BirthDate = model.BirthDate.Date.AddHours(12);
                user.Name = model.Name;

                var result = await userManager.UpdateAsync(user);
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

            return BadRequest(string.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage)));
        }

        [HttpGet("logout")]
        public async Task<IActionResult> SignOut()
        {
            await signInManager.SignOutAsync();
            return Ok();
        }

        [HttpGet("loggeduser")]
        public async Task<IActionResult> GetLoggedUser()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            return Ok(new { Name = user.Name, BirthDate = user.BirthDate, Email = user.Email, Login = user.UserName });
        }
    }
}