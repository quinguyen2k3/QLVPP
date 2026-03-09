import { createContext, useState } from 'react';
import { materialRequestApi } from 'src/api/request/material/materialRequestApi';

export const MaterialRequestContext = createContext(undefined);

export const MaterialRequestProvider = ({ children }) => {
  const [materialRequests, setMaterialRequests] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const processData = (rawData) => {
    const list = rawData?.data ?? rawData ?? [];
    return list;
  };

  const clearError = () => setError(null);

  const fetchMyRequests = async () => {
    setLoading(true);
    try {
      const res = await materialRequestApi.getMyRequests();
      setMaterialRequests(processData(res));
    } catch (err) {
      console.error(err);
      setError(err);
    } finally {
      setLoading(false);
    }
  };

  const fetchToApproveByDepartment = async () => {
    setLoading(true);
    try {
      const res = await materialRequestApi.getPendingByDepartment();
      setMaterialRequests(processData(res));
    } catch (err) {
      console.error(err);
      setError(err);
    } finally {
      setLoading(false);
    }
  };

  const fetchToApproveByWarehouse = async () => {
    setLoading(true);
    try {
      const res = await materialRequestApi.getPendingByWarehouse();
      setMaterialRequests(processData(res));
    } catch (err) {
      console.error(err);
      setError(err);
    } finally {
      setLoading(false);
    }
  };

  const addMaterialRequest = async (payload) => {
    try {
      await materialRequestApi.create(payload);
    } catch (err) {
      console.error(err);
      setError(err);
      throw err;
    }
  };

  const updateMaterialRequest = async (payload) => {
    if (!payload?.id) return;
    try {
      await materialRequestApi.update(payload.id, payload);
      return true;
    } catch (err) {
      console.error(err);
      setError(err);
      return false;
    }
  };

  const deleteMaterialRequest = async (id) => {
    if (!id) return;
    try {
      await materialRequestApi.delete(id);
      setMaterialRequests((prev) => prev.filter((item) => item.id !== id));
      return true;
    } catch (err) {
      console.error(err);
      setError(err);
      return false;
    }
  };

  const getMaterialRequestById = async (id) => {
    if (!id) return null;
    try {
      const res = await materialRequestApi.getById(id);
      return res?.data ?? res;
    } catch (err) {
      console.error(err);
      setError(err);
      return null;
    }
  };

  const approveMaterialRequest = async (id, comment) => {
    setLoading(true);
    try {
      const payload = {
        requestId: id,
        comment: comment || '',
      };

      await materialRequestApi.approve(payload);

      setMaterialRequests((prev) =>
        prev.map((item) => {
          if (item.id === id) {
            let newStatus = 'Approved';
            const currentStatus = item.status?.toLowerCase() || '';

            if (currentStatus === 'pending_department') {
              newStatus = 'Pending_Warehouse';
            } else if (currentStatus === 'pending_warehouse') {
              newStatus = 'Approved';
            }

            return { ...item, status: newStatus };
          }
          return item;
        }),
      );

      return true;
    } catch (err) {
      setError(err);
      return false;
    } finally {
      setLoading(false);
    }
  };

  const cancelMaterialRequest = async (id, comment) => {
    setLoading(true);
    try {
      const payload = {
        requestId: id,
        comment: comment || '',
      };

      await materialRequestApi.reject(payload);

      setMaterialRequests((prev) =>
        prev.map((item) => (item.id === id ? { ...item, status: 'Rejected' } : item)),
      );

      return true;
    } catch (err) {
      setError(err);
      return false;
    } finally {
      setLoading(false);
    }
  };

  const delegateMaterialRequest = async (id, delegateToUserId, comment) => {
    setLoading(true);
    try {
      const payload = {
        requestId: id,
        delegateToId : delegateToUserId,
        comment: comment || '',
      };

      await materialRequestApi.delegate(payload);

      return true;
    } catch (err) {
      setError(err);
      return false;
    } finally {
      setLoading(false);
    }
  };

  return (
    <MaterialRequestContext.Provider
      value={{
        materialRequests,
        loading,
        error,
        clearError,
        fetchMyRequests,
        fetchToApproveByDepartment,
        fetchToApproveByWarehouse,
        addMaterialRequest,
        updateMaterialRequest,
        deleteMaterialRequest,
        getMaterialRequestById,
        approveMaterialRequest,
        cancelMaterialRequest,
        delegateMaterialRequest,
      }}
    >
      {children}
    </MaterialRequestContext.Provider>
  );
};
