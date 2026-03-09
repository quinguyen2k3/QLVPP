import { getFetcher, postFetcher, putFetcher } from "src/api/globalFetcher";

const api = import.meta.env.VITE_API_BASE_URL;

export const warehouseApi = {
  getAll: () => getFetcher(`${api}/warehouse`),
  getById: (id) => getFetcher(`${api}/warehouse/${id}`),
  create: (data) => postFetcher(`${api}/warehouse`, data),
  update : (id, data) => putFetcher(`${api}/warehouse/${id}`, data),
};