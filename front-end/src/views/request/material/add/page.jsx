import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';
import BlankCard from 'src/components/shared/BlankCard';
import { CardContent } from '@mui/material';

import CreateMaterialRequestApp from 'src/components/request/material/add';
import { MaterialRequestProvider } from 'src/context/MaterialRequestContext';
import { useTranslation } from 'react-i18next';

const CreateMaterialRequest = (type = 1) => {
  const { t } = useTranslation();

  const getPageTitle = () => {
    switch (type) {
      case 1:
        return t('RequestType.Issue');
      case 2:
        return t('RequestType.Return');
      default:
        return t('Page.AddMaterialRequest');
    }
  };

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
      title: getPageTitle(),
    },
  ];

  return (
    <MaterialRequestProvider>
      <PageContainer title={getPageTitle()} description={getPageTitle()}>
        <Breadcrumb title={getPageTitle()} items={BCrumb} />

        <BlankCard>
          <CardContent>
            <CreateMaterialRequestApp />
          </CardContent>
        </BlankCard>
      </PageContainer>
    </MaterialRequestProvider>
  );
};

export default CreateMaterialRequest;
