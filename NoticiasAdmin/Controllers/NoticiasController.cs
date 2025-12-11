using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NoticiasAdmin.Data;
using NoticiasAdmin.Models;
using System.Security.Claims;

namespace NoticiasAdmin.Controllers
{
    [Authorize(Policy = "EditorOrAdmin")]
    public class NoticiasController : Controller
    {
        private readonly NoticiasDBContext _context;

        public NoticiasController(NoticiasDBContext context)
        {
            _context = context;
        }

        // GET: Noticias
        public async Task<IActionResult> Index(string searchString, int? categoriaId, int? paisId)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var rol = User.FindFirstValue(ClaimTypes.Role);

            var noticiasQuery = _context.Noticias
                .Include(n => n.Categoria)
                .Include(n => n.Pais)
                .Include(n => n.Usuario)
                .AsQueryable();

            // Editor solo ve sus noticias, Admin ve todas
            if (rol == "Editor")
            {
                noticiasQuery = noticiasQuery.Where(n => n.UsuarioId == usuarioId);
            }

            // Aplicar filtros
            if (!string.IsNullOrEmpty(searchString))
            {
                noticiasQuery = noticiasQuery.Where(n =>
                    n.Titulo.Contains(searchString) ||
                    n.Contenido.Contains(searchString));
                ViewData["SearchString"] = searchString;
            }

            if (categoriaId.HasValue)
            {
                noticiasQuery = noticiasQuery.Where(n => n.CategoriaId == categoriaId);
                ViewData["CategoriaId"] = categoriaId;
            }

            if (paisId.HasValue)
            {
                noticiasQuery = noticiasQuery.Where(n => n.PaisId == paisId);
                ViewData["PaisId"] = paisId;
            }

            var noticias = await noticiasQuery
                .OrderByDescending(n => n.FechaPublicacion)
                .ToListAsync();

            // Cargar datos para filtros
            ViewBag.Categorias = await _context.Categorias
                .Where(c => c.Activo == true)
                .ToListAsync();
            ViewBag.Paises = await _context.Paises
                .Where(p => p.Activo == true)
                .ToListAsync();

            return View(noticias);
        }

        // GET: Noticias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var rol = User.FindFirstValue(ClaimTypes.Role);

            var noticia = await _context.Noticias
                .Include(n => n.Categoria)
                .Include(n => n.Pais)
                .Include(n => n.Usuario)
                .FirstOrDefaultAsync(m => m.NoticiaId == id);

            if (noticia == null)
                return NotFound();

            // Editor solo puede ver sus propias noticias
            if (rol == "Editor" && noticia.UsuarioId != usuarioId)
                return Forbid();

            return View(noticia);
        }

        // GET: Noticias/Create
        public async Task<IActionResult> Create()
        {
            ViewData["CategoriaId"] = new SelectList(
                await _context.Categorias.Where(c => c.Activo == true).ToListAsync(),
                "CategoriaId", "Nombre");
            ViewData["PaisId"] = new SelectList(
                await _context.Paises.Where(p => p.Activo == true).ToListAsync(),
                "PaisId", "Nombre");
            return View();
        }

        // POST: Noticias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Titulo,Contenido,Resumen,ImagenUrl,PaisId,CategoriaId")] Noticia noticia)
        {
            if (ModelState.IsValid)
            {
                var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                noticia.UsuarioId = usuarioId;
                noticia.FechaPublicacion = DateTime.Now;
                noticia.Activo = true;

                _context.Add(noticia);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Noticia creada exitosamente";
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoriaId"] = new SelectList(
                await _context.Categorias.Where(c => c.Activo == true).ToListAsync(),
                "CategoriaId", "Nombre", noticia.CategoriaId);
            ViewData["PaisId"] = new SelectList(
                await _context.Paises.Where(p => p.Activo == true).ToListAsync(),
                "PaisId", "Nombre", noticia.PaisId);
            return View(noticia);
        }

        // GET: Noticias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var rol = User.FindFirstValue(ClaimTypes.Role);

            var noticia = await _context.Noticias.FindAsync(id);
            if (noticia == null)
                return NotFound();

            // Editor solo puede editar sus propias noticias
            if (rol == "Editor" && noticia.UsuarioId != usuarioId)
                return Forbid();

            ViewData["CategoriaId"] = new SelectList(
                await _context.Categorias.Where(c => c.Activo == true).ToListAsync(),
                "CategoriaId", "Nombre", noticia.CategoriaId);
            ViewData["PaisId"] = new SelectList(
                await _context.Paises.Where(p => p.Activo == true).ToListAsync(),
                "PaisId", "Nombre", noticia.PaisId);
            return View(noticia);
        }

        // POST: Noticias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NoticiaId,Titulo,Contenido,Resumen,ImagenUrl,PaisId,CategoriaId,UsuarioId,FechaPublicacion,Activo")] Noticia noticia)
        {
            if (id != noticia.NoticiaId)
                return NotFound();

            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var rol = User.FindFirstValue(ClaimTypes.Role);

            // Editor solo puede editar sus propias noticias
            if (rol == "Editor" && noticia.UsuarioId != usuarioId)
                return Forbid();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(noticia);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Noticia actualizada exitosamente";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NoticiaExists(noticia.NoticiaId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoriaId"] = new SelectList(
                await _context.Categorias.Where(c => c.Activo == true).ToListAsync(),
                "CategoriaId", "Nombre", noticia.CategoriaId);
            ViewData["PaisId"] = new SelectList(
                await _context.Paises.Where(p => p.Activo == true).ToListAsync(),
                "PaisId", "Nombre", noticia.PaisId);
            return View(noticia);
        }

        // GET: Noticias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var rol = User.FindFirstValue(ClaimTypes.Role);

            var noticia = await _context.Noticias
                .Include(n => n.Categoria)
                .Include(n => n.Pais)
                .Include(n => n.Usuario)
                .FirstOrDefaultAsync(m => m.NoticiaId == id);

            if (noticia == null)
                return NotFound();

            // Editor solo puede eliminar sus propias noticias
            if (rol == "Editor" && noticia.UsuarioId != usuarioId)
                return Forbid();

            return View(noticia);
        }

        // POST: Noticias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var rol = User.FindFirstValue(ClaimTypes.Role);

            var noticia = await _context.Noticias.FindAsync(id);
            if (noticia != null)
            {
                // Editor solo puede eliminar sus propias noticias
                if (rol == "Editor" && noticia.UsuarioId != usuarioId)
                    return Forbid();

                _context.Noticias.Remove(noticia);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Noticia eliminada exitosamente";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool NoticiaExists(int id)
        {
            return _context.Noticias.Any(e => e.NoticiaId == id);
        }
    }
}