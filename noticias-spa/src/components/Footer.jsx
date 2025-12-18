const Footer = () => {
  return (
    <footer className="footer">
      <div className="container">
        <div className="footer-content">
          <div className="footer-section">
            <h3>Portal de Noticias WeverNews</h3>
            <p>Tu fuente confiable de información</p>
          </div>
          
          <div className="footer-section">
            <h4>Enlaces</h4>
            <ul>
              <li><a href="#inicio">Inicio</a></li>
              <li><a href="#noticias">Noticias</a></li>
              <li><a href="#destacadas">Destacadas</a></li>
            </ul>
          </div>
          
          <div className="footer-section">
            <h4>Contacto</h4>
            <p>info@wevernews.com</p>
            <p>+1 (809) 555-0123</p>
          </div>
        </div>
        
        <div className="footer-bottom">
          <p>&copy; 2025 Portal de Noticias WeverNews. Todos los derechos reservados.</p>
        </div>
      </div>
    </footer>
  );
};

export default Footer;