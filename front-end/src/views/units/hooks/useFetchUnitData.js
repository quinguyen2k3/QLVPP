import { useEffect, useState, useCallback } from 'react';
import { unitApi } from 'src/api/unit/unitApi';

export const useFetchUnitData = () => {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchUnits = useCallback(async () => {
    try {
      setLoading(true);
      const res = await unitApi.getAll();
      setData(res.data || []);
      setError(null);
    } catch (err) {
      console.error('Failed to fetch units:', err);
      setError(err);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchUnits();
  }, [fetchUnits]);

  return {
    units: data,
    loading,
    error,
    setUnits: setData, 
  };
};
