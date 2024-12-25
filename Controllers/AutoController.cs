using AutoFull.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoFull.Controllers;

public class AutoController : Controller
{
    private readonly AutoFullDbContext _context;
    private readonly UserManager<MyUser> _userManager;

    public AutoController(AutoFullDbContext context, UserManager<MyUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Auto autos = await _context.Autos.Include(a => a.Marca).FirstOrDefaultAsync(m => m.Id == id); // ишет авто в БД

        if (autos == null)
        {
            return NotFound();
        }
        
        return View(autos);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.MarcaId = new SelectList(_context.Marcas, "Id", "Name");
        
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Auto auto)
    {
        if (ModelState.IsValid)
        {
            _context.Add(auto);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details","Marca", new  {id = auto.MarcaId});
        }

        ViewBag.MarcaId = new SelectList(_context.Marcas, "Id", "Name");
        return View(auto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var auto = await _context.Autos.FindAsync(id);
        if (auto != null)
        {
            _context.Autos.Remove(auto);
        }

        await _context.SaveChangesAsync();

        string refererUrl = Request.Headers["Referer"].ToString();
        if (string.IsNullOrEmpty(refererUrl))
        {
            return Redirect(refererUrl);
        }

        return RedirectToAction("Index", "Marca");
    }

    private bool AutoExists(int id)
    {
        return _context.Autos.Any(a => a.Id == id);
    }
}