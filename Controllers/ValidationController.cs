using AutoFull.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AutoFull.Controllers;

public class ValidationController : Controller
{
    private readonly UserManager<MyUser> _userManager;
    private readonly AutoFullDbContext _context;

    public ValidationController(UserManager<MyUser> userManager, AutoFullDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [AcceptVerbs("GET", "POST")]
    public async Task<IActionResult> CheckUserEmail(string email, int id)
    {
        MyUser user = await _userManager.FindByEmailAsync(email);

        if (user != null && user.Id != id)
        {
            return Json(false);
        }

        return Json(true);
    }

    [AcceptVerbs("GET", "POST")]
    public async Task<IActionResult> CheckUserName(string username, int id)
    {
        MyUser user = await _userManager.FindByNameAsync(username);

        if (user != null && user.Id != id)
        {
            return Json(false);
        }
        return Json(true);
    }
    
}