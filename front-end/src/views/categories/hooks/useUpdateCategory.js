import { useState } from 'react';
import { categoryApi } from 'src/api/category/categoryApi';

export const useUpdateCategory = () => {
  const [loading, setLoading] = useState(false);

  const updateCategory = async (id, values) => {
    setLoading(true);

    try {
      const res = await categoryApi.update(id, values);

      return {
        success: true,
        message: res.message || 'Cập nhật danh mục thành công',
        data: res.data,
      };
    } catch (err) {
      return {
        success: false,
        message: err.message || 'Cập nhật danh mục thất bại',
        errors: err.errors || null,
        status: err.status,
      };
    } finally {
      setLoading(false);
    }
  };

  return { updateCategory, loading };
};
