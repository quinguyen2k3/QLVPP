import { getFetcher, postFetcher, putFetcher } from 'src/api/globalFetcher';

const api = import.meta.env.VITE_API_BASE_URL;

export const inventoryApi = {
  getMyWarehouseSnapshots: () => getFetcher(`${api}/inventory/snapshot/my-warehouse`),
  getById: (id) => getFetcher(`${api}/inventory/snapshot/${id}`),
  create: (data) => postFetcher(`${api}/inventory/snapshot`, data),
};
