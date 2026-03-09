import { useEffect, useState, useCallback } from 'react';
import { supplierApi } from 'src/api/supplier/supplierApi';

export const useFetchSupplierData = () => {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchSuppliers = useCallback(async () => {
    try {
      setLoading(true);
      const res = await supplierApi.getAll();
      setData(res.data || []);
      setError(null);
    } catch (err) {
      console.error('Failed to fetch suppliers:', err);
      setError(err);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchSuppliers();
  }, [fetchSuppliers]);

  return {
    suppliers: data,
    loading,
    error,
    setSuppliers: setData, 
  };
};
