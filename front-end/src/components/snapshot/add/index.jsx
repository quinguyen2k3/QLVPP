import React from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogContentText, // Import thêm cái này để hiển thị text đẹp hơn
  DialogActions,
  Button,
  Box,
  Stack,
  Alert,
  AlertTitle,
  Autocomplete,
  CircularProgress,
} from '@mui/material';
import { LoadingButton } from '@mui/lab';
import { IconSend, IconAlertCircle } from '@tabler/icons-react'; // Import icon cảnh báo
import { useFormik } from 'formik';
import * as yup from 'yup';
import { useTranslation } from 'react-i18next';

import { format } from 'date-fns';

import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';

import CustomTextField from 'src/components/forms/theme-elements/CustomTextField';
import CustomFormLabel from 'src/components/forms/theme-elements/CustomFormLabel';

import { inventoryApi } from 'src/api/inventory/inventoryApi';
import { useWarehouses } from 'src/hooks/useMasterData';

const AddDialog = ({ open, onClose, onSuccess }) => {
  const { t } = useTranslation();
  const { warehouses, isLoading } = useWarehouses();

  const [loading, setLoading] = React.useState(false);
  
  const [openConfirm, setOpenConfirm] = React.useState(false);

  const [alert, setAlert] = React.useState({
    open: false,
    severity: 'success',
    title: '',
    message: '',
  });

  const validationSchema = yup.object({
    warehouseId: yup.string().required(`${t('Placeholder.Select')} ${t('Menu.Warehouse')}`),
    fromDate: yup
      .date()
      .nullable()
      .required(`${t('Placeholder.Select')} ${t('Field.FromDate')}`)
      .typeError(t('Message.InvalidDate')),
    toDate: yup
      .date()
      .nullable()
      .required(`${t('Placeholder.Select')} ${t('Field.ToDate')}`)
      .min(yup.ref('fromDate'), t('Message.EndDateMustBeAfterStartDate'))
      .typeError(t('Message.InvalidDate')),
  });

  const formik = useFormik({
    enableReinitialize: false,
    initialValues: {
      warehouseId: '',
      fromDate: null,
      toDate: new Date(),
    },
    validationSchema,
    onSubmit: async (values) => {
      console.log('--- Start Submit ---');
      setLoading(true);
      try {
        const payload = {
          warehouseId: values.warehouseId,
          fromDate: values.fromDate ? format(new Date(values.fromDate), 'yyyy-MM-dd') : null,
          toDate: values.toDate ? format(new Date(values.toDate), 'yyyy-MM-dd') : null,
        };

        console.log('Payload sending API:', payload);

        await inventoryApi.create(payload);

        setAlert({
          open: true,
          severity: 'success',
          title: t('Message.Success'),
          message: t('Message.SnapshotCreatedSuccess'),
        });

        if (onSuccess) onSuccess();
        setTimeout(handleClose, 1500);
      } catch (error) {
        console.error('API Error:', error);
        setAlert({
          open: true,
          severity: 'error',
          title: t('Message.Error'),
          message: error.response?.data?.message || t('Message.Error'),
        });
      } finally {
        setLoading(false);
      }
    },
  });

  const handlePreSubmit = async () => {
    const errors = await formik.validateForm();
    
    formik.setTouched({
      warehouseId: true,
      fromDate: true,
      toDate: true,
    });

    if (Object.keys(errors).length === 0) {
      setOpenConfirm(true);
    }
  };

  const handleConfirmYes = () => {
    setOpenConfirm(false);
    formik.handleSubmit();
  };

  const handleClose = () => {
    if (loading) return;
    formik.resetForm();
    setAlert({ open: false, severity: 'success', title: '', message: '' });
    onClose();
  };

  return (
    <LocalizationProvider dateAdapter={AdapterDateFns}>
      <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
        <DialogTitle>
          {t('Action.Create')} {t('Entity.Report')}
        </DialogTitle>

        <DialogContent>
          <Stack spacing={2}>
            {alert.open && (
              <Alert
                variant="filled"
                severity={alert.severity}
                onClose={() => setAlert({ ...alert, open: false })}
              >
                <AlertTitle>{alert.title}</AlertTitle>
                {alert.message}
              </Alert>
            )}

            <Box
              component="form"
              mt={1}
              display="flex"
              flexDirection="column"
              gap={2}
            >
              <Box>
                <CustomFormLabel htmlFor="warehouseId">
                  {t('Menu.Warehouse')} <span style={{ color: 'red' }}>*</span>
                </CustomFormLabel>

                <Autocomplete
                  fullWidth
                  id="warehouseId"
                  loading={isLoading}
                  options={warehouses || []}
                  getOptionLabel={(option) => option.name || ''}
                  isOptionEqualToValue={(option, value) => option.id === value.id}
                  value={warehouses?.find((w) => w.id === formik.values.warehouseId) || null}
                  onChange={(event, newValue) => {
                    formik.setFieldValue('warehouseId', newValue ? newValue.id : '');
                  }}
                  renderInput={(params) => (
                    <CustomTextField
                      {...params}
                      placeholder={`${t('Placeholder.Select')} ${t('Menu.Warehouse')}`}
                      error={formik.touched.warehouseId && Boolean(formik.errors.warehouseId)}
                      helperText={formik.touched.warehouseId && formik.errors.warehouseId}
                      InputProps={{
                        ...params.InputProps,
                        endAdornment: (
                          <React.Fragment>
                            {isLoading ? <CircularProgress color="inherit" size={20} /> : null}
                            {params.InputProps.endAdornment}
                          </React.Fragment>
                        ),
                      }}
                    />
                  )}
                />
              </Box>

              <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
                <Box flex={1}>
                  <CustomFormLabel htmlFor="fromDate">
                    {t('Field.FromDate')} <span style={{ color: 'red' }}>*</span>
                  </CustomFormLabel>
                  <DatePicker
                    value={formik.values.fromDate}
                    onChange={(value) => formik.setFieldValue('fromDate', value)}
                    slotProps={{
                      textField: {
                        fullWidth: true,
                        size: 'small',
                        error: formik.touched.fromDate && Boolean(formik.errors.fromDate),
                        helperText: formik.touched.fromDate && formik.errors.fromDate,
                      },
                    }}
                  />
                </Box>

                <Box flex={1}>
                  <CustomFormLabel htmlFor="toDate">
                    {t('Field.ToDate')} <span style={{ color: 'red' }}>*</span>
                  </CustomFormLabel>
                  <DatePicker
                    value={formik.values.toDate}
                    onChange={(value) => formik.setFieldValue('toDate', value)}
                    slotProps={{
                      textField: {
                        fullWidth: true,
                        size: 'small',
                        error: formik.touched.toDate && Boolean(formik.errors.toDate),
                        helperText: formik.touched.toDate && formik.errors.toDate,
                      },
                    }}
                  />
                </Box>
              </Stack>
            </Box>
          </Stack>
        </DialogContent>

        <DialogActions sx={{ p: 3 }}>
          <Button color="error" variant="outlined" onClick={handleClose} disabled={loading}>
            {t('Action.Cancel')}
          </Button>

          <LoadingButton
            loading={loading}
            variant="contained"
            onClick={handlePreSubmit}
            endIcon={<IconSend size={18} />}
            loadingPosition="end"
          >
            {t('Action.Save')}
          </LoadingButton>
        </DialogActions>
      </Dialog>

      <Dialog
        open={openConfirm}
        onClose={() => setOpenConfirm(false)}
        maxWidth="xs"
        fullWidth
      >
        <DialogTitle sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          <IconAlertCircle color="orange" />
          {t('Action.Confirm') || 'Xác nhận thao tác'}
        </DialogTitle>
        <DialogContent>
          <DialogContentText>
            {t('Message.ConfirmCreateReport') || 'Bạn có chắc chắn muốn tạo phiếu kiểm kê này không? Hành động này sẽ chốt số liệu kho tại thời điểm đã chọn.'}
          </DialogContentText>
        </DialogContent>
        <DialogActions sx={{ p: 2 }}>
          <Button onClick={() => setOpenConfirm(false)} color="inherit">
            {t('Action.Cancel') || 'Hủy bỏ'}
          </Button>
          <Button onClick={handleConfirmYes} variant="contained" color="primary" autoFocus>
            {t('Action.Confirm') || 'Đồng ý'}
          </Button>
        </DialogActions>
      </Dialog>
    </LocalizationProvider>
  );
};

export default AddDialog;