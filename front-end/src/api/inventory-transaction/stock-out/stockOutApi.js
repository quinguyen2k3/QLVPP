import { getFetcher, postFetcher, putFetcher, deleteFetcher } from 'src/api/globalFetcher';

const api = import.meta.env.VITE_API_BASE_URL;

export const stockOutApi = {
  getAll: () => getFetcher(`${api}/stockout/warehouse/all`),
  getPending: () => getFetcher(`${api}/stockout/warehouse/pending`),
  getTransferIncoming: () => getFetcher(`${api}/stockout/transfer/incoming`),
  getTransferReceived: () => getFetcher(`${api}/stockout/transfer/received`),
  getIncomingForDept: () => getFetcher(`${api}/stockout/department/approved`),
  getUsage: () => getFetcher(`${api}/stockout/department/received`),
  getMyStockOut: () => getFetcher(`${api}/stockout/my`),
  getById: (id) => getFetcher(`${api}/stockout/${id}`),
  create: (data) => postFetcher(`${api}/stockout`, data),
  update: (id, data) => putFetcher(`${api}/stockout/${id}`, data),
  approve: (id) => putFetcher(`${api}/stockout/approve/${id}`),
  receive: (id) => putFetcher(`${api}/stockout/receive/${id}`),
  cancel: (id) => putFetcher(`${api}/stockout/cancel/${id}`),
  delete: (id) => deleteFetcher(`${api}/stockout/${id}`),
};
