import { getFetcher } from 'src/api/globalFetcher';

const api = import.meta.env.VITE_API_BASE_URL;

export const roleApi = {
  getAll: () => getFetcher(`${api}/role`),
};
