import React, { useState, useCallback } from 'react';
import { Button, Box } from '@mui/material';
import { IconPlus } from '@tabler/icons-react';
import { useTranslation } from 'react-i18next';

import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';
import EmployeeListTable from 'src/components/employee/list';
import AddDialog from 'src/components/employee/add';
import EditDialog from 'src/components/employee/edit';

import { useEmployees } from 'src/hooks/useMasterData';

function DepartmentPage() {
  const { t } = useTranslation();

  const {
    employees,
    isLoading,
    addLocal: addEmployeeToCache,
    updateLocal: updateEmployeeInCache,
  } = useEmployees();

  const BCrumb = [
    { to: '/', title: t('Menu.Home') },
    { title: t('Menu.Organization') },
    { title: t('Menu.Department') },
  ];

  const [openAdd, setOpenAdd] = useState(false);
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const [selectedEmployeeId, setSelectedEmployeeId] = useState(null);

  const handleEditClick = useCallback((id) => {
    setSelectedEmployeeId(id);
    setEditDialogOpen(true);
  }, []);

  const handleAddSuccess = (newRecord) => {
    if (newRecord) {
      addEmployeeToCache(newRecord);
    }
  };

  const handleUpdateSuccess = (updatedRecord) => {
    if (updatedRecord) {
      updateEmployeeInCache(updatedRecord);
      setEditDialogOpen(false);
    }
  };

  return (
    <PageContainer title={t('Menu.Employee')} description="Trang quản lý danh sách phòng ban">
      <Breadcrumb title={t('Menu.Employee')} items={BCrumb} />

      <Box sx={{ display: 'flex', justifyContent: 'flex-end', mb: 2 }}>
        <Button
          variant="contained"
          startIcon={<IconPlus size={20} />}
          onClick={() => setOpenAdd(true)}
        >
          {t('Action.Add')}
        </Button>
      </Box>

      <EmployeeListTable
        employees={employees}
        loading={isLoading}
        onEditClick={handleEditClick}
      />

      <AddDialog open={openAdd} onClose={() => setOpenAdd(false)} onSuccess={handleAddSuccess} />

      <EditDialog
        employeeId={selectedEmployeeId}
        open={editDialogOpen}
        onClose={() => setEditDialogOpen(false)}
        onSuccess={handleUpdateSuccess}
      />
    </PageContainer>
  );
}

export default DepartmentPage;
