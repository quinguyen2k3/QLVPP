import { useState } from 'react';
import { productApi } from 'src/api/product/productApi';

export const useUpdateProduct = () => {
  const [loading, setLoading] = useState(false);

  const updateProduct = async (id, values) => {
    const formData = new FormData();

    Object.entries(values).forEach(([key, value]) => {
      if (value !== null && value !== undefined) {
        formData.append(key, value);
      }
    });

    setLoading(true);

    try {
      const res = await productApi.update(id, formData);

      return {
        success: true,
        message: res.message || 'Cập nhật sản phẩm thành công',
        data: res.data,
      };
    } catch (err) {
      return {
        success: false,
        message: err.message || 'Cập nhật sản phẩm thất bại',
        errors: err.errors || null,
        status: err.status,
      };
    } finally {
      setLoading(false);
    }
  };

  return { updateProduct, loading };
};
