import { getFetcher, postFetcher, putFetcher } from "src/api/globalFetcher";

const api = import.meta.env.VITE_API_BASE_URL;

export const categoryApi = {
  getAll: () => getFetcher(`${api}/category`),
  getById: (id) => getFetcher(`${api}/category/${id}`),
  create: (data) => postFetcher(`${api}/category`, data),
  update: (id, data) => putFetcher(`${api}/category/${id}`, data),
};
