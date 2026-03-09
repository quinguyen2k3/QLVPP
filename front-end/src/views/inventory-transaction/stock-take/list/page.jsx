import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';
import StockTakeList from 'src/components/inventory-transaction/stock-take/list';
import { StockTakeProvider } from 'src/context/StockTakeContext';
import BlankCard from 'src/components/shared/BlankCard';
import { CardContent } from '@mui/material';
import { useTranslation } from 'react-i18next';

const StockTakeListing = () => {
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
      title: t('Menu.StockTake'),
    },
  ];
  return (
    <StockTakeProvider>
      <PageContainer title={t('Page.StockTakeList')} description={t('Page.StockTakeList')}>
        <Breadcrumb title={t('Menu.StockTake')} items={BCrumb} />
        <BlankCard>
          <CardContent>
            <StockTakeList />
          </CardContent>
        </BlankCard>
      </PageContainer>
    </StockTakeProvider>
  );
};

export default StockTakeListing;
