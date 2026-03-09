import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';
import ReceiveTransferList from 'src/components/inventory-transaction/transfer/list';
import BlankCard from 'src/components/shared/BlankCard';
import { CardContent } from '@mui/material';
import { useTranslation } from 'react-i18next';

const ReceiveTransferListing = () => {
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
      to: '/inventory/transfer/list',
      title: t('Menu.Transfer'),
    },
    {
      title: t('Menu.ReceiveTransfer'),
    },
  ];

  return (
    <PageContainer
      title={t('Page.ReceiveTransferList')}
      description={t('Page.ReceiveTransferList')}
    >
      <Breadcrumb title={t('Menu.ReceiveTransfer')} items={BCrumb} />
      <BlankCard>
        <CardContent>
          <ReceiveTransferList />
        </CardContent>
      </BlankCard>
    </PageContainer>
  );
};

export default ReceiveTransferListing;
