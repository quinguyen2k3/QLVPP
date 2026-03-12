import React, { lazy } from 'react';
import { Navigate, createBrowserRouter } from 'react-router';
import Loadable from '../layouts/full/shared/loadable/Loadable';
import ProtectedRoute from './ProtectedRoute';

const FullLayout = Loadable(lazy(() => import('../layouts/full/FullLayout')));
const BlankLayout = Loadable(lazy(() => import('../layouts/blank/BlankLayout')));

const ModernDash = Loadable(lazy(() => import('../views/dashboard/Modern')));
const EcommerceDash = Loadable(lazy(() => import('../views/dashboard/Ecommerce')));

const Login = Loadable(lazy(() => import('../views/authentication/auth1/Login')));
const Error = Loadable(lazy(() => import('../views/authentication/Error')));
const Forbidden = Loadable(lazy(() => import('../views/authentication/Forbidden')));

const ProductList = Loadable(lazy(() => import('../views/products/list/page')));
const AddProduct = Loadable(lazy(() => import('../views/products/add/page')));
const EditProduct = Loadable(lazy(() => import('../views/products/edit/page')));

const CategoryList = Loadable(lazy(() => import('../views/categories/list/page')));
const UnitList = Loadable(lazy(() => import('../views/units/list/page')));
const SupplierList = Loadable(lazy(() => import('../views/suppliers/list/page')));
const WarehouseList = Loadable(lazy(() => import('../views/warehouses/list/page')));

const CreateStockIn = Loadable(
  lazy(() => import('../views/inventory-transaction/stock-in/add/page')),
);
const StockInListing = Loadable(
  lazy(() => import('../views/inventory-transaction/stock-in/list/page')),
);
const EditStockIn = Loadable(
  lazy(() => import('../views/inventory-transaction/stock-in/edit/page')),
);
const StockInDetail = Loadable(
  lazy(() => import('../views/inventory-transaction/stock-in/detail/page')),
);

const CreateStockOut = Loadable(
  lazy(() => import('../views/inventory-transaction/stock-out/add/page')),
);
const EditStockOut = Loadable(
  lazy(() => import('../views/inventory-transaction/stock-out/edit/page')),
);
const StockOutDetail = Loadable(
  lazy(() => import('../views/inventory-transaction/stock-out/detail/page')),
);
const StockOutListing = Loadable(
  lazy(() => import('../views/inventory-transaction/stock-out/list/page')),
);

const ReceiveTransferList = Loadable(
  lazy(() => import('../views/inventory-transaction/transfer/list/page')),
);

const InventoryDepartmentPage = Loadable(
  lazy(() => import('../views/my-work-space/inventory/page')),
);
const IncomingStockPage = Loadable(lazy(() => import('../views/my-work-space/incoming/page')));

const SnapshotList = Loadable(lazy(() => import('../views/snapshot/list/page')));
const DepartmentList = Loadable(lazy(() => import('../views/department/page')));
const PositionList = Loadable(lazy(() => import('../views/position/page')));
const EmployeeList = Loadable(lazy(() => import('../views/employee/page')));

const StockTakeCreate = Loadable(
  lazy(() => import('../views/inventory-transaction/stock-take/add/page')),
);
const StockTakeList = Loadable(
  lazy(() => import('../views/inventory-transaction/stock-take/list/page')),
);
const EditStockTake = Loadable(
  lazy(() => import('../views/inventory-transaction/stock-take/edit/page')),
);
const StockTakeDetail = Loadable(
  lazy(() => import('../views/inventory-transaction/stock-take/detail/page')),
);

const MaterialRequestCreate = Loadable(lazy(() => import('../views/request/material/add/page')));
const MaterialRequestListing = Loadable(lazy(() => import('../views/request/material/list/page')));
const EditMaterialRequest = Loadable(lazy(() => import('../views/request/material/edit/page')));
const MaterialRequestDetail = Loadable(lazy(() => import('../views/request/material/detail/page')));
const ChangePasswordPage = Loadable(lazy(() => import('../views/change-password/page')));

const Router = [
  {
    path: '/',
    element: (
      <ProtectedRoute>
        <FullLayout />
      </ProtectedRoute>
    ),
    children: [
      { path: '/', element: <Navigate to="/dashboards/modern" /> },
      { path: '/dashboards/modern', exact: true, element: <ModernDash /> },
      { path: '/dashboards/ecommerce', exact: true, element: <EcommerceDash /> },

      {
        path: '/catalog/products',
        element: (
          <ProtectedRoute allowedRoles={['Metadata Manager']}>
            <ProductList />
          </ProtectedRoute>
        ),
      },
      {
        path: '/catalog/product/add',
        element: (
          <ProtectedRoute allowedRoles={['Metadata Manager']}>
            <AddProduct />
          </ProtectedRoute>
        ),
      },
      {
        path: '/catalog/product/edit/:id',
        element: (
          <ProtectedRoute allowedRoles={['Metadata Manager']}>
            <EditProduct />
          </ProtectedRoute>
        ),
      },
      {
        path: '/catalog/categories',
        element: (
          <ProtectedRoute allowedRoles={['Metadata Manager']}>
            <CategoryList />
          </ProtectedRoute>
        ),
      },
      {
        path: '/catalog/units',
        element: (
          <ProtectedRoute allowedRoles={['Metadata Manager']}>
            <UnitList />
          </ProtectedRoute>
        ),
      },
      {
        path: '/catalog/suppliers',
        element: (
          <ProtectedRoute allowedRoles={['Metadata Manager']}>
            <SupplierList />
          </ProtectedRoute>
        ),
      },
      {
        path: '/catalog/warehouses',
        element: (
          <ProtectedRoute allowedRoles={['Metadata Manager']}>
            <WarehouseList />
          </ProtectedRoute>
        ),
      },

      {
        path: '/inventory/stock-in/add',
        element: (
          <ProtectedRoute allowedRoles={['Warehouse Keeper', 'Warehouse Staff']}>
            <CreateStockIn />
          </ProtectedRoute>
        ),
      },
      {
        path: '/inventory/stock-in/add/transfer',
        element: (
          <ProtectedRoute allowedRoles={['Warehouse Keeper', 'Warehouse Staff']}>
            <CreateStockIn type={2} />
          </ProtectedRoute>
        ),
      },
      {
        path: '/inventory/stock-in/add/return',
        element: (
          <ProtectedRoute allowedRoles={['Warehouse Keeper', 'Warehouse Staff']}>
            <CreateStockIn type={3} />
          </ProtectedRoute>
        ),
      },
      {
        path: '/inventory/stock-in/add/adjustment',
        element: (
          <ProtectedRoute allowedRoles={['Warehouse Keeper', 'Warehouse Staff']}>
            <CreateStockIn type={4} />
          </ProtectedRoute>
        ),
      },
      {
        path: '/inventory/stock-in/list',
        element: (
          <ProtectedRoute allowedRoles={['Warehouse Keeper', 'Warehouse Staff']}>
            <StockInListing />
          </ProtectedRoute>
        ),
      },
      {
        path: '/inventory/stock-in/edit/:id',
        element: (
          <ProtectedRoute allowedRoles={['Warehouse Keeper', 'Warehouse Staff']}>
            <EditStockIn />
          </ProtectedRoute>
        ),
      },
      {
        path: '/inventory/stock-in/detail/:id',
        element: (
          <ProtectedRoute allowedRoles={['Warehouse Keeper', 'Warehouse Staff']}>
            <StockInDetail />
          </ProtectedRoute>
        ),
      },

      {
        path: '/inventory/stock-out/add',
        element: (
          <ProtectedRoute allowedRoles={['Warehouse Keeper', 'Warehouse Staff']}>
            <CreateStockOut />
          </ProtectedRoute>
        ),
      },
      {
        path: '/inventory/stock-out/add/transfer',
        element: (
          <ProtectedRoute allowedRoles={['Warehouse Keeper', 'Warehouse Staff']}>
            <CreateStockOut type={2} />
          </ProtectedRoute>
        ),
      },
      {
        path: '/inventory/stock-out/add/adjustment',
        element: (
          <ProtectedRoute allowedRoles={['Warehouse Keeper', 'Warehouse Staff']}>
            <CreateStockOut type={3} />
          </ProtectedRoute>
        ),
      },
      {
        path: '/inventory/stock-out/edit/:id',
        element: (
          <ProtectedRoute allowedRoles={['Warehouse Keeper', 'Warehouse Staff']}>
            <EditStockOut />
          </ProtectedRoute>
        ),
      },
      {
        path: '/inventory/stock-out/detail/:id',
        element: (
          <ProtectedRoute
            allowedRoles={['Warehouse Keeper', 'Warehouse Staff', 'DepartmentHead', 'Regular User']}
          >
            <StockOutDetail />
          </ProtectedRoute>
        ),
      },
      {
        path: '/inventory/stock-out/list',
        element: (
          <ProtectedRoute allowedRoles={['Warehouse Keeper', 'Warehouse Staff']}>
            <StockOutListing />
          </ProtectedRoute>
        ),
      },

      {
        path: '/inventory/transfer/list',
        element: (
          <ProtectedRoute allowedRoles={['Warehouse Keeper', 'Warehouse Staff']}>
            <ReceiveTransferList />
          </ProtectedRoute>
        ),
      },
      {
        path: '/inventory/report',
        element: (
          <ProtectedRoute allowedRoles={['Warehouse Keeper', 'Warehouse Staff']}>
            <SnapshotList />
          </ProtectedRoute>
        ),
      },

      {
        path: '/inventory/stock-take/add',
        element: (
          <ProtectedRoute allowedRoles={['Warehouse Keeper', 'Warehouse Staff']}>
            <StockTakeCreate />
          </ProtectedRoute>
        ),
      },
      {
        path: '/inventory/stock-take/list',
        element: (
          <ProtectedRoute allowedRoles={['Warehouse Keeper', 'Warehouse Staff']}>
            <StockTakeList />
          </ProtectedRoute>
        ),
      },
      {
        path: '/inventory/stock-take/edit/:id',
        element: (
          <ProtectedRoute allowedRoles={['Warehouse Keeper', 'Warehouse Staff']}>
            <EditStockTake />
          </ProtectedRoute>
        ),
      },
      {
        path: '/inventory/stock-take/detail/:id',
        element: (
          <ProtectedRoute allowedRoles={['Warehouse Keeper', 'Warehouse Staff']}>
            <StockTakeDetail />
          </ProtectedRoute>
        ),
      },

      {
        path: '/my-work-space/inventory',
        element: (
          <ProtectedRoute allowedRoles={['Department Head', 'Regular User']}>
            <InventoryDepartmentPage />
          </ProtectedRoute>
        ),
      },
      {
        path: '/my-work-space/incoming-stock',
        element: (
          <ProtectedRoute allowedRoles={['Department Head', 'Regular User']}>
            <IncomingStockPage />
          </ProtectedRoute>
        ),
      },

      {
        path: '/organization/departments',
        element: (
          <ProtectedRoute allowedRoles={['Metadata Manager']}>
            <DepartmentList />
          </ProtectedRoute>
        ),
      },
      {
        path: '/organization/positions',
        element: (
          <ProtectedRoute allowedRoles={['Metadata Manager']}>
            <PositionList />
          </ProtectedRoute>
        ),
      },
      {
        path: '/organization/employees',
        element: (
          <ProtectedRoute allowedRoles={['Metadata Manager']}>
            <EmployeeList />
          </ProtectedRoute>
        ),
      },

      { path: '/request/material/add', element: <MaterialRequestCreate /> },
      { path: '/request/material/list', element: <MaterialRequestListing /> },
      { path: '/request/material/edit/:id', element: <EditMaterialRequest /> },
      { path: '/request/material/detail/:id', element: <MaterialRequestDetail /> },

      { path: '/change-password', element: <ChangePasswordPage /> },
      { path: '*', element: <Navigate to="/auth/404" /> },
    ],
  },
  {
    path: '/',
    element: <BlankLayout />,
    children: [
      { path: '/auth/404', element: <Error /> },
      { path: '/auth/403', element: <Forbidden /> },
      { path: '/auth/login', element: <Login /> },
      { path: '*', element: <Navigate to="/auth/404" /> },
    ],
  },
];

const router = createBrowserRouter(Router);

export default router;
