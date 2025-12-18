const NoticiaCard = ({ noticia, onClick }) => {
  const formatearFecha = (fecha) => {
    return new Date(fecha).toLocaleDateString('es-DO', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
  };

  const imagenPorDefecto = 'https://via.placeholder.com/400x250/667eea/ffffff?text=Noticia';

  return (
    <article className="card" onClick={() => onClick(noticia)}>
      <div className="card-img">
        <img
          src={noticia.imagenUrl || imagenPorDefecto}
          alt={noticia.titulo}
          onError={(e) => {
            e.target.src = imagenPorDefecto;
          }}
        />
        <span className="card-categoria">{noticia.categoriaNombre}</span>
      </div>
      
      <div className="card-body">
        <h3 className="card-titulo">{noticia.titulo}</h3>
        <p className="card-resumen">{noticia.resumen}</p>
        
        <div className="card-meta">
          <span>{noticia.autorNombre}</span>
          <span>{formatearFecha(noticia.fechaPublicacion)}</span>
        </div>
        
        <div className="card-footer">
          <span className="card-pais"> {noticia.paisNombre}</span>
          <button className="btn-leer">Leer más →</button>
        </div>
      </div>
    </article>
  );
};

export default NoticiaCard;