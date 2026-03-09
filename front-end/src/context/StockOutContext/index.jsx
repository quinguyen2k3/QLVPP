import { createContext, useMemo, useState } from 'react';
import useSWR from 'swr';
import { stockOutApi } from 'src/api/inventory-transaction/stock-out/stockOutApi';
import { warehouseApi } from 'src/api/warehouse/warehouseApi';
import { departmentApi } from 'src/api/department/departmentApi';

export const StockOutContext = createContext(undefined);

export const StockOutProvider = ({ children }) => {
  const [stockOuts, setStockOuts] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const clearError = () => setError(null);

  const processData = (rawData) => {
    const list = rawData?.data ?? rawData ?? [];
    return list;
  };

  const fetchAllStockOuts = async () => {
    setLoading(true);
    try {
      const res = await stockOutApi.getAll();
      setStockOuts(processData(res));
    } catch (err) {
      console.error(err);
      setError(err);
    } finally {
      setLoading(false);
    }
  };

  const fetchMyRequests = async () => {
    setLoading(true);
    try {
      const res = await stockOutApi.getMyStockOut();
      setStockOuts(processData(res));
    } catch (err) {
      console.error(err);
      setError(err);
    } finally {
      setLoading(false);
    }
  };

  const fetchToApprove = async () => {
    setLoading(true);
    try {
      const res = await stockOutApi.getPending();
      setStockOuts(processData(res));
    } catch (err) {
      console.error(err);
      setError(err);
    } finally {
      setLoading(false);
    }
  };

  const { data: warehouseRes } = useSWR('/warehouses', warehouseApi.getAll);
  const { data: departmentRes } = useSWR('/departments', departmentApi.getAll);

  const warehouses = warehouseRes?.data ?? warehouseRes ?? [];
  const departments = departmentRes?.data ?? departmentRes ?? [];

  const warehouseMap = useMemo(
    () => Object.fromEntries(warehouses.map((w) => [w.id, w.name])),
    [warehouses],
  );

  const departmentMap = useMemo(
    () => Object.fromEntries(departments.map((d) => [d.id, d.name])),
    [departments],
  );

  const addStockOut = async (payload) => {
    try {
      await stockOutApi.create(payload);
    } catch (err) {
      console.error(err);
      setError(err);
      throw err;
    }
  };

  const updateStockOut = async (payload) => {
    if (!payload?.id) return;
    try {
      await stockOutApi.update(payload.id, payload);
      return true;
    } catch (err) {
      console.error(err);
      setError(err);
      return false;
    }
  };

  const deleteStockOut = async (id) => {
    if (!id) return;
    try {
      await stockOutApi.delete(id);
      setStockOuts((prev) => prev.filter((item) => item.id !== id));
    } catch (err) {
      console.error(err);
      setError(err);
    }
  };

  const getStockOutById = async (id) => {
    if (!id) return null;
    try {
      const res = await stockOutApi.getById(id);
      return res?.data ?? res;
    } catch (err) {
      console.error(err);
      setError(err);
      return null;
    }
  };

  const approveStockOut = async (id) => {
    if (!id) return;
    try {
      await stockOutApi.approve(id);
      setStockOuts((prev) =>
        prev.map((item) => (item.id === id ? { ...item, status: 'APPROVED' } : item)),
      );
    } catch (err) {
      console.error(err);
      setError(err);
      throw err;
    }
  };

  const cancelStockOut = async (id) => {
    if (!id) return;
    try {
      await stockOutApi.cancel(id);
      setStockOuts((prev) =>
        prev.map((item) => (item.id === id ? { ...item, status: 'CANCELLED' } : item)),
      );
      return true;
    } catch (err) {
      console.error(err);
      setError(err);
      throw err;
    }
  };

  return (
    <StockOutContext.Provider
      value={{
        stockOuts,
        loading,
        error,
        clearError,
        warehouseMap,
        departmentMap,
        fetchAllStockOuts,
        fetchMyRequests,
        fetchToApprove,
        addStockOut,
        updateStockOut,
        deleteStockOut,
        getStockOutById,
        approveStockOut,
        cancelStockOut,
      }}
    >
      {children}
    </StockOutContext.Provider>
  );
};
