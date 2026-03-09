import { useState } from 'react';
import { warehouseApi } from 'src/api/warehouse/warehouseApi';

export const useCreateWarehouse = () => {
  const [loading, setLoading] = useState(false);

  const createWarehouse = async (values) => {
    setLoading(true);

    try {
      const res = await warehouseApi.create(values);

      return {
        success: true,
        message: res.message || 'Tạo kho thành công',
        data: res.data,
      };
    } catch (err) {
      return {
        success: false,
        message: err.message || 'Tạo kho thất bại',
        errors: err.errors || null,
        data: null,
      };
    } finally {
      setLoading(false);
    }
  };

  return { createWarehouse, loading };
};
