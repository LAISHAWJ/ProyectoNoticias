using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoticiasAPI.DTOs;
using NoticiasAPI.Data;

namespace NoticiasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly NoticiasDBContext _context;

        public CategoriasController(NoticiasDBContext context)
        {
            _context = context;
        }

        // GET: api/Categorias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategorias()
        {
            var categorias = await _context.Categorias
                .Where(c => c.Activo == true)
                .OrderBy(c => c.Nombre)
                .Select(c => new CategoriaDto
                {
                    CategoriaId = c.CategoriaId,
                    Nombre = c.Nombre,
                    Descripcion = c.Descripcion,
                    Activo = c.Activo.Value
                })
                .ToListAsync();

            return Ok(categorias);
        }

        // GET: api/Categorias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaDto>> GetCategoria(int id)
        {
            var categoria = await _context.Categorias
                .Where(c => c.CategoriaId == id && c.Activo == true)
                .Select(c => new CategoriaDto
                {
                    CategoriaId = c.CategoriaId,
                    Nombre = c.Nombre,
                    Descripcion = c.Descripcion,
                    Activo = c.Activo.Value
                })
                .FirstOrDefaultAsync();

            if (categoria == null)
            {
                return NotFound(new { message = "Categoría no encontrada" });
            }

            return Ok(categoria);
        }

        // GET: api/Categorias/5/Noticias
        [HttpGet("{id}/Noticias")]
        public async Task<ActionResult<IEnumerable<NoticiaDto>>> GetNoticiasPorCategoria(int id)
        {
            var noticias = await _context.Noticias
                .Include(n => n.Pais)
                .Include(n => n.Categoria)
                .Include(n => n.Usuario)
                .Where(n => n.CategoriaId == id && n.Activo == true)
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