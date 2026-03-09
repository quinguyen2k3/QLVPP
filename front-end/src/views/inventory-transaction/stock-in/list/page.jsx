import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';
import StockInList from 'src/components/inventory-transaction/stock-in/list';
import { StockInProvider } from 'src/context/StockInContext';
import BlankCard from 'src/components/shared/BlankCard';
import { CardContent } from '@mui/material';
import { useTranslation } from 'react-i18next';

const StockInListing = () => {
  const { t } = useTranslation();
  const BCrumb = [
    {
      to: '/',
      title: t('Menu.Home'),
    },
    ,
    {
      to: '/inventory',
      title: t('Menu.Inventory'),
    },
    {
      title: t('Menu.StockIn'),
    },
  ];

  return (
    <StockInProvider>
      <PageContainer title={t('Page.StockInList')} description={t('Page.StockInList')}>
        <Breadcrumb title={t('Menu.StockIn')} items={BCrumb} />
        <BlankCard>
          <CardContent>
            <StockInList />
          </CardContent>
        </BlankCard>
      </PageContainer>
    </StockInProvider>
  );
};
export default StockInListing;
