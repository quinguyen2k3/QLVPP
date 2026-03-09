import React, { useState } from 'react';
import { Box, Button } from '@mui/material';
import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';
import { useTranslation } from 'react-i18next';
import { IconPlus } from '@tabler/icons-react';

import InventorySnapshotList from 'src/components/snapshot/list';
import AddDialog from 'src/components/snapshot/add';
import InventorySnapshotDetail from 'src/components/snapshot/detail'; 

function InventorySnapshotPage() {
  const { t } = useTranslation();
  
  const [openAddDialog, setOpenAddDialog] = useState(false);

  const [openDetailDialog, setOpenDetailDialog] = useState(false);
  const [selectedId, setSelectedId] = useState(null);

  const [refreshKey, setRefreshKey] = useState(0);

  const BCrumb = [
    { to: '/', title: t('Menu.Home') },
    { title: t('Menu.Inventory') },
    { title: t('Menu.ReportInventory') },
  ];

  const handleOpenAdd = () => setOpenAddDialog(true);
  const handleCloseAdd = () => setOpenAddDialog(false);

  const handleOpenDetail = (id) => {
    setSelectedId(id);
    setOpenDetailDialog(true);
  };

  const handleCloseDetail = () => {
    setOpenDetailDialog(false);
    setSelectedId(null);
  };

  const handleSuccess = () => {
    setRefreshKey((prev) => prev + 1); 
  };

  return (
    <PageContainer title={t('Menu.ReportInventory')} description={t('Menu.ReportInventory')}>
      <Breadcrumb title={t('Menu.ReportInventory')} items={BCrumb} />

      <Box sx={{ display: 'flex', justifyContent: 'flex-end', mb: 2 }}>
        <Button
          variant="contained"
          color="primary"
          startIcon={<IconPlus size={20} />}
          onClick={handleOpenAdd} 
        >
          {t('Action.Add')}
        </Button>
      </Box>

      <Box sx={{ mt: 2 }}>
        <InventorySnapshotList 
            key={refreshKey} 
            onViewDetail={handleOpenDetail} 
        />
      </Box>

      {openAddDialog && (
        <AddDialog 
          open={openAddDialog} 
          onClose={handleCloseAdd} 
          onSuccess={handleSuccess}
        />
      )}

      <InventorySnapshotDetail
        open={openDetailDialog}
        onClose={handleCloseDetail}
        id={selectedId}
      />
    </PageContainer>
  );
}

export default InventorySnapshotPage;