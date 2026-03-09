import React from 'react';
import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';
import InventoryList from 'src/components/my-work-space/inventory'; 

const BCrumb = [
  {
    to: '/',
    title: 'Home',
  },
  {
    title: 'Department Inventory',
  },
];

function InventoryDepartmentPage() {
  return (
    <PageContainer title="Department Inventory" description="Quản lý tồn kho phòng ban">
      <Breadcrumb title="Department Inventory" items={BCrumb} />
      <InventoryList />
    </PageContainer>
  );
}

export default InventoryDepartmentPage;