using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoticiasAdmin.Data;
using BCrypt.Net;
using NoticiasAdmin.Models;

namespace NoticiasAdmin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class UsuariosController : Controller
    {
        private readonly NoticiasDBContext _context;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(NoticiasDBContext context, ILogger<UsuariosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios
                .OrderBy(u => u.NombreCompleto)
                .ToListAsync();
            return View(usuarios);
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
            // Remover validación de PasswordHash ya que no viene del formulario
            ModelState.Remove("PasswordHash");

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el usuario ya existe
                    var existeUsuario = await _context.Usuarios
                        .AnyAsync(u => u.NombreUsuario == usuario.NombreUsuario || u.Email == usuario.Email);

                    if (existeUsuario)
                    {
                        ModelState.AddModelError("", "El nombre de usuario o email ya existe");
                        return View(usuario);
                    }

                    // Validar contraseña
                    if (string.IsNullOrWhiteSpace(Password))
                    {
                        ModelState.AddModelError("Password", "La contraseña es requerida");
                        return View(usuario);
                    }

                    if (Password.Length < 6)
                    {
                        ModelState.AddModelError("Password", "La contraseña debe tener al menos 6 caracteres");
                        return View(usuario);
                    }

                    // Hashear la contraseña con BCrypt
                    usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password, workFactor: 11);
                    usuario.FechaCreacion = DateTime.Now;

                    _context.Add(usuario);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Usuario '{usuario.NombreUsuario}' creado exitosamente por '{User.Identity.Name}'");
                    TempData["Success"] = $"Usuario creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error al crear usuario '{usuario.NombreUsuario}'");
                    ModelState.AddModelError("", "Ocurrió un error al crear el usuario");
                }
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
        public async Task<IActionResult> Edit(int id, [Bind("UsuarioId,NombreUsuario,Email,NombreCompleto,Rol,Activo,PasswordHash,FechaCreacion")] Usuario usuario, string NuevaPassword)
        {
            if (id != usuario.UsuarioId)
                return NotFound();

            // Remover validación de campos que no vienen del formulario
            ModelState.Remove("NuevaPassword");

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener el usuario actual de la base de datos
                    var usuarioExistente = await _context.Usuarios
                        .AsNoTracking()
                        .FirstOrDefaultAsync(u => u.UsuarioId == id);

                    if (usuarioExistente == null)
                        return NotFound();

                    // Si se proporciona nueva contraseña, validarla y hashearla
                    if (!string.IsNullOrWhiteSpace(NuevaPassword))
                    {
                        if (NuevaPassword.Length < 6)
                        {
                            ModelState.AddModelError("NuevaPassword", "La nueva contraseña debe tener al menos 6 caracteres");
                            return View(usuario);
                        }

                        // Hashear nueva contraseña con BCrypt
                        usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(NuevaPassword, workFactor: 11);
                        _logger.LogInformation($"Contraseña actualizada para usuario '{usuario.NombreUsuario}' por '{User.Identity.Name}'");
                    }
                    else
                    {
                        // Mantener la contraseña actual si no se proporciona una nueva
                        usuario.PasswordHash = usuarioExistente.PasswordHash;
                    }

                    // Mantener la fecha de creación original
                    usuario.FechaCreacion = usuarioExistente.FechaCreacion;

                    _context.Update(usuario);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Usuario '{usuario.NombreUsuario}' actualizado por '{User.Identity.Name}'");
                    TempData["Success"] = "Usuario actualizado exitosamente";

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.UsuarioId))
                        return NotFound();
                    else
                        throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error al actualizar usuario '{usuario.NombreUsuario}'");
                    ModelState.AddModelError("", "Ocurrió un error al actualizar el usuario");
                }
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
            try
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

                    _logger.LogInformation($"Usuario '{usuario.NombreUsuario}' eliminado por '{User.Identity.Name}'");
                    TempData["Success"] = "Usuario eliminado exitosamente";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar usuario con ID {id}");
                TempData["Error"] = "Ocurrió un error al eliminar el usuario";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.UsuarioId == id);
        }
    }
}