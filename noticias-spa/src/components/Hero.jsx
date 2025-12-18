import { useState } from 'react';

const Hero = ({ onSearch }) => {
  const [busqueda, setBusqueda] = useState('');

  const handleSubmit = (e) => {
    e.preventDefault();
    onSearch(busqueda);
  };

  return (
    <section className="hero" id="inicio">
      <div className="container">
        <div className="hero-content">
          <h2 className="hero-title">Mantente Informado</h2>
          <p className="hero-subtitle">
            Las últimas noticias de República Dominicana y el mundo
          </p>
          <form className="search-form" onSubmit={handleSubmit}>
            <div className="search-box">
              <input
                type="text"
                className="search-input"
                placeholder="Buscar noticias..."
                value={busqueda}
                onChange={(e) => setBusqueda(e.target.value)}
              />
              <button type="submit" className="search-btn">
                Buscar
              </button>
            </div>
          </form>
        </div>
      </div>
    </section>
  );
};

export default Hero;