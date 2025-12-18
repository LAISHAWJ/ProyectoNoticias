import axios from 'axios';

const API_BASE_URL = 'https://localhost:7231/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const noticiasService = {
  obtenerNoticias: async (params = {}) => {
    try {
      const response = await api.get('/Noticias', { params });
      return {
        data: response.data,
        totalCount: parseInt(response.headers['x-total-count'] || '0'),
        totalPages: parseInt(response.headers['x-total-pages'] || '1'),
        currentPage: parseInt(response.headers['x-page'] || '1'),
      };
    } catch (error) {
      console.error('Error al obtener noticias:', error);
      throw error;
    }
  },

  obtenerNoticiaPorId: async (id) => {
    try {
      const response = await api.get(`/Noticias/${id}`);
      return response.data;
    } catch (error) {
      console.error(`Error al obtener noticia ${id}:`, error);
      throw error;
    }
  },

  obtenerDestacadas: async (cantidad = 6) => {
    try {
      const response = await api.get('/Noticias/Destacadas', {
        params: { cantidad },
      });
      return response.data;
    } catch (error) {
      console.error('Error al obtener noticias destacadas:', error);
      throw error;
    }
  },
};

export const categoriasService = {
  obtenerTodas: async () => {
    try {
      const response = await api.get('/Categorias');
      return response.data;
    } catch (error) {
      console.error('Error al obtener categorías:', error);
      throw error;
    }
  },
};

export const paisesService = {
  obtenerTodos: async () => {
    try {
      const response = await api.get('/Paises');
      return response.data;
    } catch (error) {
      console.error('Error al obtener países:', error);
      throw error;
    }
  },
};

export default api;