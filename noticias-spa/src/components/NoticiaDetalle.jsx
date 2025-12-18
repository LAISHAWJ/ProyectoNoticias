import { useEffect } from 'react';

const NoticiaDetalle = ({ noticia, onClose }) => {
  useEffect(() => {
    document.body.style.overflow = 'hidden';
    return () => {
      document.body.style.overflow = 'unset';
    };
  }, []);

  if (!noticia) return null;

  const formatearFecha = (fecha) => {
    return new Date(fecha).toLocaleDateString('es-DO', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  const imagenPorDefecto = 'https://via.placeholder.com/800x400/667eea/ffffff?text=Noticia';

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal" onClick={(e) => e.stopPropagation()}>
        <button className="modal-close" onClick={onClose}>✕</button>
        
        <div className="modal-header">
          <img
            src={noticia.imagenUrl || imagenPorDefecto}
            alt={noticia.titulo}
            className="modal-img"
            onError={(e) => {
              e.target.src = imagenPorDefecto;
            }}
          />
          <div className="modal-badges">
            <span className="badge badge-cat">{noticia.categoriaNombre}</span>
            <span className="badge badge-pais">{noticia.paisNombre}</span>
          </div>
        </div>

        <div className="modal-body">
          <h2 className="modal-titulo">{noticia.titulo}</h2>
          
          <div className="modal-meta">
            <div className="meta-item">
              <span></span>
              <span>{noticia.autorNombre}</span>
            </div>
            <div className="meta-item">
              <span></span>
              <span>{formatearFecha(noticia.fechaPublicacion)}</span>
            </div>
          </div>

          {noticia.resumen && (
            <div className="modal-resumen">
              <strong>Resumen:</strong> {noticia.resumen}
            </div>
          )}

          <div className="modal-contenido">
            {noticia.contenido}
          </div>
        </div>
      </div>
    </div>
  );
};

export default NoticiaDetalle;