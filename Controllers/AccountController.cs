﻿using Calcular.CoreApi.Models;
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
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
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
                var user = new User { Name = model.Name, UserName = model.UserName, Email = model.Email, BirthDate = model.BirthDate };
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
        public async Task<IActionResult> GetLoggedUser()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            return Ok(new { Name = user.Name, BirthDate = user.BirthDate, Email = user.Email, Login = user.UserName });
        }
    }
}