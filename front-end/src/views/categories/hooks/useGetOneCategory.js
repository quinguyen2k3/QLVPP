import { useState, useEffect } from 'react';
import { categoryApi } from 'src/api/category/categoryApi';

export const useGetOneCategory = (id, enabled = true) => {
  const [category, setCategory] = useState(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!id || !enabled) return;

    const fetchData = async () => {
      setLoading(true);
      try {
        const res = await categoryApi.getById(id);
        setCategory(res.data);
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [id, enabled]);

  return { category, loading };
};
