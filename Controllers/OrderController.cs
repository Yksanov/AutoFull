using AutoFull.Models;
using AutoFull.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoFull.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly AutoFullDbContext _context;
    private readonly UserManager<MyUser> _userManager;
    private readonly EmailService _emailService;

    public OrderController(AutoFullDbContext context, UserManager<MyUser> userManager, EmailService emailService)
    {
        _context = context;
        _userManager = userManager;
        _emailService = emailService;
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        return View(await _context.Orders.Include(o => o.User).Include(o => o.Marca).ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create(int cardId, string customerName, string email, string phoneNumber, string address)
    {
        MyUser user = await _userManager.GetUserAsync(User);

        Cart? cart = await _context.Carts
            .Include(ca => ca.CartAutos)
            .ThenInclude(a => a.Auto)
            .FirstOrDefaultAsync(c => c.Id == cardId);

        if (cart == null || !cart.CartAutos.Any())
        {
            return NotFound();
        }

        Order order = new Order()
        {
            UserId = user.Id,
            MarcaId = cart.MarcaId,
            TotalPrice = (double)cart.CartAutos.Sum(a => a.Auto.Price * a.Quantity),
            //CustomerName = customerName,
            Phone = phoneNumber,
            Address = address,
            Email = email,
            OrderDate = DateTime.UtcNow,
            Autos = cart.CartAutos.Select(a => a.Auto).ToList()
        };
        
        await _context.Orders.AddAsync(order);
        _context.CartAutos.RemoveRange(cart.CartAutos);
        _context.Carts.Remove(cart);
        await _context.SaveChangesAsync();

        string subject = $"Заказ #{order.Id}";
        string message = $"Детали:\n" +
                         $"Получатель: {customerName}\n" +
                         $"Адрес доставки: {address}\n" +
                         $"Почта получателя: {email}\n" +
                         $"Номер телефона получателя: {phoneNumber}\n" +
                         $"Блюда: \n" +
                         $"{string.Join(", ", order.Autos.Select(a => a.Name).ToList())}";
        
        _emailService.SendEmailAsync(email,subject, message);

        return RedirectToAction("Details", "Marca", new {id = cart.MarcaId});
    }
}