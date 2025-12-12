using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoticiasAPI.DTOs;
using NoticiasAPI.Data;

namespace NoticiasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoticiasController : ControllerBase
    {
        private readonly NoticiasDBContext _context;

        public NoticiasController(NoticiasDBContext context)
        {
            _context = context;
        }

        // GET: api/Noticias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NoticiaDto>>> GetNoticias(
            [FromQuery] int? paisId,
            [FromQuery] int? categoriaId,
            [FromQuery] string busqueda,
            [FromQuery] int pagina = 1,
            [FromQuery] int porPagina = 10)
        {
            var query = _context.Noticias
                .Include(n => n.Pais)
                .Include(n => n.Categoria)
                .Include(n => n.Usuario)
                .Where(n => n.Activo == true)
                .AsQueryable();

            // Filtros
            if (paisId.HasValue)
            {
                query = query.Where(n => n.PaisId == paisId.Value);
            }

            if (categoriaId.HasValue)
            {
                query = query.Where(n => n.CategoriaId == categoriaId.Value);
            }

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                busqueda = busqueda.ToLower();
                query = query.Where(n =>
                    n.Titulo.ToLower().Contains(busqueda) ||
                    n.Contenido.ToLower().Contains(busqueda) ||
                    n.Resumen.ToLower().Contains(busqueda));
            }

            // Ordenar por fecha descendente
            query = query.OrderByDescending(n => n.FechaPublicacion);

            // Paginación
            var total = await query.CountAsync();
            var noticias = await query
                .Skip((pagina - 1) * porPagina)
                .Take(porPagina)
                .Select(n => new NoticiaDto
                {
                    NoticiaId = n.NoticiaId,
                    Titulo = n.Titulo,
                    Contenido = n.Contenido,
                    Resumen = n.Resumen,
                    ImagenUrl = n.ImagenUrl,
                    FechaPublicacion = n.FechaPublicacion.Value,
                    Activo = n.Activo.Value,
                    PaisId = n.PaisId,
                    PaisNombre = n.Pais.Nombre,
                    PaisCodigo = n.Pais.Codigo,
                    CategoriaId = n.CategoriaId,
                    CategoriaNombre = n.Categoria.Nombre,
                    AutorNombre = n.Usuario.NombreCompleto
                })
                .ToListAsync();

            // Agregar headers de paginación
            Response.Headers.Add("X-Total-Count", total.ToString());
            Response.Headers.Add("X-Page", pagina.ToString());
            Response.Headers.Add("X-Per-Page", porPagina.ToString());
            Response.Headers.Add("X-Total-Pages", Math.Ceiling((double)total / porPagina).ToString());

            return Ok(noticias);
        }

        // GET: api/Noticias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NoticiaDto>> GetNoticia(int id)
        {
            var noticia = await _context.Noticias
                .Include(n => n.Pais)
                .Include(n => n.Categoria)
                .Include(n => n.Usuario)
                .Where(n => n.NoticiaId == id && n.Activo == true)
                .Select(n => new NoticiaDto
                {
                    NoticiaId = n.NoticiaId,
                    Titulo = n.Titulo,
                    Contenido = n.Contenido,
                    Resumen = n.Resumen,
                    ImagenUrl = n.ImagenUrl,
                    FechaPublicacion = n.FechaPublicacion.Value,
                    Activo = n.Activo.Value,
                    PaisId = n.PaisId,
                    PaisNombre = n.Pais.Nombre,
                    PaisCodigo = n.Pais.Codigo,
                    CategoriaId = n.CategoriaId,
                    CategoriaNombre = n.Categoria.Nombre,
                    AutorNombre = n.Usuario.NombreCompleto
                })
                .FirstOrDefaultAsync();

            if (noticia == null)
            {
                return NotFound(new { message = "Noticia no encontrada" });
            }

            return Ok(noticia);
        }

        // GET: api/Noticias/Destacadas
        [HttpGet("Destacadas")]
        public async Task<ActionResult<IEnumerable<NoticiaDto>>> GetNoticiasDestacadas([FromQuery] int cantidad = 5)
        {
            var noticias = await _context.Noticias
                .Include(n => n.Pais)
                .Include(n => n.Categoria)
                .Include(n => n.Usuario)
                .Where(n => n.Activo == true)
                .OrderByDescending(n => n.FechaPublicacion)
                .Take(cantidad)
                .Select(n => new NoticiaDto
                {
                    NoticiaId = n.NoticiaId,
                    Titulo = n.Titulo,
                    Contenido = n.Contenido,
                    Resumen = n.Resumen,
                    ImagenUrl = n.ImagenUrl,
                    FechaPublicacion = n.FechaPublicacion.Value,
                    Activo = n.Activo.Value,
                    PaisId = n.PaisId,
                    PaisNombre = n.Pais.Nombre,
                    PaisCodigo = n.Pais.Codigo,
                    CategoriaId = n.CategoriaId,
                    CategoriaNombre = n.Categoria.Nombre,
                    AutorNombre = n.Usuario.NombreCompleto
                })
                .ToListAsync();

            return Ok(noticias);
        }

        // GET: api/Noticias/PorCategoria/1
        [HttpGet("PorCategoria/{categoriaId}")]
        public async Task<ActionResult<IEnumerable<NoticiaDto>>> GetNoticiasPorCategoria(
            int categoriaId,
            [FromQuery] int pagina = 1,
            [FromQuery] int porPagina = 10)
        {
            var query = _context.Noticias
                .Include(n => n.Pais)
                .Include(n => n.Categoria)
                .Include(n => n.Usuario)
                .Where(n => n.CategoriaId == categoriaId && n.Activo == true)
                .OrderByDescending(n => n.FechaPublicacion);

            var total = await query.CountAsync();
            var noticias = await query
                .Skip((pagina - 1) * porPagina)
                .Take(porPagina)
                .Select(n => new NoticiaDto
                {
                    NoticiaId = n.NoticiaId,
                    Titulo = n.Titulo,
                    Contenido = n.Contenido,
                    Resumen = n.Resumen,
                    ImagenUrl = n.ImagenUrl,
                    FechaPublicacion = n.FechaPublicacion.Value,
                    Activo = n.Activo.Value,
                    PaisId = n.PaisId,
                    PaisNombre = n.Pais.Nombre,
                    PaisCodigo = n.Pais.Codigo,
                    CategoriaId = n.CategoriaId,
                    CategoriaNombre = n.Categoria.Nombre,
                    AutorNombre = n.Usuario.NombreCompleto
                })
                .ToListAsync();

            Response.Headers.Add("X-Total-Count", total.ToString());
            Response.Headers.Add("X-Page", pagina.ToString());
            Response.Headers.Add("X-Per-Page", porPagina.ToString());

            return Ok(noticias);
        }

        // GET: api/Noticias/PorPais/1
        [HttpGet("PorPais/{paisId}")]
        public async Task<ActionResult<IEnumerable<NoticiaDto>>> GetNoticiasPorPais(
            int paisId,
            [FromQuery] int pagina = 1,
            [FromQuery] int porPagina = 10)
        {
            var query = _context.Noticias
                .Include(n => n.Pais)
                .Include(n => n.Categoria)
                .Include(n => n.Usuario)
                .Where(n => n.PaisId == paisId && n.Activo == true)
                .OrderByDescending(n => n.FechaPublicacion);

            var total = await query.CountAsync();
            var noticias = await query
                .Skip((pagina - 1) * porPagina)
                .Take(porPagina)
                .Select(n => new NoticiaDto
                {
                    NoticiaId = n.NoticiaId,
                    Titulo = n.Titulo,
                    Contenido = n.Contenido,
                    Resumen = n.Resumen,
                    ImagenUrl = n.ImagenUrl,
                    FechaPublicacion = n.FechaPublicacion.Value,
                    Activo = n.Activo.Value,
                    PaisId = n.PaisId,
                    PaisNombre = n.Pais.Nombre,
                    PaisCodigo = n.Pais.Codigo,
                    CategoriaId = n.CategoriaId,
                    CategoriaNombre = n.Categoria.Nombre,
                    AutorNombre = n.Usuario.NombreCompleto
                })
                .ToListAsync();

            Response.Headers.Add("X-Total-Count", total.ToString());
            Response.Headers.Add("X-Page", pagina.ToString());
            Response.Headers.Add("X-Per-Page", porPagina.ToString());

            return Ok(noticias);
        }
    }
}