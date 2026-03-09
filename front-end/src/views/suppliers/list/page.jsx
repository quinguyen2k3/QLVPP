import { Button, Box } from '@mui/material';
import { IconPlus } from '@tabler/icons';
import React from 'react';

import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';
import SupplierListTable from 'src/components/suppliers/list/page';
import AddDialog from '../add/dialogAdd';
import EditDialog from '../edit/dialogEdit';
import { useFetchSupplierData } from '../hooks/useFetchSupplierData';
import { useTranslation } from 'react-i18next';

function Page() {
  const { t } = useTranslation();

  const BCrumb = [
    { to: '/', title: t('Menu.Home') },
    { title: t('Menu.Catalog') },
    { title: t('Menu.Supplier') },
  ];

  const [openAdd, setOpenAdd] = React.useState(false);

  const [editDialogOpen, setEditDialogOpen] = React.useState(false);
  const [selectedSupplierId, setSelectedSupplierId] = React.useState(null);

  const { suppliers, setSuppliers, loading } = useFetchSupplierData();

  const handleEditClick = (id) => {
    setSelectedSupplierId(id);
    setEditDialogOpen(true);
  };

  const handleAddSuccess = (newSupplier) => {
    setSuppliers((prev) => [newSupplier, ...prev]);
  };

  const handleUpdateSuccess = (updatedSupplier) => {
    setSuppliers((prev) => prev.map((u) => (u.id === updatedSupplier.id ? updatedSupplier : u)));
    setEditDialogOpen(false);
  };

  return (
    <PageContainer title={t('Menu.Supplier')} description="Trang quản lý danh sách nhà cung cấp">
      <Breadcrumb title={t('Menu.Supplier')} items={BCrumb} />

      <Box sx={{ display: 'flex', justifyContent: 'flex-end', mb: 2 }}>
        <Button
          variant="contained"
          startIcon={<IconPlus size={20} />}
          onClick={() => setOpenAdd(true)}
        >
          {t('Action.Add')}
        </Button>
      </Box>

      <SupplierListTable suppliers={suppliers} loading={loading} onEditClick={handleEditClick} />

      <AddDialog open={openAdd} onClose={() => setOpenAdd(false)} onSuccess={handleAddSuccess} />
      <EditDialog
        supplierId={selectedSupplierId}
        open={editDialogOpen}
        onClose={() => setEditDialogOpen(false)}
        onSuccess={handleUpdateSuccess}
      />
    </PageContainer>
  );
}

export default Page;
