import { useState } from 'react';
import { unitApi } from 'src/api/unit/unitApi';

export const useUpdateUnit = () => {
  const [loading, setLoading] = useState(false);

  const updateUnit = async (id, values) => {
    setLoading(true);

    try {
      const res = await unitApi.update(id, values);
      return {
        success: true,
        message: res.message || 'Cập nhật đơn vị thành công',
        data: res.data,
      };
    } catch (err) {
      return {
        success: false,
        message: err.message || 'Cập nhật đơn vị thất bại',
        errors: err.errors || null,
        status: err.status,
      };
    } finally {
      setLoading(false);
    }
  };

  return { updateUnit, loading };
};
