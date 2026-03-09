import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';
import { StockTakeProvider } from 'src/context/StockTakeContext';
import StockTakeDetail from 'src/components/inventory-transaction/stock-take/detail/index';
import BlankCard from 'src/components/shared/BlankCard';
import { CardContent } from '@mui/material';
import { useTranslation } from 'react-i18next';

const StockTakeDetailPage = () => {
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
      title: t('Page.StockTakeDetail'),
    },
  ];

  return (
    <StockTakeProvider>
      <PageContainer title={t('Page.StockTakeDetail')} description={t('Page.StockTakeDetail')}>
        <Breadcrumb title={t('Page.StockTakeDetail')} items={BCrumb} />
        <BlankCard>
          <CardContent>
            <StockTakeDetail />
          </CardContent>
        </BlankCard>
      </PageContainer>
    </StockTakeProvider>
  );
};

export default StockTakeDetailPage;