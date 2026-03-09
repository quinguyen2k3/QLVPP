import { useState, useEffect } from 'react';
import { supplierApi } from 'src/api/supplier/supplierApi';

export const useGetOneSupplier = (id, enabled = true) => {
  const [supplier, setSupplier] = useState(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!id || !enabled) return;

    const fetchData = async () => {
      setLoading(true);
      try {
        const res = await supplierApi.getById(id);
        setSupplier(res.data);
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [id, enabled]);

  return { supplier, loading };
};
