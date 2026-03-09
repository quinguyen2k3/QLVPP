import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';
import EditStockOutApp from 'src/components/inventory-transaction/stock-out/edit';
import BlankCard from 'src/components/shared/BlankCard';
import { CardContent } from '@mui/material';
import { StockOutProvider } from 'src/context/StockOutContext';
import { useTranslation } from 'react-i18next';

const EditStockOut = () => {
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
      title: t('Page.EditStockOut'),
    },
  ];

  return (
    <StockOutProvider>
      <PageContainer title={t('Page.EditStockOut')} description={t('Page.EditStockOut')}>
        <Breadcrumb title={t('Page.EditStockOut')} items={BCrumb} />
        <BlankCard>
          <CardContent>
            <EditStockOutApp />
          </CardContent>
        </BlankCard>
      </PageContainer>
    </StockOutProvider>
  );
};
export default EditStockOut;
