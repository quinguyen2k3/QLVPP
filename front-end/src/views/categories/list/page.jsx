import { Button, Box } from '@mui/material';
import { IconPlus } from '@tabler/icons';
import React from 'react';
import { useTranslation } from 'react-i18next';

import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';
import CategoryListTable from 'src/components/categories/list/page';
import AddDialog from '../add/dialogAdd';
import EditDialog from '../edit/dialogEdit';
import { useFetchCategoryData } from '../hooks/useFetchCategoryData';

function Page() {
  const { t } = useTranslation();

  const BCrumb = [
    { to: '/', title: t('Menu.Home') },
    { title: t('Menu.Catalog') },
    { title: t('Menu.Category') },
  ];

  const [openAdd, setOpenAdd] = React.useState(false);

  const [editDialogOpen, setEditDialogOpen] = React.useState(false);
  const [selectedCategoryId, setSelectedCategoryId] = React.useState(null);

  const { categories, setCategories, loading } = useFetchCategoryData();

  const handleEditClick = (id) => {
    setSelectedCategoryId(id);
    setEditDialogOpen(true);
  };

  const handleAddSuccess = (newCategory) => {
    setCategories((prev) => [newCategory, ...prev]);
  };

  const handleUpdateSuccess = (updatedCategory) => {
    setCategories((prev) => prev.map((c) => (c.id === updatedCategory.id ? updatedCategory : c)));
    setEditDialogOpen(false);
  };

  return (
    <PageContainer
      title={t('Menu.Category')}
      description="Trang quản lý danh sách danh mục sản phẩm"
    >
      <Breadcrumb title={t('Menu.Category')} items={BCrumb} />

      <Box sx={{ display: 'flex', justifyContent: 'flex-end', mb: 2 }}>
        <Button
          variant="contained"
          startIcon={<IconPlus size={20} />}
          onClick={() => setOpenAdd(true)}
        >
          {t('Action.Add')}
        </Button>
      </Box>

      <CategoryListTable categories={categories} loading={loading} onEditClick={handleEditClick} />

      <AddDialog open={openAdd} onClose={() => setOpenAdd(false)} onSuccess={handleAddSuccess} />
      <EditDialog
        categoryId={selectedCategoryId}
        open={editDialogOpen}
        onClose={() => setEditDialogOpen(false)}
        onSuccess={handleUpdateSuccess}
      />
    </PageContainer>
  );
}

export default Page;
