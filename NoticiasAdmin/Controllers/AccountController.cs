using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoticiasAdmin.Data;
using NoticiasAdmin.ViewModels;
using System.Security.Claims;

namespace NoticiasAdmin.Controllers
{
    public class AccountController : Controller
    {
        private readonly NoticiasDBContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(NoticiasDBContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            // Si ya está autenticado, redirigir al home
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Buscar usuario en la base de datos
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.NombreUsuario == model.NombreUsuario && u.Activo == true);

                if (usuario == null)
                {
                    _logger.LogWarning($"Intento de login fallido: Usuario '{model.NombreUsuario}' no encontrado");
                    ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos");
                    return View(model);
                }

                // Verificar contraseña usando BCrypt
                bool passwordValida = BCrypt.Net.BCrypt.Verify(model.Password, usuario.PasswordHash);

                if (!passwordValida)
                {
                    _logger.LogWarning($"Intento de login fallido: Contraseña incorrecta para usuario '{model.NombreUsuario}'");
                    ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos");
                    return View(model);
                }

                // Usuario autenticado correctamente
                _logger.LogInformation($"Usuario '{usuario.NombreUsuario}' autenticado exitosamente");

                // Crear claims para el usuario
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                    new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim(ClaimTypes.Role, usuario.Rol),
                    new Claim("NombreCompleto", usuario.NombreCompleto)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = false, // No persistente
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(3),
                    AllowRefresh = true
                };


                // Iniciar sesión
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Redirigir
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error durante el login del usuario '{model.NombreUsuario}'");
                ModelState.AddModelError(string.Empty, "Ocurrió un error durante el inicio de sesión. Por favor, intente nuevamente.");
                return View(model);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var nombreUsuario = User.Identity?.Name ?? "Usuario desconocido";
            _logger.LogInformation($"Usuario '{nombreUsuario}' cerró sesión");

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            _logger.LogWarning($"Acceso denegado al usuario '{User.Identity?.Name}'");
            return View();
        }

        // Método auxiliar para generar hash de contraseña 
        [NonAction]
        public string GenerarHashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);
        }
    }
}