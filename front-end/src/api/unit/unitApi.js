import { getFetcher, postFetcher, putFetcher} from "src/api/globalFetcher";

const api = import.meta.env.VITE_API_BASE_URL;

export const unitApi = {
  getAll: () => getFetcher(`${api}/unit`),
  getById: (id) => getFetcher(`${api}/unit/${id}`),
  create: (data) => postFetcher(`${api}/unit`, data),
  update: (id, data) => putFetcher(`${api}/unit/${id}`, data),
};
