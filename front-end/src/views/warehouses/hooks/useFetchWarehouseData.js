import { useEffect, useState, useCallback } from 'react';
import { warehouseApi } from 'src/api/warehouse/warehouseApi';

export const useFetchWarehouseData = () => {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchWarehouses = useCallback(async () => {
    try {
      setLoading(true);
      const res = await warehouseApi.getAll();
      setData(res.data || []);
      setError(null);
    } catch (err) {
      console.error('Failed to fetch warehouses:', err);
      setError(err);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchWarehouses();
  }, [fetchWarehouses]);

  return {
    warehouses: data,
    loading,
    error,
    setWarehouses: setData,
  };
};
