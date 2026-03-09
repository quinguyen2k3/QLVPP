import React from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Box,
  Stack,
  Alert,
  AlertTitle,
  Switch,
  FormControlLabel,
  Typography,
} from '@mui/material';
import { LoadingButton } from '@mui/lab';
import { IconSend } from '@tabler/icons-react';
import { useFormik } from 'formik';
import * as yup from 'yup';
import { useTranslation } from 'react-i18next';

import CustomTextField from 'src/components/forms/theme-elements/CustomTextField';
import TiptapEdit from 'src/views/forms/from-tiptap/TiptapEdit';
import { useGetOneWarehouse } from '../hooks/useGetOneWarehouse';
import { useUpdateWarehouse } from '../hooks/useUpdateWarehouse';

const EditDialog = ({ warehouseId, open, onClose, onSuccess }) => {
  const { t } = useTranslation();
  const { warehouse, loading: loadingGet } = useGetOneWarehouse(warehouseId, open);
  const { updateWarehouse, loading: updating } = useUpdateWarehouse();

  const [alert, setAlert] = React.useState({
    open: false,
    severity: 'success',
    title: '',
    message: '',
  });

  const loading = loadingGet || updating;

  const validationSchema = yup.object({
    name: yup
      .string()
      .trim()
      .required(t('Placeholder.Enter') + ' ' + t('Field.Name')),
  });

  const formik = useFormik({
    enableReinitialize: true,
    initialValues: {
      name: warehouse?.name || '',
      note: warehouse?.note || '',
      isActivated: warehouse?.isActivated ?? true,
    },
    validationSchema,
    onSubmit: async (values) => {
      const payload = {
        name: values.name,
        note: values.note,
        isActivated: values.isActivated,
      };

      const result = await updateWarehouse(warehouseId, payload);

      if (result.success) {
        setAlert({
          open: true,
          severity: 'success',
          title: t('Message.Success'),
          message: result.message || t('Message.Success'),
        });

        onSuccess?.(result.data);
        setTimeout(handleClose, 1500);
      } else {
        setAlert({
          open: true,
          severity: 'error',
          title: t('Message.Error'),
          message: result.message || t('Message.Error'),
        });
      }
    },
  });

  const handleClose = () => {
    if (loading) return;
    formik.resetForm();
    setAlert({ open: false, severity: 'success', title: '', message: '' });
    onClose();
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="md" fullWidth>
      <DialogTitle>
        {t('Action.Update')} {t('Menu.Warehouse')}
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
        </Stack>

        <Box
          component="form"
          mt={2}
          display="flex"
          flexDirection="column"
          gap={2}
          onSubmit={formik.handleSubmit}
        >
          <CustomTextField
            autoFocus
            label={t('Field.Name')}
            name="name"
            value={formik.values.name}
            onChange={formik.handleChange}
            error={formik.touched.name && Boolean(formik.errors.name)}
            helperText={formik.touched.name && formik.errors.name}
            fullWidth
            required
            disabled={loading}
            placeholder={t('Placeholder.Enter')}
          />

          <Box>
            <Typography variant="subtitle1" fontWeight={600} component="label" htmlFor="note" mb="5px">
              {t('Field.Note')}
            </Typography>
            <TiptapEdit
              value={formik.values.note}
              onChange={(value) => formik.setFieldValue('note', value)}
            />
          </Box>

          <Box display="flex" justifyContent="flex-start">
            <FormControlLabel
              control={
                <Switch
                  name="isActivated"
                  checked={formik.values.isActivated}
                  onChange={(e) => formik.setFieldValue('isActivated', e.target.checked)}
                  color="primary"
                  disabled={loading}
                />
              }
              label={t('Status.Active')}
            />
          </Box>
        </Box>
      </DialogContent>

      <DialogActions>
        <Button color="error" onClick={handleClose} disabled={loading}>
          {t('Action.Cancel')}
        </Button>

        <LoadingButton
          loading={loading}
          variant="contained"
          onClick={formik.submitForm}
          endIcon={<IconSend size={18} />}
          loadingPosition="end"
        >
          {t('Action.Update')}
        </LoadingButton>
      </DialogActions>
    </Dialog>
  );
};

export default EditDialog;