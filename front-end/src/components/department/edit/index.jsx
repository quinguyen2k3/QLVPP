import React, { useState, useEffect } from 'react';
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

import { departmentApi } from 'src/api/department/departmentApi';

const EditDialog = ({ departmentId, open, onClose, onSuccess }) => {
  const { t } = useTranslation();

  const [department, setDepartment] = useState(null);
  const [loading, setLoading] = useState(false);

  const [alert, setAlert] = useState({
    open: false,
    severity: 'success',
    title: '',
    message: '',
  });

  useEffect(() => {
    const fetchDetail = async () => {
      if (open && departmentId) {
        setLoading(true);
        try {
          const response = await departmentApi.getById(departmentId);
          setDepartment(response.data || response);
        } catch (error) {
          setAlert({
            open: true,
            severity: 'error',
            title: t('Message.Error'),
            message: t('Message.LoadDetailError') || 'Không tải được dữ liệu',
          });
        } finally {
          setLoading(false);
        }
      }
    };

    fetchDetail();
  }, [open, departmentId, t]);

  const validationSchema = yup.object({
    name: yup
      .string()
      .trim()
      .required(t('Placeholder.Enter') + ' ' + t('Field.Name')),
  });

  const formik = useFormik({
    enableReinitialize: true,
    initialValues: {
      name: department?.name || '',
      note: department?.note || '',
      isActivated: department?.isActivated ?? true,
    },
    validationSchema,
    onSubmit: async (values) => {
      setLoading(true);
      const payload = {
        name: values.name,
        note: values.note,
        isActivated: values.isActivated,
      };

      try {
        const result = await departmentApi.update(departmentId, payload);

        setAlert({
          open: true,
          severity: 'success',
          title: t('Message.Success'),
          message: t('Message.UpdateSuccess') || 'Cập nhật thành công',
        });

        if (onSuccess) {
          onSuccess(result.data || result);
        }

        setTimeout(handleClose, 1500);
      } catch (error) {
        setAlert({
          open: true,
          severity: 'error',
          title: t('Message.Error'),
          message: error.response?.data?.message || t('Message.ErrorOccurred'),
        });
      } finally {
        setLoading(false);
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
        {t('Action.Update')} {t('Menu.Department')}
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
          mt={1}
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
            <Typography
              variant="subtitle1"
              fontWeight={600}
              component="label"
              htmlFor="note"
              mb="5px"
            >
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
