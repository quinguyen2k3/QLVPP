import { getFetcher, postFetcher, putFetcher } from "src/api/globalFetcher";

const api = import.meta.env.VITE_API_BASE_URL;

export const productApi = {
  getAll: () => getFetcher(`${api}/product`),
  getIsActivated: () => getFetcher(`${api}/product?activated=true`),
  getByWarehouse: (id) => getFetcher(`${api}/product/warehouse/${id}`),
  getById: (id) => getFetcher(`${api}/product/${id}`),
  getIsAsset: () => getFetcher(`${api}/product/type?isAsset=true`),
  create: (data) => postFetcher(`${api}/product`, data),
  update: (id, data) => putFetcher(`${api}/product/${id}`, data),
};
