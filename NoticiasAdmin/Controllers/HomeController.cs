using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoticiasAdmin.Data;
using NoticiasAdmin.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace NoticiasAdmin.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly NoticiasDBContext _context;

        public HomeController(NoticiasDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var rol = User.FindFirstValue(ClaimTypes.Role);

            // Estadísticas
            ViewBag.TotalNoticias = rol == "Administrador"
                ? await _context.Noticias.CountAsync()
                : await _context.Noticias.CountAsync(n => n.UsuarioId == usuarioId);

            ViewBag.TotalCategorias = await _context.Categorias.CountAsync(c => c.Activo == true);
            ViewBag.TotalPaises = await _context.Paises.CountAsync(p => p.Activo == true);
            ViewBag.TotalUsuarios = rol == "Administrador"
                ? await _context.Usuarios.CountAsync(u => u.Activo == true)
                : 0;

            // Últimas noticias
            var ultimasNoticias = rol == "Administrador"
                ? await _context.Noticias
                    .Include(n => n.Categoria)
                    .Include(n => n.Usuario)
                    .OrderByDescending(n => n.FechaPublicacion)
                    .Take(5)
                    .ToListAsync()
                : await _context.Noticias
                    .Include(n => n.Categoria)
                    .Include(n => n.Usuario)
                    .Where(n => n.UsuarioId == usuarioId)
                    .OrderByDescending(n => n.FechaPublicacion)
                    .Take(5)
                    .ToListAsync();

            ViewBag.UltimasNoticias = ultimasNoticias;
            ViewBag.Rol = rol;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}