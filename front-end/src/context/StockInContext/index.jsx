import { createContext, useMemo, useState } from 'react';
import useSWR from 'swr';
import { stockInApi } from 'src/api/inventory-transaction/stock-in/stockInApi';
import { supplierApi } from 'src/api/supplier/supplierApi';
import { warehouseApi } from 'src/api/warehouse/warehouseApi';

export const StockInContext = createContext(undefined);

export const StockInProvider = ({ children }) => {
  const [stockIns, setStockIns] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const clearError = () => setError(null);

  const processData = (rawData) => {
    const list = rawData?.data ?? rawData ?? [];
    return list; 
  };

  const fetchAllStockIns = async () => {
    setLoading(true);
    try {
      const res = await stockInApi.getAll();
      setStockIns(processData(res));
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
      const res = await stockInApi.getMyStockIn();
      setStockIns(processData(res));
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
      const res = await stockInApi.getPending();
      setStockIns(processData(res));
    } catch (err) {
      console.error(err);
      setError(err);
    } finally {
      setLoading(false);
    }
  };

  const { data: supplierRes } = useSWR('/suppliers', supplierApi.getAll);
  const { data: warehouseRes } = useSWR('/warehouses', warehouseApi.getAll);

  const suppliers = supplierRes?.data ?? supplierRes ?? [];
  const warehouses = warehouseRes?.data ?? warehouseRes ?? [];

  const supplierMap = useMemo(
    () => Object.fromEntries(suppliers.map((s) => [s.id, s.name])),
    [suppliers],
  );

  const warehouseMap = useMemo(
    () => Object.fromEntries(warehouses.map((w) => [w.id, w.name])),
    [warehouses],
  );

  const addStockIn = async (payload) => {
    try {
      await stockInApi.create(payload);
    } catch (err) {
      console.error(err);
      setError(err);
      throw err;
    }
  };

  const updateStockIn = async (payload) => {
    if (!payload?.id) return;
    try {
      await stockInApi.update(payload.id, payload);
      return true;
    } catch (err) {
      console.error(err);
      setError(err);
      return false;
    }
  };

  const deleteStockIn = async (id) => {
    if (!id) return;
    try {
      await stockInApi.delete(id);
      setStockIns((prev) => prev.filter((item) => item.id !== id));
    } catch (err) {
      console.error(err);
      setError(err);
    }
  };

  const getStockInById = async (id) => {
    if (!id) return null;
    try {
      const res = await stockInApi.getById(id);
      return res?.data ?? res;
    } catch (err) {
      console.error(err);
      setError(err);
      return null;
    }
  };

  const approveStockIn = async (id) => {
    if (!id) return;
    try {
      await stockInApi.approve(id);
      setStockIns((prev) =>
        prev.map((item) => (item.id === id ? { ...item, status: 'APPROVED' } : item)),
      );
    } catch (err) {
      console.error(err);
      setError(err);
      throw err;
    }
  };

  const cancelStockIn = async (id) => {
    if (!id) return;
    try {
      await stockInApi.cancel(id);
      setStockIns((prev) =>
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
    <StockInContext.Provider
      value={{
        stockIns,
        loading,
        error,
        clearError,
        supplierMap,
        warehouseMap,
        fetchAllStockIns,
        fetchMyRequests,
        fetchToApprove,
        addStockIn,
        updateStockIn,
        deleteStockIn,
        getStockInById,
        approveStockIn,
        cancelStockIn,
      }}
    >
      {children}
    </StockInContext.Provider>
  );
};