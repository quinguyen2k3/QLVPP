import { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
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
import { IconSend } from '@tabler/icons';
import Breadcrumb from 'src/layouts/full/shared/breadcrumb/Breadcrumb';
import PageContainer from 'src/components/container/PageContainer';

import GeneralCard from 'src/components/products/edit/GeneralCard';
import DimensionCard from 'src/components/products/edit/DimensionCard';
import Thumbnail from 'src/components/products/edit/Thumbnail';
import StatusCard from 'src/components/products/edit/Status';
import AssetCard from 'src/components/products/edit/Asset';
import ProductDetails from 'src/components/products/edit/ProductDetails';
import BlankCard from 'src/components/shared/BlankCard';

import { useForm, FormProvider } from 'react-hook-form';
import { useParams } from 'react-router-dom';
import { useUpdateProduct } from '../hooks/useUpdateProduct';
import { useGetOneProduct } from '../hooks/useGetOneProduct';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';

const validationSchema = yup.object({
  prodCode: yup.string().trim().required('Mã sản phẩm không được để trống'),
  name: yup.string().trim().required('Tên sản phẩm không được để trống'),
  categoryId: yup.string().nullable().required('Vui lòng chọn danh mục'),
  unitId: yup.string().nullable().required('Vui lòng chọn đơn vị'),
  weight: yup
    .number()
    .transform((value, originalValue) => (originalValue === '' ? null : value))
    .nullable()
    .typeError('Phải là số'),
  width: yup
    .number()
    .transform((value, originalValue) => (originalValue === '' ? null : value))
    .nullable()
    .typeError('Phải là số'),
  height: yup
    .number()
    .transform((value, originalValue) => (originalValue === '' ? null : value))
    .nullable()
    .typeError('Phải là số'),
  depth: yup
    .number()
    .transform((value, originalValue) => (originalValue === '' ? null : value))
    .nullable()
    .typeError('Phải là số'),
});

const EditProduct = () => {
  const { t } = useTranslation();

  const BCrumb = [
    { to: '/', title: t('Menu.Home') },
    { title: t('Menu.Catalog') },
    { title: t('Menu.Product') },
    { title: t('Page.ProductEdit') },
  ];
  
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
      imagePath: '',
    },
  });

  const {
    handleSubmit,
    reset,
    getValues,
    formState: { errors },
  } = methods;
  const { updateProduct, loading } = useUpdateProduct();

  const [alert, setAlert] = useState({ open: false, severity: 'success', title: '', message: '' });
  const [openCancelDialog, setOpenCancelDialog] = useState(false);
  const [openUpdateDialog, setOpenUpdateDialog] = useState(false);

  const { id: productId } = useParams();
  const { product, loading: loadingData, error } = useGetOneProduct(productId);

  useEffect(() => {
    if (product) {
      reset({
        prodCode: product.prodCode || '',
        name: product.name || '',
        description: product.description || '',
        image: product.image || null,
        isActivated: product.isActivated ?? true,
        weight: product.weight || '',
        width: product.width || '',
        height: product.height || '',
        depth: product.depth || '',
        categoryId: product.categoryId || '',
        unitId: product.unitId || '',
        isAsset: product.isAsset ?? false,
        imagePath: product.imagePath || '',
      });
    }
  }, [product, reset]);

  const onSubmitAPI = async (data) => {
    try {
      const result = await updateProduct(productId, data);
      setAlert({
        open: true,
        severity: result.success ? 'success' : 'error',
        title: result.success ? t('Message.Success') : t('Message.Error'),
        message:
          result.message ||
          (result.success ? t('Message.UpdateSuccess') : t('Message.UpdateFailed')),
      });
      return result.success;
    } catch (err) {
      setAlert({
        open: true,
        severity: 'error',
        title: t('Message.Error'),
        message: t('Message.ErrorUpdating'),
      });
      return false;
    }
  };

  const onPreSubmit = (data) => {
    setOpenUpdateDialog(true);
  };

  const handleUpdateConfirm = async () => {
    const data = getValues();
    await onSubmitAPI(data);
    setOpenUpdateDialog(false);
  };

  const handleCancel = () => setOpenCancelDialog(true);
  const handleCancelClose = () => setOpenCancelDialog(false);
  const handleCancelConfirm = () => {
    if (product) reset(product);
    setAlert({ open: false, severity: 'success', title: '', message: '' });
    setOpenCancelDialog(false);
  };

  const handleUpdateClose = () => setOpenUpdateDialog(false);

  return (
    <PageContainer title={t('Page.ProductEdit')} description={t('Page.ProductEdit')}>
      <Breadcrumb title={t('Page.ProductEdit')} items={BCrumb} />

      {error && (
        <Stack mb={2}>
          <Alert variant="filled" severity="error">
            <AlertTitle>{t('Message.ErrorLoadingData')}</AlertTitle>
            {t('Message.CannotLoadProductData')}
          </Alert>
        </Stack>
      )}

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
        <form onSubmit={handleSubmit(onPreSubmit)}>
          <Grid container spacing={3}>
            <Grid size={{ lg: 8 }}>
              <Stack spacing={3}>
                <BlankCard>
                  <GeneralCard />
                </BlankCard>
                <BlankCard>
                  <DimensionCard />
                </BlankCard>
              </Stack>
            </Grid>

            <Grid size={{ lg: 4 }}>
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
              loading={loading}
              variant="contained"
              color="primary"
              endIcon={<IconSend size={18} />}
              loadingPosition="end"
              onClick={handleSubmit(onPreSubmit)}
            >
              Update
            </LoadingButton>

            <Button variant="outlined" color="error" onClick={handleCancel}>
              Cancel
            </Button>
          </Stack>
        </form>
      </FormProvider>

      <Dialog open={openCancelDialog} onClose={handleCancelClose}>
        <DialogTitle>Hủy thay đổi?</DialogTitle>
        <DialogContent>
          <DialogContentText>{t('Message.ConfirmCancel')}</DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button color="primary" onClick={handleCancelClose}>
            {t('Action.No')}
          </Button>
          <Button color="error" onClick={handleCancelConfirm} autoFocus>
            {t('Action.Yes')}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Dialog Cập nhật (Update) */}
      <Dialog open={openUpdateDialog} onClose={handleUpdateClose}>
        <DialogTitle>{t('Message.ConfirmUpdate')}</DialogTitle>
        <DialogContent>
          <DialogContentText>{t('Message.ConfirmUpdate')}</DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button color="error" onClick={handleUpdateClose}>
            {t('Action.No')}
          </Button>

          <LoadingButton loading={loading} color="primary" onClick={handleUpdateConfirm} autoFocus>
            {t('Action.Yes')}
          </LoadingButton>
        </DialogActions>
      </Dialog>
    </PageContainer>
  );
};

export default EditProduct;
