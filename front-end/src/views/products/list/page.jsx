import { Button, Box } from '@mui/material';
import { Link } from 'react-router-dom';
import { IconPlus } from '@tabler/icons-react'; // Đã sửa import chuẩn React
import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';
import ProductListTable from 'src/components/products/list/page';
import { useTranslation } from 'react-i18next';

function ProductListPage() {
  const { t } = useTranslation();

  const BCrumb = [
    {
      to: '/',
      title: t('Menu.Home'),
    },
    { title: t('Menu.Catalog') },
    {
      title: t('Menu.Product'),
    },
  ];

  return (
    <PageContainer title={t('Page.ProductList')} description={t('Page.ProductList')}>
      <Breadcrumb title={t('Page.ProductList')} items={BCrumb} />

      <Box sx={{ display: 'flex', justifyContent: 'flex-end', mb: 2 }}>
        <Button
          component={Link}
          to="/catalog/product/add"
          variant="contained"
          color="primary"
          startIcon={<IconPlus size={20} />}
        >
          {t('Action.Add')}
        </Button>
      </Box>

      <ProductListTable />
    </PageContainer>
  );
}

export default ProductListPage;
