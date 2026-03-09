import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';
import EditMaterialRequestApp from 'src/components/request/material/edit';
import BlankCard from 'src/components/shared/BlankCard';
import { CardContent } from '@mui/material';
import { MaterialRequestProvider } from 'src/context/MaterialRequestContext';
import { useTranslation } from 'react-i18next';

const EditMaterialRequest = () => {
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
    {
      title: t('Page.EditMaterialRequest') || 'Cập nhật yêu cầu',
    },
  ];

  return (
    <MaterialRequestProvider>
      <PageContainer title={t('Page.EditMaterialRequest') || 'Cập nhật yêu cầu'} description={t('Page.EditMaterialRequest') || 'Cập nhật yêu cầu'}>
        <Breadcrumb title={t('Page.EditMaterialRequest') || 'Cập nhật yêu cầu'} items={BCrumb} />

        <BlankCard>
          <CardContent>
            <EditMaterialRequestApp />
          </CardContent>
        </BlankCard>
      </PageContainer>
    </MaterialRequestProvider>
  );
};

export default EditMaterialRequest;