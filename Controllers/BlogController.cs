using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using blog.Data;
using blog.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace blog.Controllers
{
		[Route("blog")]
      // Kullanıcı giriş yapmış olmalı
		[IgnoreAntiforgeryToken]
    public class BlogController : Controller
    {
			private readonly ApplicationDbContext _context;
			private readonly UserManager<ApplicationUser> _userManager;

			// Constructor
			public BlogController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
			{
					_context = context;
					_userManager = userManager;
			}
        
			// Index: Blog yazılarının listelendiği sayfa
			public async Task<IActionResult> Index()
			{
				var blogs = await _context.Blogs
						.Include(b => b.User)  // User bilgisi ile birlikte
						.Include(b => b.Category)  // Kategori bilgisi ile birlikte
						.ToListAsync();

				return View(blogs); // blogs listesini View'a gönderiyoruz
			}

			
			[HttpGet("create")]	// Create: Yeni bir blog yazısı ekleme sayfası
			public IActionResult Create()
			{
					// Kategorileri al
    			var categories = _context.Categories.ToList();

					// Kategorileri ViewBag'e gönder
					ViewBag.Categories = new SelectList(categories, "Id", "Name");
					return View();
			}

			// Create (POST): Blog yazısını veritabanına ekler
			[HttpPost("create")]
			public async Task<IActionResult> Create([Bind("Title,Content,CategoryId,ImageUrl")] Blog blog)
			{
					if (ModelState.IsValid)
					{
							// Kullanıcı kimliğini al
							blog.ApplicationUserId = _userManager.GetUserId(User);  // Current logged-in user's ID

							// Yayınlanma tarihi
							blog.PublishedDate = DateTime.Now;

							// Blog yazısını veritabanına ekle
							_context.Add(blog);
							await _context.SaveChangesAsync();

							// Başarıyla kaydedildiyse, ana sayfaya yönlendir
							return RedirectToAction(nameof(Index));
					}

					return View(blog); // Hata varsa, aynı sayfaya geri dön
			}

			[HttpGet("edit/{id}")]// Edit (GET): Bir blog yazısını düzenleme sayfasını getirir
			public async Task<IActionResult> Edit(int? id)
			{
					if (id == null)
					{
							return NotFound();
					}

					var blog = await _context.Blogs.FindAsync(id);
					if (blog == null)
					{
							return NotFound();
					}

					return View(blog); // Blog verisiyle birlikte düzenleme sayfasını göster
			}

			// Edit (POST): Blog yazısını günceller
			[HttpPost]
			public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,CategoryId,ImageUrl")] Blog blog)
			{
					if (id != blog.Id)
					{
							return NotFound();
					}

					if (ModelState.IsValid)
					{
							try
							{
									_context.Update(blog);  // Blogu güncelle
									await _context.SaveChangesAsync(); // Değişiklikleri kaydet
							}
							catch (DbUpdateConcurrencyException)
							{
									if (!BlogExists(blog.Id))
									{
											return NotFound();
									}
									else
									{
											throw;
									}
							}
							return RedirectToAction(nameof(Index)); // Başarıyla güncellenince liste sayfasına dön
					}
					return View(blog); // Hata varsa düzenleme sayfasına geri dön
			}
			private bool BlogExists(int id)
			{
					return _context.Blogs.Any(e => e.Id == id);
			}

			[HttpGet("delete/{id}")]// Delete (GET): Blog yazısını silme doğrulama sayfasını getirir
			public async Task<IActionResult> Delete(int? id)
			{
					if (id == null)
					{
							return NotFound();
					}

					var blog = await _context.Blogs
							.Include(b => b.User)  // User bilgisi ile birlikte
							.Include(b => b.Category)  // Kategori bilgisi ile birlikte
							.FirstOrDefaultAsync(m => m.Id == id);
					if (blog == null)
					{
							return NotFound();
					}

					return View(blog); // Silme onay sayfasını blog ile birlikte göster
			}

			// Delete (POST): Blog yazısını siler
			[HttpPost("delete/{id}")]
			public async Task<IActionResult> DeleteConfirmed(int id)
			{
					var blog = await _context.Blogs.FindAsync(id);
					_context.Blogs.Remove(blog);  // Blogu sil
					await _context.SaveChangesAsync();  // Değişiklikleri kaydet

					return RedirectToAction(nameof(Index));  // Silme işlemi sonrası ana sayfaya yönlendir
			}

    }
}
