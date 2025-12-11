using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoticiasAdmin.Data;
using NoticiasAdmin.Models;

namespace NoticiasAdmin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class PaisesController : Controller
    {
        private readonly NoticiasDBContext _context;

        public PaisesController(NoticiasDBContext context)
        {
            _context = context;
        }

        // GET: Paises
        public async Task<IActionResult> Index()
        {
            return View(await _context.Paises.OrderBy(p => p.Nombre).ToListAsync());
        }

        // GET: Paises/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var pais = await _context.Paises
                .FirstOrDefaultAsync(m => m.PaisId == id);

            if (pais == null)
                return NotFound();

            return View(pais);
        }

        // GET: Paises/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Paises/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Codigo,Activo")] Pais pais)
        {
            if (ModelState.IsValid)
            {
                pais.FechaCreacion = DateTime.Now;
                _context.Add(pais);
                await _context.SaveChangesAsync();
                TempData["Success"] = "País creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            return View(pais);
        }

        // GET: Paises/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var pais = await _context.Paises.FindAsync(id);
            if (pais == null)
                return NotFound();

            return View(pais);
        }

        // POST: Paises/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PaisId,Nombre,Codigo,Activo,FechaCreacion")] Pais pais)
        {
            if (id != pais.PaisId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pais);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "País actualizado exitosamente";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaisExists(pais.PaisId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(pais);
        }

        // GET: Paises/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var pais = await _context.Paises
                .FirstOrDefaultAsync(m => m.PaisId == id);

            if (pais == null)
                return NotFound();

            return View(pais);
        }

        // POST: Paises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pais = await _context.Paises.FindAsync(id);
            if (pais != null)
            {
                // Verificar si hay noticias usando este país
                var tieneNoticias = await _context.Noticias
                    .AnyAsync(n => n.PaisId == id);

                if (tieneNoticias)
                {
                    TempData["Error"] = "No se puede eliminar el país porque tiene noticias asociadas";
                    return RedirectToAction(nameof(Index));
                }

                _context.Paises.Remove(pais);
                await _context.SaveChangesAsync();
                TempData["Success"] = "País eliminado exitosamente";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PaisExists(int id)
        {
            return _context.Paises.Any(e => e.PaisId == id);
        }
    }
}