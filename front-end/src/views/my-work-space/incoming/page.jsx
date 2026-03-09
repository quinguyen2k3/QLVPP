import React from 'react';
import { useTranslation } from 'react-i18next';
import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';
import IncomingStockList from 'src/components/my-work-space/incoming';

function IncomingStockPage() {
  const { t } = useTranslation();

  const BCrumb = [
    {
      to: '/',
      title: t('Menu.Home') || 'Home',
    },
    {
      title: t('Page.IncomingStock') || 'Incoming Stock',
    },
  ];

  return (
    <PageContainer
      title={t('Page.IncomingStock') || 'Incoming Stock'}
      description={t('Description.IncomingStock') || 'Danh sách phiếu chuyển kho đang đến'}
    >
      <Breadcrumb title={t('Page.IncomingStock') || 'Incoming Stock'} items={BCrumb} />
      <IncomingStockList />
    </PageContainer>
  );
}

export default IncomingStockPage;
