import { getFetcher, postFetcher, putFetcher } from "src/api/globalFetcher";

const api = import.meta.env.VITE_API_BASE_URL;

export const departmentApi = {
  getAll: () => getFetcher(`${api}/department`),
  getById: (id) => getFetcher(`${api}/department/${id}`),
  create: (data) => postFetcher(`${api}/department`, data),
  update : (id, data) => putFetcher(`${api}/department/${id}`, data),
};