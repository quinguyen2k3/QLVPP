import React, { useState, useCallback } from 'react';
import { Button, Box } from '@mui/material';
import { IconPlus } from '@tabler/icons-react';
import { useTranslation } from 'react-i18next';

import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';

import PositionListTable from 'src/components/position/list';
import AddDialog from 'src/components/position/add';
import EditDialog from 'src/components/position/edit';

import { usePositions } from 'src/hooks/useMasterData';

function PositionPage() {
  const { t } = useTranslation();

  const {
    positions,
    isLoading,
    addLocal: addPositionToCache,
    updateLocal: updatePositionInCache,
  } = usePositions();

  const BCrumb = [
    { to: '/', title: t('Menu.Home') },
    { title: t('Menu.Organization') },
    { title: t('Menu.Position') },
  ];

  const [openAdd, setOpenAdd] = useState(false);
  const [editDialogOpen, setEditDialogOpen] = useState(false);

  const [selectedPositionId, setSelectedPositionId] = useState(null);

  const handleEditClick = useCallback((id) => {
    setSelectedPositionId(id);
    setEditDialogOpen(true);
  }, []);

  const handleAddSuccess = (newRecord) => {
    if (newRecord) {
      addPositionToCache(newRecord);
    }
  };

  const handleUpdateSuccess = (updatedRecord) => {
    if (updatedRecord) {
      updatePositionInCache(updatedRecord);
      setEditDialogOpen(false);
    }
  };

  return (
    <PageContainer title={t('Menu.Position')} description="Trang quản lý danh sách chức vụ">
      <Breadcrumb title={t('Menu.Position')} items={BCrumb} />

      <Box sx={{ display: 'flex', justifyContent: 'flex-end', mb: 2 }}>
        <Button
          variant="contained"
          startIcon={<IconPlus size={20} />}
          onClick={() => setOpenAdd(true)}
        >
          {t('Action.Add')}
        </Button>
      </Box>

      <PositionListTable positions={positions} loading={isLoading} onEditClick={handleEditClick} />

      <AddDialog open={openAdd} onClose={() => setOpenAdd(false)} onSuccess={handleAddSuccess} />

      <EditDialog
        positionId={selectedPositionId}
        open={editDialogOpen}
        onClose={() => setEditDialogOpen(false)}
        onSuccess={handleUpdateSuccess}
      />
    </PageContainer>
  );
}

export default PositionPage;
