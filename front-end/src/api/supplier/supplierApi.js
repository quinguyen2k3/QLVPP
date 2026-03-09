import { getFetcher, postFetcher, putFetcher} from "src/api/globalFetcher";

const api = import.meta.env.VITE_API_BASE_URL;

export const supplierApi = {
  getAll: () => getFetcher(`${api}/supplier`),
  getById: (id) => getFetcher(`${api}/supplier/${id}`),
  create: (data) => postFetcher(`${api}/supplier`, data),
  update: (id, data) => putFetcher(`${api}/supplier/${id}`, data),
};
