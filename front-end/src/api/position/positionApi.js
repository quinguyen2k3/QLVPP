import { getFetcher, postFetcher, putFetcher } from 'src/api/globalFetcher';

const api = import.meta.env.VITE_API_BASE_URL;

export const positionApi = {
  getAll: () => getFetcher(`${api}/position`),
  getById: (id) => getFetcher(`${api}/position/${id}`),
  create: (data) => postFetcher(`${api}/position`, data),
  update: (id, data) => putFetcher(`${api}/position/${id}`, data),
};
