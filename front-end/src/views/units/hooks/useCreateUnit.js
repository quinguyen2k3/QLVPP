import { useState } from 'react';
import { unitApi } from 'src/api/unit/unitApi';

export const useCreateUnit = () => {
  const [loading, setLoading] = useState(false);

  const createUnit = async (values) => {
    setLoading(true);

    try {
      const res = await unitApi.create(values);

      return {
        success: true,
        message: res.message || 'Tạo đơn vị thành công',
        data: res.data,
      };
    } catch (err) {
      return {
        success: false,
        message: err.message || 'Tạo đơn vị thất bại',
        errors: err.errors || null,
        data: null,
      };
    } finally {
      setLoading(false);
    }
  };

  return { createUnit, loading };
};
