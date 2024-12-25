using AutoFull.Models;
using AutoFull.Services;
using AutoFull.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace AutoFull.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<MyUser> _userManager;
    private readonly SignInManager<MyUser> _signInManager;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly EmailService _emailService;

    public AccountController(UserManager<MyUser> userManager, SignInManager<MyUser> signInManager, IWebHostEnvironment webHostEnvironment, EmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _webHostEnvironment = webHostEnvironment;
        _emailService = emailService;
    }


    [HttpGet]
    public async Task<IActionResult> Login(string returnUrl = "")
    {
        return View(new LoginViewModel {ReturnUrl = returnUrl});
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            MyUser user = await _userManager.FindByEmailAsync(model.UserNameOrEmail);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(model.UserNameOrEmail);
            }

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Пользователь не найден!");
                return View(model);
            }
            
            SignInResult result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                if(!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);
                return RedirectToAction("Index", "Marca");
            }
            ModelState.AddModelError(string.Empty, "Неправильное логин или пароль");
        }
        return View(model);
    }


    [HttpGet]
    public async Task<IActionResult> Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            string fileName = $"avatar_{model.Email}{Path.GetExtension(model.Avatar.FileName)}";
            if (model.Avatar != null && model.Avatar.Length > 0 && model.Avatar.ContentType.StartsWith("image/"))
            {
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "avatars", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Avatar.CopyToAsync(stream);
                }
            }
            else
            {
                ModelState.AddModelError("Avatar", "Изображение может быть только картинкой (image)");
                return View(model);
            }

            MyUser user = new MyUser()
            {
                UserName = model.UserName.ToLower(),
                Email = model.Email,
                PathAvatar = $"/avatars/{fileName}",
                PhoneNumber = model.PhoneNumber
            };
            
            IdentityResult result =  await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("Index", "Marca");
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            // string subject = $"Зарегистрировалисть: {model.Id}";
            // string emailBody = $"------------------\n" +
            //                    $"Name: {customerName}\n" +
            //                    $"Email: {email}\n" +
            //                    $"Phone: {phoneNumber}\n";
            //
            // await _emailService.SendEmailAsync(email, subject, emailBody);

        }
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("", "");
    }
}