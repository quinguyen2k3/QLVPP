import { Button, Box } from '@mui/material';
import { IconPlus } from '@tabler/icons';
import React from 'react';
import { useTranslation } from 'react-i18next';

import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';
import UnitListTable from 'src/components/units/list/page';
import AddDialog from '../add/dialogAdd';
import EditDialog from '../edit/dialogEdit';
import { useFetchUnitData } from '../hooks/useFetchUnitData';

function Page() {
  const { t } = useTranslation();
  const BCrumb = [
    { to: '/', title: t('Menu.Home') },
    { title: t('Menu.Catalog') },
    { title: t('Menu.Unit') },
  ];
  const [openAdd, setOpenAdd] = React.useState(false);

  const [editDialogOpen, setEditDialogOpen] = React.useState(false);
  const [selectedUnitId, setSelectedUnitId] = React.useState(null);

  const { units, setUnits, loading } = useFetchUnitData();

  const handleEditClick = (id) => {
    setSelectedUnitId(id);
    setEditDialogOpen(true);
  };

  const handleAddSuccess = (newUnit) => {
    setUnits((prev) => [newUnit, ...prev]);
  };

  const handleUpdateSuccess = (updatedUnit) => {
    setUnits((prev) => prev.map((u) => (u.id === updatedUnit.id ? updatedUnit : u)));
    setEditDialogOpen(false);
  };

  return (
    <PageContainer title={t('Page.UnitList')} description={t('Page.UnitList')}>
      <Breadcrumb title={t('Menu.Unit')} items={BCrumb} />

      <Box sx={{ display: 'flex', justifyContent: 'flex-end', mb: 2 }}>
        <Button
          variant="contained"
          startIcon={<IconPlus size={20} />}
          onClick={() => setOpenAdd(true)}
        >
          {t('Action.Add')}
        </Button>
      </Box>

      {/* Table nhận data */}
      <UnitListTable units={units} loading={loading} onEditClick={handleEditClick} />

      {/* Dialog */}
      <AddDialog open={openAdd} onClose={() => setOpenAdd(false)} onSuccess={handleAddSuccess} />
      <EditDialog
        unitId={selectedUnitId}
        open={editDialogOpen}
        onClose={() => setEditDialogOpen(false)}
        onSuccess={handleUpdateSuccess}
      />
    </PageContainer>
  );
}

export default Page;
