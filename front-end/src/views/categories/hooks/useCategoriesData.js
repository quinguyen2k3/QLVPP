import { categoryApi } from 'src/api/category/categoryApi';

/**
 * Lấy tất cả danh mục
 * @returns Promise<Array>
 */
export const fetchAllCategory = async () => {
  try {
    const data = await categoryApi.getAll();
    return data.data;
  } catch (err) {
    console.error('Failed to fetch categories:', err);
    return [];
  }
};

