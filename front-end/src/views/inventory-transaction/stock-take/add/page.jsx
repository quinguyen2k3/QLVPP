import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';
import CreateStockTakeApp from 'src/components/inventory-transaction/stock-take/add';
import BlankCard from 'src/components/shared/BlankCard';
import { CardContent } from '@mui/material';
import { StockTakeProvider } from 'src/context/StockTakeContext';
import { useTranslation } from 'react-i18next';

const CreateStockTake = () => {
  const { t } = useTranslation();

  const pageTitle = t('Page.AddStockTake');

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
      title: pageTitle,
    },
  ];

  return (
    <StockTakeProvider>
      <PageContainer title={pageTitle} description={pageTitle}>
        <Breadcrumb title={pageTitle} items={BCrumb} />

        <BlankCard>
          <CardContent>
            <CreateStockTakeApp />
          </CardContent>
        </BlankCard>
      </PageContainer>
    </StockTakeProvider>
  );
};

export default CreateStockTake;
