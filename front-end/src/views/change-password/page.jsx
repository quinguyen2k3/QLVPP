import React from 'react';
import { useTranslation } from 'react-i18next';
import { Box } from '@mui/material';

import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';

import ChangePassword from 'src/components/change-password';

function ChangePasswordPage() {
  const { t } = useTranslation();

  const BCrumb = [
    { to: '/', title: t('Menu.Home') || 'Home' },
    { title: t('Page.ChangePassword') || 'Đổi mật khẩu' },
  ];

  return (
    <PageContainer
      title={t('Page.ChangePassword') || 'Đổi mật khẩu'}
      description={
        t('Description.ChangePasswordPage') || 'Trang cập nhật mật khẩu mới cho người dùng'
      }
    >
      <Breadcrumb title={t('Page.ChangePassword') || 'Đổi mật khẩu'} items={BCrumb} />

      <Box mt={3}>
        <ChangePassword />
      </Box>
    </PageContainer>
  );
}

export default ChangePasswordPage;
