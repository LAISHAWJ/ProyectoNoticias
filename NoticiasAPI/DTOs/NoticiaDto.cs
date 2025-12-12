namespace NoticiasAPI.DTOs
{
    public class NoticiaDto
    {
        public int NoticiaId { get; set; }
        public string Titulo { get; set; }
        public string Contenido { get; set; }
        public string Resumen { get; set; }
        public string ImagenUrl { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public bool Activo { get; set; }

        // Datos relacionados
        public int PaisId { get; set; }
        public string PaisNombre { get; set; }
        public string PaisCodigo { get; set; }

        public int CategoriaId { get; set; }
        public string CategoriaNombre { get; set; }

        public string AutorNombre { get; set; }
    }

    public class CategoriaDto
    {
        public int CategoriaId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
    }

    public class PaisDto
    {
        public int PaisId { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public bool Activo { get; set; }
    }
}