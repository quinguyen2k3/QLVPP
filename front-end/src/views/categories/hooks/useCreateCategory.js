import { useState } from 'react';
import { categoryApi } from 'src/api/category/categoryApi';

export const useCreateCategory = () => {
  const [loading, setLoading] = useState(false);

  const createCategory = async (values) => {
    setLoading(true);

    try {
      const res = await categoryApi.create(values);

      // Trả về cả message và data
      return {
        success: true,
        message: res.message || 'Tạo danh mục thành công',
        data: res.data, // đây mới là category mới
      };
    } catch (err) {
      return {
        success: false,
        message: err.message || 'Tạo danh mục thất bại',
        errors: err.errors || null,
        data: null,
      };
    } finally {
      setLoading(false);
    }
  };

  return { createCategory, loading };
};
