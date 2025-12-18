import { useState, useEffect } from 'react';
import Header from './components/Header';
import Hero from './components/Hero';
import Filtros from './components/Filtros';
import NoticiaCard from './components/NoticiaCard';
import NoticiaDetalle from './components/NoticiaDetalle';
import Loading from './components/Loading';
import Footer from './components/Footer';
import { noticiasService } from './services/api';
import './App.css';

function App() {
  const [noticias, setNoticias] = useState([]);
  const [noticiaSeleccionada, setNoticiaSeleccionada] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [filtros, setFiltros] = useState({
    categoriaId: null,
    paisId: null,
    busqueda: '',
  });
  const [paginacion, setPaginacion] = useState({
    pagina: 1,
    porPagina: 10,
    totalCount: 0,
    totalPages: 1,
  });

  useEffect(() => {
    cargarNoticias();
  }, [filtros, paginacion.pagina]);

  const cargarNoticias = async () => {
    setLoading(true);
    setError(null);
    
    try {
      const params = {
        pagina: paginacion.pagina,
        porPagina: paginacion.porPagina,
      };

      if (filtros.categoriaId) params.categoriaId = filtros.categoriaId;
      if (filtros.paisId) params.paisId = filtros.paisId;
      if (filtros.busqueda) params.busqueda = filtros.busqueda;

      const response = await noticiasService.obtenerNoticias(params);
      
      setNoticias(response.data);
      setPaginacion((prev) => ({
        ...prev,
        totalCount: response.totalCount,
        totalPages: response.totalPages,
        pagina: response.currentPage,
      }));
    } catch (err) {
      console.error('Error al cargar noticias:', err);
      setError('No se pudieron cargar las noticias. Verifica que la API esté funcionando.');
    } finally {
      setLoading(false);
    }
  };

  const handleFiltrar = (nuevosFiltros) => {
    setFiltros(nuevosFiltros);
    setPaginacion((prev) => ({ ...prev, pagina: 1 }));
  };

  const handleBuscar = (termino) => {
    setFiltros((prev) => ({ ...prev, busqueda: termino }));
    setPaginacion((prev) => ({ ...prev, pagina: 1 }));
  };

  const handleNoticiaClick = async (noticia) => {
    try {
      const noticiaCompleta = await noticiasService.obtenerNoticiaPorId(noticia.noticiaId);
      setNoticiaSeleccionada(noticiaCompleta);
    } catch (err) {
      setNoticiaSeleccionada(noticia);
    }
  };

  const handleCambiarPagina = (nuevaPagina) => {
    setPaginacion((prev) => ({ ...prev, pagina: nuevaPagina }));
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  return (
    <div className="app">
      <Header />
      <Hero onSearch={handleBuscar} />
      <Filtros onFiltrar={handleFiltrar} filtrosActivos={filtros} />

      <main className="main">
        <div className="container">
          {loading ? (
            <Loading />
          ) : error ? (
            <div className="error">
              <p>❌ {error}</p>
              <button className="btn-retry" onClick={cargarNoticias}>
                🔄 Reintentar
              </button>
            </div>
          ) : noticias.length === 0 ? (
            <div className="no-results">
              <p>📭 No se encontraron noticias</p>
              <button
                className="btn-clear"
                onClick={() => handleFiltrar({ categoriaId: null, paisId: null, busqueda: '' })}
              >
                Limpiar filtros
              </button>
            </div>
          ) : (
            <>
              <div className="results-info">
                <p>Mostrando {noticias.length} de {paginacion.totalCount} noticias</p>
              </div>

              <div className="grid" id="destacadas">
                {noticias.map((noticia) => (
                  <NoticiaCard
                    key={noticia.noticiaId}
                    noticia={noticia}
                    onClick={handleNoticiaClick}
                  />
                ))}
              </div>

              {paginacion.totalPages > 1 && (
                <div className="pagination">
                  <button
                    className="pag-btn"
                    onClick={() => handleCambiarPagina(paginacion.pagina - 1)}
                    disabled={paginacion.pagina === 1}
                  >
                    ← Anterior
                  </button>
                  
                  <div className="pag-numbers">
                    {[...Array(paginacion.totalPages)].map((_, index) => {
                      const numeroPagina = index + 1;
                      if (
                        numeroPagina === 1 ||
                        numeroPagina === paginacion.totalPages ||
                        (numeroPagina >= paginacion.pagina - 1 &&
                          numeroPagina <= paginacion.pagina + 1)
                      ) {
                        return (
                          <button
                            key={numeroPagina}
                            className={`pag-num ${
                              paginacion.pagina === numeroPagina ? 'active' : ''
                            }`}
                            onClick={() => handleCambiarPagina(numeroPagina)}
                          >
                            {numeroPagina}
                          </button>
                        );
                      } else if (
                        numeroPagina === paginacion.pagina - 2 ||
                        numeroPagina === paginacion.pagina + 2
                      ) {
                        return <span key={numeroPagina}>...</span>;
                      }
                      return null;
                    })}
                  </div>

                  <button
                    className="pag-btn"
                    onClick={() => handleCambiarPagina(paginacion.pagina + 1)}
                    disabled={paginacion.pagina === paginacion.totalPages}
                  >
                    Siguiente →
                  </button>
                </div>
              )}
            </>
          )}
        </div>
      </main>

      <Footer />

      {noticiaSeleccionada && (
        <NoticiaDetalle
          noticia={noticiaSeleccionada}
          onClose={() => setNoticiaSeleccionada(null)}
        />
      )}
    </div>
  );
}

export default App;