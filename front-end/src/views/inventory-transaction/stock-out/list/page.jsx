import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';
import StockOutList from 'src/components/inventory-transaction/stock-out/list';
import { StockOutProvider } from 'src/context/StockOutContext';
import BlankCard from 'src/components/shared/BlankCard';
import { CardContent } from '@mui/material';
import { useTranslation } from 'react-i18next';

const StockOutListing = () => {
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
      title: t('Menu.StockOut'),
    },
  ];
  return (
    <StockOutProvider>
      <PageContainer title={t('Page.StockOutList')} description={t('Page.StockOutList')}>
        <Breadcrumb title={t('Menu.StockOut')} items={BCrumb} />
        <BlankCard>
          <CardContent>
            <StockOutList />
          </CardContent>
        </BlankCard>
      </PageContainer>
    </StockOutProvider>
  );
};
export default StockOutListing;
