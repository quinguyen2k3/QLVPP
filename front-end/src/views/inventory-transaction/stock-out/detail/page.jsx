import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';
import { StockOutProvider } from 'src/context/StockOutContext/index';
import StockOutDetail from 'src/components/inventory-transaction/stock-out/detail/index';
import BlankCard from 'src/components/shared/BlankCard';
import { CardContent } from '@mui/material';
import { useTranslation } from 'react-i18next';

const StockOutDetailPage = () => {
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
    {
      title: t('Page.StockOutDetail'),
    }
  ];
  return (
    <StockOutProvider>
      <PageContainer title={t('Page.StockOutDetail')} description={t('Page.StockOutDetail')}>
        <Breadcrumb title={t('Page.StockOutDetail')} items={BCrumb} />
        <BlankCard>
          <CardContent>
            <StockOutDetail />
          </CardContent>
        </BlankCard>
      </PageContainer>
    </StockOutProvider>
  );
};
export default StockOutDetailPage;
