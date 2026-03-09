import { getFetcher, postFetcher, putFetcher } from "src/api/globalFetcher";

const api = import.meta.env.VITE_API_BASE_URL;

export const employeeApi = {
  getAll: () => getFetcher(`${api}/employee`),
  getById: (id) => getFetcher(`${api}/employee/${id}`),
  create: (data) => postFetcher(`${api}/employee`, data),
  update: (id, data) => putFetcher(`${api}/employee/${id}`, data),
};
