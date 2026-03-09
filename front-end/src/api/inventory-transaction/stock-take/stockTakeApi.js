import { getFetcher, postFetcher, putFetcher, deleteFetcher } from 'src/api/globalFetcher';

const api = import.meta.env.VITE_API_BASE_URL;

export const stockTakeApi = {
  getAll: () => getFetcher(`${api}/stocktake/warehouse/all`),
  getMyStockTake: () => getFetcher(`${api}/stocktake/my`),
  getById: (id) => getFetcher(`${api}/stocktake/${id}`),
  create: (data) => postFetcher(`${api}/stocktake`, data),
  update: (id, data) => putFetcher(`${api}/stocktake/${id}`, data),
  approve: (id) => putFetcher(`${api}/stocktake/approve/${id}`),
  cancel: (id) => putFetcher(`${api}/stocktake/cancel/${id}`),
  delete: (id) => deleteFetcher(`${api}/stocktake/${id}`),
};
