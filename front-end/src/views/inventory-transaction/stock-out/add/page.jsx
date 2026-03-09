import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';
import CreateStockOutApp from 'src/components/inventory-transaction/stock-out/add';
import BlankCard from 'src/components/shared/BlankCard';
import { CardContent } from '@mui/material';
import { StockOutProvider } from 'src/context/StockOutContext';
import { useTranslation } from 'react-i18next';

const CreateStockOut = ({ type = 1 }) => {
  const { t } = useTranslation();

  const getPageTitle = () => {
    switch (type) {
      case 1:
        return t('StockOutType.Usage');
      case 2:
        return t('StockOutType.Transfer');
      case 3:
        return t('StockOutType.Adjustment');
      default:
        return t('Page.AddStockOut');
    }
  };

  const pageTitle = getPageTitle();

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
      to: '/inventory/stock-out/list',
      title: t('Menu.StockOut'),
    },
    {
      title: pageTitle,
    },
  ];

  return (
    <StockOutProvider>
      <PageContainer title={pageTitle} description={pageTitle}>
        <Breadcrumb title={pageTitle} items={BCrumb} />

        <BlankCard>
          <CardContent>
            <CreateStockOutApp type={type} />
          </CardContent>
        </BlankCard>
      </PageContainer>
    </StockOutProvider>
  );
};

export default CreateStockOut;
