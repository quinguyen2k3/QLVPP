import { useState } from 'react';
import { warehouseApi } from 'src/api/warehouse/warehouseApi';

export const useUpdateWarehouse = () => {
  const [loading, setLoading] = useState(false);

  const updateWarehouse = async (id, values) => {
    setLoading(true);

    try {
      const res = await warehouseApi.update(id, values);
      return {
        success: true,
        message: res.message || 'Cập nhật kho thành công',
        data: res.data,
      };
    } catch (err) {
      return {
        success: false,
        message: err.message || 'Cập nhật kho thất bại',
        errors: err.errors || null,
        status: err.status,
      };
    } finally {
      setLoading(false);
    }
  };

  return { updateWarehouse, loading };
};
