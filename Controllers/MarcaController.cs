using Microsoft.AspNetCore.Mvc;
using AutoFull.Models;
using AutoFull.Repositories;
using AutoFull.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> Index(string search = "", int page = 1)
        {
            var marcas = string.IsNullOrWhiteSpace(search) ? await _context.Marcas.ToListAsync() : await _context.Marcas.Where(m => m.Name.ToLower().Contains(search) || m.Description.ToLower().Contains(search)).ToListAsync();
            
            int pageSize = 4;
            int marcaCount = marcas.Count();
            List<Marca> marcaList = marcas.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            IndexViewModel vm = new IndexViewModel()
            {
                Marcas = marcaList,
                PageViewModel = new PageViewModel(marcaCount, page, pageSize),
            };
            return View(vm);
        }

        public async Task<IActionResult> GetUsers(int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }
            
            var user = await _userRepository.GetUserAsync(User);

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

            MyUser user = await _userManager.GetUserAsync(User);

            return View(new MarcaViewModel
            {
                CurrentUser = user,
                MarcasDetais = marca
            });

            //return View(marca);
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
        
        //---------------------------------------------------

            [HttpPost]
            public async Task<IActionResult> AddFeedback(int marcaId, string feedbackText, int rating)
            {
                Marca? marca = await _context.Marcas
                    .Include(a => a.Autos)
                    .Include(e => e.Feedbacks)
                    .ThenInclude(u => u.User)
                    .FirstOrDefaultAsync(m => m.Id == marcaId);
                
                if (marca == null)
                {
                    return NotFound();
                }
                
                if (string.IsNullOrWhiteSpace(feedbackText) || rating <= 0 || rating > 5)
                {
                    Console.WriteLine("Некорректные данные: feedbackText=" + feedbackText + ", rating=" + rating);
                    return BadRequest("Некорректные данные");
                }

                MyUser user = await _userRepository.GetUserAsync(User);

                if ((await _context.Feedbacks.Include(u =>  u.User).AnyAsync(f => f.UserId == user.Id && f.MarcaId == marca.Id)))
                {
                    return BadRequest();
                }
                
                Feedback feedback = new Feedback
                {
                    Rating = rating,
                    Text = feedbackText,
                    CreationDate = DateTime.UtcNow,
                    MarcaId = marcaId,
                    UserId = user.Id
                };

                await _context.Feedbacks.AddAsync(feedback);
                Console.WriteLine("Отзыв добавлен: " + feedbackText); // Лог для проверки
                await _context.SaveChangesAsync();

                

                return PartialView("_FeedbacksPartialView", new MarcaViewModel()
                {
                    CurrentUser = user,
                    MarcasDetais = marca
                });
            }

        public async Task<IActionResult> DeleteFeedback(int marcaId)
        {
            Marca? marca =  await _context.Marcas
                .Include(e => e.Feedbacks)
                .ThenInclude(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == marcaId);
            
            if (marca == null)
            {
                return NotFound("Страница не найдено");
            }
            
            MyUser user = await _userRepository.GetUserAsync(User);

            Feedback? feedback = (await _context.Feedbacks.Include(f => f.User).FirstOrDefaultAsync(f => f.MarcaId == marcaId && f.UserId == user.Id));

            if (feedback == null)
            {
                return NotFound();
            }

            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new {id = marca.Id});
        }
    }
}
