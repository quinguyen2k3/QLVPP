import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';
import { MaterialRequestProvider } from 'src/context/MaterialRequestContext';
import MaterialRequestDetail from 'src/components/request/material/detail';
import BlankCard from 'src/components/shared/BlankCard';
import { CardContent } from '@mui/material';
import { useTranslation } from 'react-i18next';

const MaterialRequestDetailPage = () => {
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
      to: '/inventory/material-request/list',
      title: t('Menu.MaterialRequest') || 'Yêu cầu vật tư',
    },
    {
      title: t('Page.MaterialRequestDetail') || 'Chi tiết yêu cầu',
    }
  ];

  return (
    <MaterialRequestProvider>
      <PageContainer title={t('Page.MaterialRequestDetail') || 'Chi tiết yêu cầu'} description={t('Page.MaterialRequestDetail') || 'Chi tiết yêu cầu'}>
        <Breadcrumb title={t('Page.MaterialRequestDetail') || 'Chi tiết yêu cầu'} items={BCrumb} />
        <BlankCard>
          <CardContent>
            <MaterialRequestDetail />
          </CardContent>
        </BlankCard>
      </PageContainer>
    </MaterialRequestProvider>
  );
};

export default MaterialRequestDetailPage;