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
} from '@mui/material';
import { LoadingButton } from '@mui/lab';
import { IconSend } from '@tabler/icons';
import { useFormik } from 'formik';
import * as yup from 'yup';

import CustomTextField from 'src/components/forms/theme-elements/CustomTextField';
import TiptapEdit from 'src/views/forms/from-tiptap/TiptapEdit';
import { useCreateSupplier } from '../hooks/useCreateSupplier';

/* ================= VALIDATION ================= */
const validationSchema = yup.object({
  name: yup
    .string()
    .trim()
    .required('Supplier name không được để trống'),

  phone: yup
    .string()
    .trim()
    .matches(/^[0-9]{9,11}$/, 'Số điện thoại không hợp lệ')
    .required('Phone là bắt buộc'),

  email: yup
    .string()
    .trim()
    .email('Email không hợp lệ')
    .required('Email là bắt buộc'),
});

const AddDialog = ({ open, onClose, initialData, onSuccess }) => {
  const { createSupplier, loading } = useCreateSupplier();

  const [alert, setAlert] = React.useState({
    open: false,
    severity: 'success',
    title: '',
    message: '',
  });

  /* ================= FORMIK ================= */
  const formik = useFormik({
    enableReinitialize: true,
    initialValues: {
      name: initialData?.name || '',
      phone: initialData?.phone || '',
      email: initialData?.email || '',
      note: initialData?.note || '',
    },
    validationSchema,
    onSubmit: async (values) => {
      const payload = {
        name: values.name,
        phone: values.phone,
        email: values.email,
        note: values.note,
        isActivated: true,
      };

      const result = await createSupplier(payload);

      if (result.success) {
        setAlert({
          open: true,
          severity: 'success',
          title: 'Success',
          message: result.message || 'Tạo supplier thành công',
        });

        onSuccess?.(result.data);
        setTimeout(handleClose, 1500);
      } else {
        setAlert({
          open: true,
          severity: 'error',
          title: 'Error',
          message: result.message || 'Tạo supplier thất bại',
        });
      }
    },
  });

  /* ================= CLOSE ================= */
  const handleClose = () => {
    if (loading) return;
    formik.resetForm();
    setAlert({ open: false, severity: 'success', title: '', message: '' });
    onClose();
  };

  /* ================= RENDER ================= */
  return (
    <Dialog open={open} onClose={handleClose} maxWidth="md" fullWidth>
      <DialogTitle>Add Supplier</DialogTitle>

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
            label="Supplier Name"
            name="name"
            value={formik.values.name}
            onChange={formik.handleChange}
            error={formik.touched.name && Boolean(formik.errors.name)}
            helperText={formik.touched.name && formik.errors.name}
            fullWidth
            required
            disabled={loading}
          />

          <CustomTextField
            label="Phone"
            name="phone"
            value={formik.values.phone}
            onChange={formik.handleChange}
            error={formik.touched.phone && Boolean(formik.errors.phone)}
            helperText={formik.touched.phone && formik.errors.phone}
            fullWidth
            required
            disabled={loading}
          />

          <CustomTextField
            label="Email"
            name="email"
            value={formik.values.email}
            onChange={formik.handleChange}
            error={formik.touched.email && Boolean(formik.errors.email)}
            helperText={formik.touched.email && formik.errors.email}
            fullWidth
            required
            disabled={loading}
          />

          <TiptapEdit
            value={formik.values.note}
            onChange={(value) => formik.setFieldValue('note', value)}
          />
        </Box>
      </DialogContent>

      <DialogActions>
        <Button color="error" onClick={handleClose} disabled={loading}>
          Cancel
        </Button>

        <LoadingButton
          loading={loading}
          variant="contained"
          onClick={formik.submitForm}
          endIcon={<IconSend size={18} />}
          loadingPosition="end"
        >
          Save
        </LoadingButton>
      </DialogActions>
    </Dialog>
  );
};

export default AddDialog;
