import { useState } from 'react';
import { supplierApi } from 'src/api/supplier/supplierApi';

export const useCreateSupplier = () => {
  const [loading, setLoading] = useState(false);

  const createSupplier = async (values) => {
    setLoading(true);

    try {
      const res = await supplierApi.create(values);
      return {
        success: true,
        message: res.message || 'Tạo nhà cung cấp thành công',
        data: res.data,
      };
    } catch (err) {
      return {
        success: false,
        message: err.message || 'Tạo nhà cung cấp thất bại',
        errors: err.errors || null,
        data: null,
      };
    } finally {
      setLoading(false);
    }
  };

  return { createSupplier, loading };
};
