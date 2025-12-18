import { useState } from 'react';

const Header = () => {
  const [menuAbierto, setMenuAbierto] = useState(false);

  const toggleMenu = () => setMenuAbierto(!menuAbierto);
  const cerrarMenu = () => setMenuAbierto(false);

  return (
    <>
      <header className="header">
        <div className="container">
          <div className="header-content">
            <div className="logo">
              <h1>WeverNews</h1>
            </div>

            <button 
              className={`menu-toggle ${menuAbierto ? 'active' : ''}`}
              onClick={toggleMenu}
              aria-label="Menú"
            >
              <span></span>
              <span></span>
              <span></span>
            </button>

            <nav className={`nav ${menuAbierto ? 'nav-open' : ''}`}>
              <a href="#inicio" onClick={cerrarMenu}>Inicio</a>
              <a href="#noticias" onClick={cerrarMenu}>Noticias</a>
              <a href="#destacadas" onClick={cerrarMenu}>Destacadas</a>
            </nav>
          </div>
        </div>
      </header>

      {menuAbierto && <div className="overlay" onClick={cerrarMenu}></div>}
    </>
  );
};

export default Header;