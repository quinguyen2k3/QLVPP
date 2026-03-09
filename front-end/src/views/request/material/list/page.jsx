import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';
import MaterialRequestList from 'src/components/request/material/list';
import { MaterialRequestProvider } from 'src/context/MaterialRequestContext';
import BlankCard from 'src/components/shared/BlankCard';
import { CardContent } from '@mui/material';
import { useTranslation } from 'react-i18next';

const MaterialRequestListing = () => {
  const { t } = useTranslation();

  const BCrumb = [
    {
      to: '/',
      title: t('Menu.Home'),
    },
    {
      to: '/inventory',
      title: t('Menu.ManageRequest'),
    },
    {
      title: t('Menu.MaterialRequest') || 'Yêu cầu vật tư',
    },
  ];

  return (
    <MaterialRequestProvider>
      <PageContainer
        title={t('Page.MaterialRequestList') || 'Danh sách yêu cầu vật tư'}
        description={t('Page.MaterialRequestList') || 'Danh sách yêu cầu vật tư'}
      >
        <Breadcrumb title={t('Menu.MaterialRequest') || 'Yêu cầu vật tư'} items={BCrumb} />
        <BlankCard>
          <CardContent>
            <MaterialRequestList />
          </CardContent>
        </BlankCard>
      </PageContainer>
    </MaterialRequestProvider>
  );
};

export default MaterialRequestListing;
