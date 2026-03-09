import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';
import { StockInProvider } from 'src/context/StockInContext/index';
import StockInDetail from 'src/components/inventory-transaction/stock-in/detail/index';
import BlankCard from 'src/components/shared/BlankCard';
import { CardContent } from '@mui/material';
import { useTranslation } from 'react-i18next';

const StockInDetailPage = () => {
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
    { title: t('Menu.StockIn') },
    {
      title: t('Page.StockInDetail'),
    },
  ];
  return (
    <StockInProvider>
      <PageContainer title={t('Page.StockInDetail')} description={t('Description.StockInDetail')}>
        <Breadcrumb title={t('Page.StockInDetail')} items={BCrumb} />
        <BlankCard>
          <CardContent>
            <StockInDetail />
          </CardContent>
        </BlankCard>
      </PageContainer>
    </StockInProvider>
  );
};
export default StockInDetailPage;
