import useSWR from 'swr';
import { unitApi } from 'src/api/unit/unitApi';
import { employeeApi } from 'src/api/employee/employeeApi';
import { warehouseApi } from 'src/api/warehouse/warehouseApi';
import { departmentApi } from 'src/api/department/departmentApi';
import { categoryApi } from 'src/api/category/categoryApi';
import { supplierApi } from 'src/api/supplier/supplierApi';
import { productApi } from 'src/api/product/productApi';
import { positionApi } from 'src/api/position/positionApi';

const masterDataConfig = {
  revalidateOnFocus: false,
  dedupingInterval: 60 * 60 * 1000,
  shouldRetryOnError: false,
};

const createCacheHandlers = (mutate, idField = 'id') => {
  return {
    addLocal: (newItem) => {
      if (!newItem) return;
      mutate((currentData) => {
        const currentList = currentData?.data || [];
        return { ...currentData, data: [newItem, ...currentList] };
      }, false);
    },

    updateLocal: (updatedItem) => {
      if (!updatedItem) return;
      mutate((currentData) => {
        const currentList = currentData?.data || [];
        const newList = currentList.map((item) =>
          item[idField] === updatedItem[idField] ? updatedItem : item,
        );
        return { ...currentData, data: newList };
      }, false);
    },

    removeLocal: (itemId) => {
      if (!itemId) return;
      mutate((currentData) => {
        const currentList = currentData?.data || [];
        const newList = currentList.filter((item) => item[idField] !== itemId);
        return { ...currentData, data: newList };
      }, false);
    },

    refresh: () => mutate(),
  };
};

export const useUnits = () => {
  const { data, error, isLoading, mutate } = useSWR('/units', unitApi.getAll, masterDataConfig);
  const handlers = createCacheHandlers(mutate);
  return { units: data?.data || [], isLoading, isError: error, ...handlers };
};

export const useEmployees = () => {
  const { data, error, isLoading, mutate } = useSWR(
    '/employees',
    employeeApi.getAll,
    masterDataConfig,
  );
  const handlers = createCacheHandlers(mutate);
  return { employees: data?.data || [], isLoading, isError: error, ...handlers };
};

export const useWarehouses = () => {
  const { data, error, isLoading, mutate } = useSWR(
    '/warehouses',
    warehouseApi.getAll,
    masterDataConfig,
  );
  const handlers = createCacheHandlers(mutate);
  return { warehouses: data?.data || [], isLoading, isError: error, ...handlers };
};

export const usePositions = () => {
  const { data, error, isLoading, mutate } = useSWR(
    '/positions',
    positionApi.getAll,
    masterDataConfig,
  );
  const handlers = createCacheHandlers(mutate);
  return { positions: data?.data || [], isLoading, isError: error, ...handlers };
};

export const useDepartments = () => {
  const { data, error, isLoading, mutate } = useSWR(
    '/departments',
    departmentApi.getAll,
    masterDataConfig,
  );
  const handlers = createCacheHandlers(mutate);
  return { departments: data?.data || [], isLoading, isError: error, ...handlers };
};

export const useCategories = () => {
  const { data, error, isLoading, mutate } = useSWR(
    '/categories',
    categoryApi.getAll,
    masterDataConfig,
  );
  const handlers = createCacheHandlers(mutate);
  return { categories: data?.data || [], isLoading, isError: error, ...handlers };
};

export const useSuppliers = () => {
  const { data, error, isLoading, mutate } = useSWR(
    '/suppliers',
    supplierApi.getAll,
    masterDataConfig,
  );
  const handlers = createCacheHandlers(mutate);
  return { suppliers: data?.data || [], isLoading, isError: error, ...handlers };
};

export const useProducts = (scope = 'global', warehouseId = null) => {
  let swrKey = null;

  if (scope === 'global') {
    swrKey = '/products/global';
  } else if (scope === 'local' && warehouseId) {
    swrKey = ['/products/warehouse', warehouseId];
  }

  const fetcher = async () => {
    if (scope === 'local' && warehouseId) {
      return await productApi.getByWarehouse(warehouseId);
    }
    return await productApi.getAll();
  };

  const { data, error, isLoading, mutate } = useSWR(swrKey, fetcher, masterDataConfig);
  const handlers = createCacheHandlers(mutate);

  return {
    products: data?.data || [],
    isLoading,
    isError: error,
    ...handlers,
  };
};

export const useMasterData = () => {
  const { units } = useUnits();
  const { employees } = useEmployees();
  const { warehouses } = useWarehouses();
  const { departments } = useDepartments();
  const { categories } = useCategories();
  const { suppliers } = useSuppliers();
  const { positions } = usePositions();
  const { products } = useProducts('global');

  return {
    units,
    requesters: employees,
    employees,
    warehouses,
    departments,
    categories,
    suppliers,
    products,
    positions,
  };
};
