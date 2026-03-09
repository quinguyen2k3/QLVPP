import { useState } from 'react';
import { supplierApi } from 'src/api/supplier/supplierApi';

export const useUpdateSupplier = () => {
  const [loading, setLoading] = useState(false);

  const updateSupplier = async (id, values) => {
    setLoading(true);

    try {
      const res = await supplierApi.update(id, values);
      return {
        success: true,
        message: res.message || 'Cập nhật nhà cung cấp thành công',
        data: res.data,
      };
    } catch (err) {
      return {
        success: false,
        message: err.message || 'Cập nhật nhà cung cấp thất bại',
        errors: err.errors || null,
        status: err.status,
      };
    } finally {
      setLoading(false);
    }
  };

  return { updateSupplier, loading };
};
