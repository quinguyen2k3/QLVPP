import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';
import EditStockInApp from 'src/components/inventory-transaction/stock-in/edit';
import BlankCard from 'src/components/shared/BlankCard';
import { CardContent } from '@mui/material';
import { StockInProvider } from 'src/context/StockInContext';
import { useTranslation } from 'react-i18next';

const EditStockIn = () => {
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
    {
      title: t('Page.EditStockIn'),
    },
  ];

  return (
    <StockInProvider>
      <PageContainer title={t('Page.EditStockIn')} description={t('Page.EditStockIn')}>
        <Breadcrumb title={t('Page.EditStockIn')} items={BCrumb} />

        <BlankCard>
          <CardContent>
            <EditStockInApp />
          </CardContent>
        </BlankCard>
      </PageContainer>
    </StockInProvider>
  );
};
export default EditStockIn;
