import { useEffect, useState, useCallback } from 'react';
import { categoryApi } from 'src/api/category/categoryApi';

export const useFetchCategoryData = () => {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchCategories = useCallback(async () => {
    try {
      setLoading(true);
      const res = await categoryApi.getAll();
      setData(res.data || []);
      setError(null);
    } catch (err) {
      console.error('Failed to fetch categories:', err);
      setError(err);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchCategories();
  }, [fetchCategories]);

  return {
    categories: data,
    loading,
    error,
    setCategories: setData, 
  };
};
