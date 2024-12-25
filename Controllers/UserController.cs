using AutoFull.Models;
using AutoFull.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoFull.Controllers;

public class UserController : Controller
{
    private readonly AutoFullDbContext _context;
    private readonly UserManager<MyUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    public UserController(AutoFullDbContext context, UserManager<MyUser> userManager, RoleManager<IdentityRole<int>> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.Users.ToListAsync();
        return View(user);
    }

    [HttpGet]
    public async Task<IActionResult> Profile(int? id)
    {
        MyUser user = await _userManager.GetUserAsync(User);

        MyUser currentUser = await _userManager.GetUserAsync(User);

        if (id != null)
        {
            user = await _userManager.FindByIdAsync(id.ToString());
        }

        if (user == null)
        {
            return NotFound();
        }
        
        return View(new ProfileViewModel()
        {
            User = user,
            CurrentUser = currentUser
        });
    }

    [HttpPost]
    public async Task<IActionResult> Search(string search)
    {
        if (string.IsNullOrEmpty(search))
        {
            return View(new List<MyUser>());
        }
        List<MyUser> user = await _context.Users.Where(u => u.UserName.ToLower().Contains(search.ToLower()) || u.Email.ToLower().Contains(search.ToLower())).ToListAsync();

        return View("Search", user);
    }

    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            MyUser user = new MyUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                if (string.IsNullOrEmpty(model.Role))
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                }

                return RedirectToAction("Index", "User");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        MyUser user = await _userManager.FindByIdAsync(id.ToString());
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
        }
        
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ChangeRoles(int id)
    {
        MyUser? user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return NotFound();
        }

        if (await _userManager.IsInRoleAsync(user, "user"))
        {
            await _userManager.RemoveFromRoleAsync(user, "user");
            await _userManager.AddToRoleAsync(user, "admin");
        }
        else
        {
            await _userManager.RemoveFromRoleAsync(user, "admin");
            await _userManager.AddToRoleAsync(user, "user");
        }

        await _userManager.UpdateAsync(user);
        return RedirectToAction("Index", "User");
    }
}