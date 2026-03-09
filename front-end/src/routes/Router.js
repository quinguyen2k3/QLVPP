import React, { lazy } from 'react';
import { Navigate, createBrowserRouter } from 'react-router';

import Loadable from '../layouts/full/shared/loadable/Loadable';

/* ***Layouts**** */
const FullLayout = Loadable(lazy(() => import('../layouts/full/FullLayout')));
const BlankLayout = Loadable(lazy(() => import('../layouts/blank/BlankLayout')));

/* ****Pages***** */
const ModernDash = Loadable(lazy(() => import('../views/dashboard/Modern')));
const EcommerceDash = Loadable(lazy(() => import('../views/dashboard/Ecommerce')));

/* ****Apps***** */
// const Blog = Loadable(lazy(() => import('../views/apps/blog/Blog')));
// const BlogDetail = Loadable(lazy(() => import('../views/apps/blog/BlogPost')));

const Login = Loadable(lazy(() => import('../views/authentication/auth1/Login')));
const Register = Loadable(lazy(() => import('../views/authentication/auth1/Register')));
const ForgotPassword = Loadable(lazy(() => import('../views/authentication/auth1/ForgotPassword')));

const TwoSteps = Loadable(lazy(() => import('../views/authentication/auth1/TwoSteps')));
const Error = Loadable(lazy(() => import('../views/authentication/Error')));
const Maintenance = Loadable(lazy(() => import('../views/authentication/Maintenance')));

//product
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
const MaterialRequestCreate = Loadable(
  lazy(() => import('../views/request/material/add/page')),
);
const MaterialRequestListing = Loadable(
  lazy(() => import('../views/request/material/list/page')),
);
const EditMaterialRequest = Loadable(
  lazy(() => import('../views/request/material/edit/page')),
);
const MaterialRequestDetail = Loadable(
  lazy(() => import('../views/request/material/detail/page')),
);
const ChangePasswordPage = Loadable(
  lazy(() => import('../views/change-password/page')),
);

const Router = [
  {
    path: '/',
    element: <FullLayout />,
    children: [
      { path: '/', element: <Navigate to="/dashboards/modern" /> },
      { path: '/dashboards/modern', exact: true, element: <ModernDash /> },
      { path: '/dashboards/ecommerce', exact: true, element: <EcommerceDash /> },

      { path: '/catalog/products', element: <ProductList /> },
      { path: '/catalog/product/add', element: <AddProduct /> },
      { path: '/catalog/product/edit/:id', element: <EditProduct /> },

      { path: '/catalog/categories', element: <CategoryList /> },
      { path: '/catalog/units', element: <UnitList /> },
      { path: '/catalog/suppliers', element: <SupplierList /> },
      { path: '/catalog/warehouses', element: <WarehouseList /> },

      { path: '/inventory/stock-in/add', element: <CreateStockIn /> },
      { path: '/inventory/stock-in/add/transfer', element: <CreateStockIn type={2} /> },
      { path: '/inventory/stock-in/add/return', element: <CreateStockIn type={3} /> },
      { path: '/inventory/stock-in/add/adjustment', element: <CreateStockIn type={4} /> },
      { path: '/inventory/stock-in/list', element: <StockInListing /> },
      { path: '/inventory/stock-in/edit/:id', element: <EditStockIn /> },
      { path: '/inventory/stock-in/detail/:id', element: <StockInDetail /> },

      { path: '/inventory/stock-out/add', element: <CreateStockOut /> },
      { path: '/inventory/stock-out/add/transfer', element: <CreateStockOut type={2} /> },
      { path: '/inventory/stock-out/add/adjustment', element: <CreateStockOut type={3} /> },
      { path: '/inventory/stock-out/edit/:id', element: <EditStockOut /> },
      { path: '/inventory/stock-out/detail/:id', element: <StockOutDetail /> },
      { path: '/inventory/stock-out/list', element: <StockOutListing /> },

      { path: '/inventory/transfer/list', element: <ReceiveTransferList /> },

      { path: '/my-work-space/inventory', element: <InventoryDepartmentPage /> },
      { path: '/my-work-space/incoming-stock', element: <IncomingStockPage /> },

      { path: '/inventory/report', element: <SnapshotList /> },
      { path: '/organization/departments', element: <DepartmentList /> },
      { path: '/organization/positions', element: <PositionList /> },
      { path: '/organization/employees', element: <EmployeeList /> },

      { path: '/inventory/stock-take/add', element: <StockTakeCreate /> },
      { path: '/inventory/stock-take/list', element: <StockTakeList /> },
      { path: '/inventory/stock-take/edit/:id', element: <EditStockTake /> },
      { path: '/inventory/stock-take/detail/:id', element: <StockTakeDetail /> },

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
      { path: '/auth/login', element: <Login /> },
      { path: '/auth/register', element: <Register /> },
      { path: '/auth/forgot-password', element: <ForgotPassword /> },
      { path: '/auth/two-steps', element: <TwoSteps /> },
      { path: '*', element: <Navigate to="/auth/404" /> },
    ],
  },
];

const router = createBrowserRouter(Router);

export default router;
