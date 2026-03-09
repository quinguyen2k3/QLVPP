import { useState, useEffect } from "react";
import { productApi } from "src/api/product/productApi";

/**
 * Hook: Lấy thông tin một sản phẩm theo id
 * @param {string|number} id
 */
export const useGetOneProduct = (id) => {
  const [loading, setLoading] = useState(false);
  const [product, setProduct] = useState(null);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (!id) return;

    const fetchProduct = async () => {
      setLoading(true);
      setError(null);
      try {
        const res = await productApi.getById(id);
        setProduct(res.data);
      } catch (err) {
        setError(err);
        setProduct(null);
      } finally {
        setLoading(false);
      }
    };

    fetchProduct();
  }, [id]);

  return { product, loading, error };
};
