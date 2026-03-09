import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';
import EditStockTakeApp from 'src/components/inventory-transaction/stock-take/edit';
import BlankCard from 'src/components/shared/BlankCard';
import { CardContent } from '@mui/material';
import { StockTakeProvider } from 'src/context/StockTakeContext';
import { useTranslation } from 'react-i18next';

const EditStockTake = () => {
  const { t } = useTranslation();

  const BCrumb = [
    {
      to: '/',
      title: t('Menu.Home'),
    },
    {
      to: '/inventory',
      title: t('Menu.Inventory'),
    },
    {
      to: '/inventory/stock-take/list',
      title: t('Menu.StockTake'),
    },
    {
      title: t('Page.EditStockTake'),
    },
  ];

  return (
    <StockTakeProvider>
      <PageContainer title={t('Page.EditStockTake')} description={t('Page.EditStockTake')}>
        <Breadcrumb title={t('Page.EditStockTake')} items={BCrumb} />
        <BlankCard>
          <CardContent>
            <EditStockTakeApp />
          </CardContent>
        </BlankCard>
      </PageContainer>
    </StockTakeProvider>
  );
};

export default EditStockTake;
