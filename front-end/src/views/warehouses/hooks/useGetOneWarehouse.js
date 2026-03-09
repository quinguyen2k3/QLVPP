import { useState, useEffect } from 'react';
import { warehouseApi } from 'src/api/warehouse/warehouseApi';

export const useGetOneWarehouse = (id, enabled = true) => {
  const [warehouse, setWarehouse] = useState(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!id || !enabled) return;

    const fetchData = async () => {
      setLoading(true);
      try {
        const res = await warehouseApi.getById(id);
        setWarehouse(res.data);
      } catch (err) {
        console.error('Failed to fetch warehouse:', err);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [id, enabled]);

  return { warehouse, loading };
};
