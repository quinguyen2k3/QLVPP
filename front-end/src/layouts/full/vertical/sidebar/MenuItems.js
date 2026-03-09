import {
  IconHome,
  IconPoint,
  IconNotebook,
  IconBuildingWarehouse,
  IconBriefcase,
  IconArrowsLeftRight, // Icon phù hợp hơn cho Điều chuyển
} from '@tabler/icons-react';
import { uniqueId } from 'lodash';

const Menuitems = [
  {
    id: uniqueId(),
    title: 'Menu.Dashboard',
    icon: IconHome,
    href: '/dashboards/',
    children: [
      {
        id: uniqueId(),
        title: 'Menu.Modern',
        icon: IconPoint,
        href: '/dashboards/modern',
        chip: 'New',
        chipColor: 'secondary',
      },
      {
        id: uniqueId(),
        title: 'Menu.Ecommerce',
        icon: IconPoint,
        href: '/dashboards/ecommerce',
      },
    ],
  },
  {
    id: uniqueId(),
    title: 'Menu.MyWorkSpace',
    icon: IconBriefcase,
    href: '/my-work-space/',
    children: [
      {
        id: uniqueId(),
        title: 'Menu.MyInventory',
        icon: IconPoint,
        href: '/my-work-space/inventory',
      },
      {
        id: uniqueId(),
        title: 'Menu.IncomingStock',
        icon: IconPoint,
        href: '/my-work-space/incoming-stock',
      },
    ],
  },
  {
    id: uniqueId(),
    title: 'Menu.Catalog',
    icon: IconNotebook,
    href: '/catalog/',
    children: [
      {
        id: uniqueId(),
        title: 'Menu.Product',
        icon: IconPoint,
        href: '/catalog/products',
      },
      {
        id: uniqueId(),
        title: 'Menu.Category',
        icon: IconPoint,
        href: '/catalog/categories',
      },
      {
        id: uniqueId(),
        title: 'Menu.Unit',
        icon: IconPoint,
        href: '/catalog/units',
      },
      {
        id: uniqueId(),
        title: 'Menu.Supplier',
        icon: IconPoint,
        href: '/catalog/suppliers',
      },
      {
        id: uniqueId(),
        title: 'Menu.Warehouse',
        icon: IconPoint,
        href: '/catalog/warehouses',
      },
    ],
  },
  {
    id: uniqueId(),
    title: 'Menu.Inventory',
    icon: IconBuildingWarehouse,
    href: '/inventory/',
    children: [
      {
        id: uniqueId(),
        title: 'Menu.StockIn',
        icon: IconPoint,
        href: '/inventory/stock-in/list',
      },
      {
        id: uniqueId(),
        title: 'Menu.StockOut',
        icon: IconPoint,
        href: '/inventory/stock-out/list',
      },
      {
        id: uniqueId(),
        title: 'Menu.ReportInventory',
        icon: IconPoint,
        href: '/inventory/report',
      },
      {
        id: uniqueId(),
        title: 'Menu.StockTake',
        icon: IconPoint,
        href: '/inventory/stock-take/list',
      },
    ],
  },
];

export default Menuitems;
