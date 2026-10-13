using System.Security.Claims;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;

namespace PresentationLayer.Controllers;

public class AccountController : Controller
{
    private readonly IAccountRepository _accountRepository;
    private readonly IPasswordHasher<AccountUserRecord> _passwordHasher;

    public AccountController(
        IAccountRepository accountRepository,
        IPasswordHasher<AccountUserRecord> passwordHasher)
    {
        _accountRepository = accountRepository;
        _passwordHasher = passwordHasher;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel model,
        string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _accountRepository.FindByEmailAsync(model.Email);
        if (user is not null && _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                model.Password) != PasswordVerificationResult.Failed)
        {
            await SignInUserAsync(user, model.RememberMe);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "E-mailadres of wachtwoord is onjuist.");
        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (await _accountRepository.EmailExistsAsync(model.Email))
        {
            ModelState.AddModelError(nameof(model.Email), "Er bestaat al een account met dit e-mailadres.");
            return View(model);
        }

        var user = new AccountUserRecord
        {
            Name = model.Name,
            Email = model.Email
        };
        var passwordHash = _passwordHasher.HashPassword(user, model.Password);

        try
        {
            await _accountRepository.CreateAsync(model.Name, model.Email, passwordHash);
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(model);
        }

        user = (await _accountRepository.FindByEmailAsync(model.Email))!;
        await SignInUserAsync(user, isPersistent: false);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    private async Task SignInUserAsync(AccountUserRecord user, bool isPersistent)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email)
        };

        if (!string.IsNullOrWhiteSpace(user.RoleName))
        {
            claims.Add(new Claim(ClaimTypes.Role, user.RoleName));
        }

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var properties = new AuthenticationProperties
        {
            IsPersistent = isPersistent
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            properties);
    }
}