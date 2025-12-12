using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoticiasAPI.DTOs;
using NoticiasAPI.Data;

namespace NoticiasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaisesController : ControllerBase
    {
        private readonly NoticiasDBContext _context;

        public PaisesController(NoticiasDBContext context)
        {
            _context = context;
        }

        // GET: api/Paises
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaisDto>>> GetPaises()
        {
            var paises = await _context.Paises
                .Where(p => p.Activo == true)
                .OrderBy(p => p.Nombre)
                .Select(p => new PaisDto
                {
                    PaisId = p.PaisId,
                    Nombre = p.Nombre,
                    Codigo = p.Codigo,
                    Activo = p.Activo.Value
                })
                .ToListAsync();

            return Ok(paises);
        }

        // GET: api/Paises/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PaisDto>> GetPais(int id)
        {
            var pais = await _context.Paises
                .Where(p => p.PaisId == id && p.Activo == true)
                .Select(p => new PaisDto
                {
                    PaisId = p.PaisId,
                    Nombre = p.Nombre,
                    Codigo = p.Codigo,
                    Activo = p.Activo.Value
                })
                .FirstOrDefaultAsync();

            if (pais == null)
            {
                return NotFound(new { message = "País no encontrado" });
            }

            return Ok(pais);
        }

        // GET: api/Paises/5/Noticias
        [HttpGet("{id}/Noticias")]
        public async Task<ActionResult<IEnumerable<NoticiaDto>>> GetNoticiasPorPais(int id)
        {
            var noticias = await _context.Noticias
                .Include(n => n.Pais)
                .Include(n => n.Categoria)
                .Include(n => n.Usuario)
                .Where(n => n.PaisId == id && n.Activo == true)
                .OrderByDescending(n => n.FechaPublicacion)
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
    }
}