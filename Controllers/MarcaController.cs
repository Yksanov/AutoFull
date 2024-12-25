using Microsoft.AspNetCore.Mvc;
using AutoFull.Models;
using AutoFull.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace AutoFull.Controllers
{
    [Authorize]
    public class MarcaController : Controller
    {
        private readonly AutoFullDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<MyUser> _userManager;

        public MarcaController(AutoFullDbContext context, IUserRepository userRepository, UserManager<MyUser> userManager)
        {
            _context = context;
            _userRepository = userRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string search = "")
        {
            var marcas = string.IsNullOrWhiteSpace(search) ? await _context.Marcas.ToListAsync() : await _context.Marcas.Where(m => m.Name.ToLower().Contains(search) || m.Description.ToLower().Contains(search)).ToListAsync();
            
            return View(marcas);
        }

        public async Task<IActionResult> GetUsers(int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            MyUser user = _userRepository.GetById(id.Value);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Marca/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marca = await _context.Marcas
                .Include(a => a.Autos)
                .Include(c => c.Carts)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (marca == null)
            {
                return NotFound();
            }

            return View(marca);
        }
        
        //GET: Marca/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Marca marca)
        {
            if (ModelState.IsValid)
            {
                await _context.AddAsync(marca);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(marca);
        }
        
        //GET: Marca/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marca = await _context.Marcas.FindAsync(id);
            if (marca == null)
            {
                return NotFound();
            }
            
            return View(marca);
        }
        
        // POST: Marca/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Marca marca)
        {
            if (id != marca.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(marca);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MarcaExists(marca.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(marca);
        }
        
        //GET: Marca/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marca = await _context.Marcas.FirstOrDefaultAsync(m => m.Id == id); // Для поиска объекта Marca по идентификатору.
            if (marca == null)
            {
                return NotFound();
            }
            return View(marca);
        }
        
        // POST: Marca/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var marca = await _context.Marcas.FindAsync(id);
            if (marca != null)
            {
                _context.Marcas.Remove(marca);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MarcaExists(int id)
        {
            return _context.Marcas.Any(e => e.Id == id);
        }
    }
}
