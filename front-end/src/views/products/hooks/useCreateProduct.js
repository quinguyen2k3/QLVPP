import { useState } from "react";
import { productApi } from "src/api/product/productApi";

export const useCreateProduct = () => {
  const [loading, setLoading] = useState(false);

  const createProduct = async (values) => {
    const formData = new FormData();

    Object.entries(values).forEach(([key, value]) => {
      if (value !== null && value !== undefined) {
        formData.append(key, value);
      }
    });

    setLoading(true);

    try {
      const res = await productApi.create(formData);

      return {
        success: true,
        message: res.message || "Tạo sản phẩm thành công",
        data: res.data,
      };
    } catch (err) {
      return {
        success: false,
        message: err.message || "Tạo sản phẩm thất bại",
        errors: err.errors || null,
        status: err.status,
      };
    } finally {
      setLoading(false);
    }
  };

  return { createProduct, loading };
};
