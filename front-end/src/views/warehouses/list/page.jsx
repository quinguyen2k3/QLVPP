import { Button, Box } from '@mui/material';
import { IconPlus } from '@tabler/icons';
import React from 'react';

import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';
import WarehouseListTable from 'src/components/warehouses/list/page';
import AddWarehouseDialog from '../add/dialogAdd';
import EditWarehouseDialog from '../edit/dialogEdit';
import { useFetchWarehouseData } from '../hooks/useFetchWarehouseData';
import { useTranslation } from 'react-i18next';

function Page() {
  const { t } = useTranslation();
  const BCrumb = [
    { to: '/', title: t('Menu.Home') },
    { title: t('Menu.Catalog') },
    { title: t('Menu.Warehouse') },
  ];
  const [openAdd, setOpenAdd] = React.useState(false);

  const [editDialogOpen, setEditDialogOpen] = React.useState(false);
  const [selectedWarehouseId, setSelectedWarehouseId] = React.useState(null);

  const { warehouses, setWarehouses, loading } = useFetchWarehouseData();

  const handleEditClick = (id) => {
    setSelectedWarehouseId(id);
    setEditDialogOpen(true);
  };

  const handleAddSuccess = (newWarehouse) => {
    setWarehouses((prev) => [newWarehouse, ...prev]);
  };

  const handleUpdateSuccess = (updatedWarehouse) => {
    setWarehouses((prev) => prev.map((w) => (w.id === updatedWarehouse.id ? updatedWarehouse : w)));
    setEditDialogOpen(false);
  };

  return (
    <PageContainer title={t('Menu.Warehouse')} description="Trang quản lý danh sách kho hàng">
      <Breadcrumb title={t('Menu.Warehouse')} items={BCrumb} />

      <Box sx={{ display: 'flex', justifyContent: 'flex-end', mb: 2 }}>
        <Button
          variant="contained"
          startIcon={<IconPlus size={20} />}
          onClick={() => setOpenAdd(true)}
        >
          {t('Action.Add')}
        </Button>
      </Box>

      <WarehouseListTable warehouses={warehouses} loading={loading} onEditClick={handleEditClick} />

      {/* Dialog */}
      <AddWarehouseDialog
        open={openAdd}
        onClose={() => setOpenAdd(false)}
        onSuccess={handleAddSuccess}
      />

      <EditWarehouseDialog
        warehouseId={selectedWarehouseId}
        open={editDialogOpen}
        onClose={() => setEditDialogOpen(false)}
        onSuccess={handleUpdateSuccess}
      />
    </PageContainer>
  );
}

export default Page;
