import { getFetcher, postFetcher, putFetcher, deleteFetcher } from 'src/api/globalFetcher';

const api = import.meta.env.VITE_API_BASE_URL;

export const stockInApi = {
  getAll: () => getFetcher(`${api}/stockin/warehouse/all`),
  getPending: () => getFetcher(`${api}/stockin/warehouse/pending`),
  getMyStockIn: () => getFetcher(`${api}/stockin/my`),
  getById: (id) => getFetcher(`${api}/stockin/${id}`),
  getUsage: () => getFetcher(`${api}/stockin/usage/received`),
  getTransfer: () => getFetcher(`${api}/stockin/transfer/received`),
  create: (data) => postFetcher(`${api}/stockin`, data),
  update: (id, data) => putFetcher(`${api}/stockin/${id}`, data),
  delete: (id) => deleteFetcher(`${api}/stockin/${id}`),
  approve: (id) => putFetcher(`${api}/stockin/approve/${id}`),
  cancel: (id) => putFetcher(`${api}/stockin/cancel/${id}`),
};
