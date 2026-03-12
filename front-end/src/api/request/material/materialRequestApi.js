import { getFetcher, postFetcher, putFetcher, deleteFetcher } from 'src/api/globalFetcher';

const api = import.meta.env.VITE_API_BASE_URL;

export const materialRequestApi = {
  getMyRequests: () => getFetcher(`${api}/materialrequest/my-requests`),
  getPendingByDepartment: () => getFetcher(`${api}/materialrequest/department/pending`),
  getPendingByWarehouse: () => getFetcher(`${api}/materialrequest/warehouse/pending`),
  getApprovedByWarehouse: () => getFetcher(`${api}/materialrequest/warehouse/approved`),
  getById: (id) => getFetcher(`${api}/materialrequest/${id}`),
  create: (data) => postFetcher(`${api}/materialrequest`, data),
  update: (id, data) => putFetcher(`${api}/materialrequest/${id}`, data),
  approve: (data) => putFetcher(`${api}/materialrequest/approve`, data),
  reject: (data) => putFetcher(`${api}/materialrequest/reject`, data),
  delegate: (data) => putFetcher(`${api}/materialrequest/delegate`, data),
  delete: (id) => deleteFetcher(`${api}/materialrequest/${id}`),
};
