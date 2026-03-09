import { useState, useMemo } from 'react';
import {
  Button,
  Grid,
  Stack,
  Alert,
  AlertTitle,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogContentText,
  DialogActions,
} from '@mui/material';
import LoadingButton from '@mui/lab/LoadingButton';
import { IconSend } from '@tabler/icons-react'; // Đã sửa import icon
import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';

import GeneralCard from 'src/components/products/add/GeneralCard';
import DimensionCard from 'src/components/products/add/DimensionCard';
import Thumbnail from 'src/components/products/add/Thumbnail';
import StatusCard from 'src/components/products/add/Status';
import AssetCard from 'src/components/products/add/Asset';
import ProductDetails from 'src/components/products/add/ProductDetails';
import BlankCard from 'src/components/shared/BlankCard';

import { useForm, FormProvider } from 'react-hook-form';
import { useCreateProduct } from '../hooks/useCreateProduct';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { useTranslation } from 'react-i18next';

const AddProduct = () => {
  const { t } = useTranslation();
  const { createProduct, loading } = useCreateProduct();
  const [alert, setAlert] = useState({ open: false, severity: 'success', title: '', message: '' });
  const [openDialog, setOpenDialog] = useState(false);

  const validationSchema = useMemo(() => {
    return yup.object({
      prodCode: yup
        .string()
        .trim()
        .required(`${t('Field.Code')} ${t('Message.Required')}`),
      name: yup
        .string()
        .trim()
        .required(`${t('Field.Name')} ${t('Message.Required')}`),
      categoryId: yup.string().required(t('Message.SelectCategory')),
      unitId: yup.string().required(t('Message.SelectUnit')),
      weight: yup
        .number()
        .transform((value, originalValue) => (originalValue === '' ? null : value))
        .nullable()
        .typeError(t('Message.MustBeNumber')),
      width: yup
        .number()
        .transform((value, originalValue) => (originalValue === '' ? null : value))
        .nullable()
        .typeError(t('Message.MustBeNumber')),
      height: yup
        .number()
        .transform((value, originalValue) => (originalValue === '' ? null : value))
        .nullable()
        .typeError(t('Message.MustBeNumber')),
      depth: yup
        .number()
        .transform((value, originalValue) => (originalValue === '' ? null : value))
        .nullable()
        .typeError(t('Message.MustBeNumber')),
    });
  }, [t]);

  const methods = useForm({
    resolver: yupResolver(validationSchema),
    defaultValues: {
      prodCode: '',
      name: '',
      description: '',
      image: null,
      isActivated: true,
      weight: '',
      width: '',
      height: '',
      depth: '',
      categoryId: '',
      unitId: '',
      isAsset: false,
    },
  });

  const { handleSubmit, reset } = methods;

  const BCrumb = [
    { to: '/', title: t('Menu.Home') },
    { title: t('Menu.Catalog') },
    { title: t('Menu.Product') },
    { title: t('Page.ProductAdd') },
  ];

  const onSubmit = async (data) => {
    const result = await createProduct(data);

    setAlert({
      open: true,
      severity: result.success ? 'success' : 'error',
      title: result.success ? t('Message.Success') : t('Message.Error'),
      message:
        result.message || (result.success ? t('Message.CreateSuccess') : t('Message.CreateFailed')),
    });

    if (result.success) {
      reset();
    }
  };

  const handleCancel = () => setOpenDialog(true);
  const handleDialogClose = () => setOpenDialog(false);
  const handleDialogConfirm = () => {
    reset();
    setAlert({ open: false, severity: 'success', title: '', message: '' });
    setOpenDialog(false);
  };

  return (
    <PageContainer title={t('Page.ProductAdd')} description={t('Page.ProductAdd')}>
      <Breadcrumb title={t('Page.ProductAdd')} items={BCrumb} />

      {alert.open && (
        <Stack mb={2}>
          <Alert
            variant="filled"
            severity={alert.severity}
            onClose={() => setAlert({ ...alert, open: false })}
          >
            <AlertTitle>{alert.title}</AlertTitle>
            <strong>{alert.message}</strong>
          </Alert>
        </Stack>
      )}

      <FormProvider {...methods}>
        <form onSubmit={handleSubmit(onSubmit)}>
          <Grid container spacing={3}>
            <Grid item size={{ xs: 12, lg: 8 }}>
              <Stack spacing={3}>
                <BlankCard>
                  <GeneralCard />
                </BlankCard>
                <BlankCard>
                  <DimensionCard />
                </BlankCard>
              </Stack>
            </Grid>

            <Grid item size={{ xs: 12, lg: 4 }}>
              <Stack spacing={3}>
                <BlankCard>
                  <Thumbnail />
                </BlankCard>
                <BlankCard>
                  <StatusCard />
                </BlankCard>
                <BlankCard>
                  <AssetCard />
                </BlankCard>
                <BlankCard>
                  <ProductDetails />
                </BlankCard>
              </Stack>
            </Grid>
          </Grid>

          <Stack direction="row" spacing={2} mt={3}>
            <LoadingButton
              type="submit"
              loading={loading}
              variant="contained"
              color="primary"
              endIcon={<IconSend size={18} />}
              loadingPosition="end"
            >
              {t('Action.Save')}
            </LoadingButton>

            <Button variant="outlined" color="error" onClick={handleCancel}>
              {t('Action.Cancel')}
            </Button>
          </Stack>
        </form>
      </FormProvider>

      <Dialog open={openDialog} onClose={handleDialogClose}>
        <DialogTitle>{t('Message.ConfirmCancel')}</DialogTitle>
        <DialogContent>
          <DialogContentText>{t('Message.DiscardChanges')}</DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button color="primary" onClick={handleDialogClose}>
            {t('Action.No')}
          </Button>
          <Button color="error" onClick={handleDialogConfirm} autoFocus>
            {t('Action.Yes')}
          </Button>
        </DialogActions>
      </Dialog>
    </PageContainer>
  );
};

export default AddProduct;
