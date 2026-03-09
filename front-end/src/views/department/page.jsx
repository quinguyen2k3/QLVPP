import React, { useState, useCallback } from 'react';
import { Button, Box } from '@mui/material';
import { IconPlus } from '@tabler/icons-react';
import { useTranslation } from 'react-i18next';

import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';
import DepartmentListTable from 'src/components/department/list';
import AddDialog from 'src/components/department/add';
import EditDialog from 'src/components/department/edit';

import { useDepartments } from 'src/hooks/useMasterData';

function DepartmentPage() {
  const { t } = useTranslation();

  const {
    departments,
    isLoading,
    addLocal: addDepartmentToCache,
    updateLocal: updateDepartmentInCache,
  } = useDepartments();

  const BCrumb = [
    { to: '/', title: t('Menu.Home') },
    { title: t('Menu.Organization') },
    { title: t('Menu.Department') },
  ];

  const [openAdd, setOpenAdd] = useState(false);
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const [selectedDepartmentId, setSelectedDepartmentId] = useState(null);

  const handleEditClick = useCallback((id) => {
    setSelectedDepartmentId(id);
    setEditDialogOpen(true);
  }, []);

  const handleAddSuccess = (newRecord) => {
    if (newRecord) {
      addDepartmentToCache(newRecord);
    }
  };

  const handleUpdateSuccess = (updatedRecord) => {
    if (updatedRecord) {
      updateDepartmentInCache(updatedRecord);
      setEditDialogOpen(false);
    }
  };

  return (
    <PageContainer title={t('Menu.Department')} description="Trang quản lý danh sách phòng ban">
      <Breadcrumb title={t('Menu.Department')} items={BCrumb} />

      <Box sx={{ display: 'flex', justifyContent: 'flex-end', mb: 2 }}>
        <Button
          variant="contained"
          startIcon={<IconPlus size={20} />}
          onClick={() => setOpenAdd(true)}
        >
          {t('Action.Add')}
        </Button>
      </Box>

      <DepartmentListTable
        departments={departments}
        loading={isLoading}
        onEditClick={handleEditClick}
      />

      <AddDialog open={openAdd} onClose={() => setOpenAdd(false)} onSuccess={handleAddSuccess} />

      <EditDialog
        departmentId={selectedDepartmentId}
        open={editDialogOpen}
        onClose={() => setEditDialogOpen(false)}
        onSuccess={handleUpdateSuccess}
      />
    </PageContainer>
  );
}

export default DepartmentPage;
