import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';
import BlankCard from 'src/components/shared/BlankCard';
import { CardContent } from '@mui/material';

import CreateStockInApp from 'src/components/inventory-transaction/stock-in/add';
import { StockInProvider } from 'src/context/StockInContext';
import { useTranslation } from 'react-i18next';

const CreateStockIn = ({ type = 1 }) => {
  const { t } = useTranslation();

  const getPageTitle = () => {
    switch (type) {
      case 1:
        return t('StockInType.Purchase');
      case 2:
        return t('StockInType.Transfer');
      case 3:
        return t('StockInType.Return');
      case 4:
        return t('StockInType.Adjustment');
      default:
        return t('Page.AddStockIn');
    }
  };

  const pageTitle = getPageTitle();

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
      to: '/inventory/stock-in/list',
      title: t('Menu.StockIn'),
    },
    {
      title: pageTitle,
    },
  ];

  return (
    <StockInProvider>
      <PageContainer title={pageTitle} description={pageTitle}>
        <Breadcrumb title={pageTitle} items={BCrumb} />

        <BlankCard>
          <CardContent>
            <CreateStockInApp type={type} />
          </CardContent>
        </BlankCard>
      </PageContainer>
    </StockInProvider>
  );
};

export default CreateStockIn;