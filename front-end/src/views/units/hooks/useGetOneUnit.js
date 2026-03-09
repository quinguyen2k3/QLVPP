import { useState, useEffect } from 'react';
import { unitApi } from 'src/api/unit/unitApi';

export const useGetOneUnit = (id, enabled = true) => {
  const [unit, setUnit] = useState(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!id || !enabled) return;

    const fetchData = async () => {
      setLoading(true);
      try {
        const res = await unitApi.getById(id);
        setUnit(res.data);
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [id, enabled]);

  return { unit, loading };
};
