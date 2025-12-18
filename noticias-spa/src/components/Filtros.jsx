import { useEffect, useState } from 'react';
import { categoriasService, paisesService } from '../services/api';

const Filtros = ({ onFiltrar, filtrosActivos }) => {
  const [categorias, setCategorias] = useState([]);
  const [paises, setPaises] = useState([]);

  useEffect(() => {
    cargarFiltros();
  }, []);

  const cargarFiltros = async () => {
    try {
      const [categoriasData, paisesData] = await Promise.all([
        categoriasService.obtenerTodas(),
        paisesService.obtenerTodos(),
      ]);
      setCategorias(categoriasData);
      setPaises(paisesData);
    } catch (error) {
      console.error('Error al cargar filtros:', error);
    }
  };

  const handleCategoriaClick = (categoriaId) => {
    onFiltrar({
      ...filtrosActivos,
      categoriaId: filtrosActivos.categoriaId === categoriaId ? null : categoriaId,
    });
  };

  const handlePaisClick = (paisId) => {
    onFiltrar({
      ...filtrosActivos,
      paisId: filtrosActivos.paisId === paisId ? null : paisId,
    });
  };

  const limpiarFiltros = () => {
    onFiltrar({ categoriaId: null, paisId: null, busqueda: '' });
  };

  const hayFiltrosActivos = filtrosActivos.categoriaId || filtrosActivos.paisId || filtrosActivos.busqueda;

  return (
    <div className="filtros" id="noticias">
      <div className="container">
        <div className="filtros-header">
          <h3>Filtrar Noticias</h3>
          {hayFiltrosActivos && (
            <button className="btn-limpiar" onClick={limpiarFiltros}>
              ✕ Limpiar filtros
            </button>
          )}
        </div>

        <div className="filtros-content">
          <div className="filtro-grupo">
            <h4>Por Categoría</h4>
            <div className="filtro-opciones">
              {categorias.map((cat) => (
                <button
                  key={cat.categoriaId}
                  className={`filtro-btn ${
                    filtrosActivos.categoriaId === cat.categoriaId ? 'active' : ''
                  }`}
                  onClick={() => handleCategoriaClick(cat.categoriaId)}
                >
                  {cat.nombre}
                </button>
              ))}
            </div>
          </div>

          <div className="filtro-grupo">
            <h4>Por País</h4>
            <div className="filtro-opciones">
              {paises.map((pais) => (
                <button
                  key={pais.paisId}
                  className={`filtro-btn ${
                    filtrosActivos.paisId === pais.paisId ? 'active' : ''
                  }`}
                  onClick={() => handlePaisClick(pais.paisId)}
                >
                  {pais.nombre}
                </button>
              ))}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Filtros;