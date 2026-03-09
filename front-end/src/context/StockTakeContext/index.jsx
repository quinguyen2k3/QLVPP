import { createContext, useState } from 'react';
import { stockTakeApi } from 'src/api/inventory-transaction/stock-take/stockTakeApi';

export const StockTakeContext = createContext(undefined);

export const StockTakeProvider = ({ children }) => {
  const [stockTakes, setStockTakes] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const clearError = () => setError(null);

  const processData = (rawData) => {
    const list = rawData?.data ?? rawData ?? [];
    return list;
  };

  const fetchAllStockTakes = async () => {
    setLoading(true);
    try {
      const res = await stockTakeApi.getAll();
      setStockTakes(processData(res));
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
      const res = await stockTakeApi.getMyStockTake();
      setStockTakes(processData(res));
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
      const res = await stockTakeApi.getPending();
      setStockTakes(processData(res));
    } catch (err) {
      console.error(err);
      setError(err);
    } finally {
      setLoading(false);
    }
  };

  const addStockTake = async (payload) => {
    try {
      await stockTakeApi.create(payload);
      return true;
    } catch (err) {
      console.error(err);
      setError(err);
      throw err;
    }
  };

  const updateStockTake = async (payload) => {
    if (!payload?.id) return false;
    try {
      await stockTakeApi.update(payload.id, payload);
      return true;
    } catch (err) {
      console.error(err);
      setError(err);
      return false;
    }
  };

  const deleteStockTake = async (id) => {
    if (!id) return;
    try {
      await stockTakeApi.delete(id);
      setStockTakes((prev) => prev.filter((item) => item.id !== id));
    } catch (err) {
      console.error(err);
      setError(err);
    }
  };

  const getStockTakeById = async (id) => {
    if (!id) return null;
    try {
      const res = await stockTakeApi.getById(id);
      return res?.data ?? res;
    } catch (err) {
      console.error(err);
      setError(err);
      return null;
    }
  };

  const approveStockTake = async (id) => {
    if (!id) return;
    try {
      await stockTakeApi.approve(id);
      setStockTakes((prev) =>
        prev.map((item) => (item.id === id ? { ...item, status: 'APPROVED' } : item)),
      );
    } catch (err) {
      console.error(err);
      setError(err);
      throw err;
    }
  };

  const cancelStockTake = async (id) => {
    if (!id) return false;
    try {
      await stockTakeApi.cancel(id);
      setStockTakes((prev) =>
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
    <StockTakeContext.Provider
      value={{
        stockTakes,
        loading,
        error,
        clearError,
        fetchAllStockTakes,
        fetchMyRequests,
        fetchToApprove,
        addStockTake,
        updateStockTake,
        deleteStockTake,
        getStockTakeById,
        approveStockTake,
        cancelStockTake,
      }}
    >
      {children}
    </StockTakeContext.Provider>
  );
};
