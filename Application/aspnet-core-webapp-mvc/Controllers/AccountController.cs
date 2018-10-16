﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace aspnet_core_webapp_mvc.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult SignIn()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/"
            });
        }

        public IActionResult SignOut()
        {
            return SignOut(
                new AuthenticationProperties
                {
                    RedirectUri = "/Account/SignedOut"
                },
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        public IActionResult SignedOut() => View();
    }
}
