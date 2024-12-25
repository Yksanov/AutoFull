using AutoFull.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoFull.Controllers;

public class CartController : Controller
{
    private readonly UserManager<MyUser> _userManager;
    private readonly AutoFullDbContext _context;

    public CartController(UserManager<MyUser> userManager, AutoFullDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int autoId, int marcaId, int quantity)
    {
        MyUser user = await _userManager.GetUserAsync(User);
        
        Auto? auto = await _context.Autos.FirstOrDefaultAsync(a => a.Id == autoId);
        if (auto == null)
        {
            return NotFound();
        }

        Cart? cart = await _context.Carts
            .Include(c => c.CartAutos)
            .ThenInclude(ca => ca.Auto)
            .FirstOrDefaultAsync(m => m.UserId == user.Id && m.MarcaId == marcaId);

        if (cart == null)
        {
            cart = new Cart()
            {
                UserId = user.Id,
                MarcaId = marcaId
            };
            await _context.Carts.AddAsync(cart);
        }

        CartAuto? cartAuto = await _context.CartAutos.FirstOrDefaultAsync(c => c.CartId == cart.Id && c.AutoId == autoId);

        if (cartAuto != null)
        {
            cartAuto.Quantity += quantity;
            _context.CartAutos.Update(cartAuto);
        }
        else
        {
            cartAuto = new CartAuto()
            {
                CartId = cart.Id,
                AutoId = autoId,
                Quantity = quantity
            };
            await _context.CartAutos.AddAsync(cartAuto);
        }

        await _context.SaveChangesAsync();
        return PartialView("_CartPartialView", cart);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteFromCart(int autoId)
    {
        CartAuto? cartAuto = await _context.CartAutos.Include(ca => ca.Auto).FirstOrDefaultAsync(ca => ca.Id == autoId);

        if (cartAuto == null)
        {
            return NotFound();
        }
        _context.CartAutos.Remove(cartAuto);
        await _context.SaveChangesAsync();

        Cart? cart = await _context.Carts.Include(c => c.CartAutos).ThenInclude(ca => ca.Auto).FirstOrDefaultAsync(c => c.Id == cartAuto.CartId);

        if (cart == null)
        {
            return NotFound();
        }
        
        return PartialView("_CartPartialView", cart);
    }
}