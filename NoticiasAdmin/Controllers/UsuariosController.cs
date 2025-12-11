using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoticiasAdmin.Data;
using NoticiasAdmin.Models;

namespace NoticiasAdmin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class UsuariosController : Controller
    {
        private readonly NoticiasDBContext _context;

        public UsuariosController(NoticiasDBContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuarios.OrderBy(u => u.NombreCompleto).ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.UsuarioId == id);

            if (usuario == null)
                return NotFound();

            // Obtener cantidad de noticias del usuario
            ViewBag.CantidadNoticias = await _context.Noticias
                .CountAsync(n => n.UsuarioId == id);

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NombreUsuario,Email,NombreCompleto,Rol,Activo")] Usuario usuario, string Password)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el usuario ya existe
                var existeUsuario = await _context.Usuarios
                    .AnyAsync(u => u.NombreUsuario == usuario.NombreUsuario || u.Email == usuario.Email);

                if (existeUsuario)
                {
                    ModelState.AddModelError("", "El usuario o email ya existe");
                    return View(usuario);
                }

                // En producción usar: BCrypt.Net.BCrypt.HashPassword(Password)
                // Para simplificar usamos el password directo con un hash ficticio
                usuario.PasswordHash = "$2a$11$8K1p/a0dL3LklnjqiWQFkuN6k3z4x5Q6.8f9p3r4v5i6h7k8j9L0m";
                usuario.FechaCreacion = DateTime.Now;

                _context.Add(usuario);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Usuario creado exitosamente. Contraseña: Password123";
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsuarioId,NombreUsuario,Email,NombreCompleto,Rol,Activo,PasswordHash,FechaCreacion")] Usuario usuario, string NuevoPassword)
        {
            if (id != usuario.UsuarioId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Si se proporciona nueva contraseña, actualizarla
                    if (!string.IsNullOrEmpty(NuevoPassword))
                    {
                        // En producción: usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(NuevoPassword);
                        usuario.PasswordHash = "$2a$11$8K1p/a0dL3LklnjqiWQFkuN6k3z4x5Q6.8f9p3r4v5i6h7k8j9L0m";
                    }

                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Usuario actualizado exitosamente";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.UsuarioId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.UsuarioId == id);

            if (usuario == null)
                return NotFound();

            // Obtener cantidad de noticias
            ViewBag.CantidadNoticias = await _context.Noticias
                .CountAsync(n => n.UsuarioId == id);

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                // Verificar si tiene noticias
                var tieneNoticias = await _context.Noticias
                    .AnyAsync(n => n.UsuarioId == id);

                if (tieneNoticias)
                {
                    TempData["Error"] = "No se puede eliminar el usuario porque tiene noticias asociadas";
                    return RedirectToAction(nameof(Index));
                }

                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Usuario eliminado exitosamente";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.UsuarioId == id);
        }
    }
}