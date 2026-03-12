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
  Autocomplete,
  Grid,
} from '@mui/material';
import { LoadingButton } from '@mui/lab';
import { IconSend } from '@tabler/icons-react';
import { useFormik } from 'formik';
import * as yup from 'yup';
import { useTranslation } from 'react-i18next';

import CustomTextField from 'src/components/forms/theme-elements/CustomTextField';

import { employeeApi } from 'src/api/employee/employeeApi';
import { useMasterData } from 'src/hooks/useMasterData';

const EditDialog = ({ employeeId, open, onClose, onSuccess }) => {
  const { t } = useTranslation();
  const { departments, warehouses, positions, roles } = useMasterData();

  const [employee, setEmployee] = useState(null);
  const [loading, setLoading] = useState(false);

  const [alert, setAlert] = useState({
    open: false,
    severity: 'success',
    title: '',
    message: '',
  });

  useEffect(() => {
    const fetchDetail = async () => {
      if (open && employeeId) {
        setLoading(true);
        try {
          const response = await employeeApi.getById(employeeId);
          setEmployee(response.data || response);
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
  }, [open, employeeId, t]);

  const validationSchema = yup.object({
    name: yup
      .string()
      .trim()
      .required(t('Placeholder.Enter') + ' ' + t('Field.Name')),
    email: yup
      .string()
      .email(t('Message.InvalidEmail') || 'Email không hợp lệ')
      .required(t('Placeholder.Enter') + ' ' + t('Field.Email')),
    phone: yup
      .string()
      .trim()
      .required(t('Placeholder.Enter') + ' ' + t('Field.Phone')),
    account: yup
      .string()
      .trim()
      .required(t('Placeholder.Enter') + ' ' + t('Field.Account')),
    password: yup.string().trim(),
    departmentId: yup
      .number()
      .nullable()
      .required(t('Placeholder.Select') + ' ' + t('Field.Department')),
    positionId: yup
      .number()
      .nullable()
      .required(t('Placeholder.Select') + ' ' + t('Field.Position')),
    warehouseId: yup
      .number()
      .nullable()
      .required(t('Placeholder.Select') + ' ' + t('Field.Warehouse')),
    roleId: yup
      .number()
      .nullable()
      .required(t('Placeholder.Select') + ' ' + t('Field.Role')),
  });

  const formik = useFormik({
    enableReinitialize: true,
    initialValues: {
      name: employee?.name || '',
      email: employee?.email || '',
      phone: employee?.phone || '',
      account: employee?.account || '',
      password: '',
      departmentId: employee?.departmentId || null,
      positionId: employee?.positionId || null,
      warehouseId: employee?.warehouseId || null,
      roleId: employee?.roleId || null,
      isActivated: employee?.isActivated ?? true,
    },
    validationSchema,
    onSubmit: async (values) => {
      setLoading(true);
      const payload = {
        name: values.name,
        email: values.email,
        phone: values.phone,
        account: values.account,
        password: values.password,
        departmentId: values.departmentId,
        positionId: values.positionId,
        warehouseId: values.warehouseId,
        roleId: values.roleId,
        isActivated: values.isActivated,
      };

      try {
        const result = await employeeApi.update(employeeId, payload);

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
        {t('Action.Update')} {t('Menu.Employee') || 'Employee'}
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

        <Box component="form" mt={2} onSubmit={formik.handleSubmit}>
          <Grid container spacing={2}>
            <Grid item size={{ xs: 12, sm: 6 }}>
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
            </Grid>

            <Grid item size={{ xs: 12, sm: 6 }}>
              <CustomTextField
                label={t('Field.Email')}
                name="email"
                type="email"
                value={formik.values.email}
                onChange={formik.handleChange}
                error={formik.touched.email && Boolean(formik.errors.email)}
                helperText={formik.touched.email && formik.errors.email}
                fullWidth
                required
                disabled={loading}
                placeholder={t('Placeholder.Enter')}
              />
            </Grid>

            <Grid item size={{ xs: 12, sm: 6 }}>
              <CustomTextField
                label={t('Field.Phone')}
                name="phone"
                value={formik.values.phone}
                onChange={formik.handleChange}
                error={formik.touched.phone && Boolean(formik.errors.phone)}
                helperText={formik.touched.phone && formik.errors.phone}
                fullWidth
                required
                disabled={loading}
                placeholder={t('Placeholder.Enter')}
              />
            </Grid>

            <Grid item size={{ xs: 12, sm: 6 }}>
              <CustomTextField
                label={t('Field.Account')}
                name="account"
                value={formik.values.account}
                onChange={formik.handleChange}
                error={formik.touched.account && Boolean(formik.errors.account)}
                helperText={formik.touched.account && formik.errors.account}
                fullWidth
                required
                disabled={loading}
                placeholder={t('Placeholder.Enter')}
              />
            </Grid>

            <Grid item size={{ xs: 12 }}>
              <CustomTextField
                label={t('Field.Password')}
                name="password"
                type="password"
                value={formik.values.password}
                onChange={formik.handleChange}
                error={formik.touched.password && Boolean(formik.errors.password)}
                helperText={
                  formik.touched.password && formik.errors.password
                    ? formik.errors.password
                    : t('Message.LeaveBlankToKeepPassword') ||
                      'Bỏ trống nếu không muốn đổi mật khẩu'
                }
                fullWidth
                disabled={loading}
                placeholder={t('Placeholder.Enter')}
              />
            </Grid>

            <Grid item size={{ xs: 12, sm: 6 }}>
              <Autocomplete
                options={roles || []}
                getOptionLabel={(option) => option.name || ''}
                value={roles?.find((r) => r.id === formik.values.roleId) || null}
                onChange={(e, newValue) => {
                  formik.setFieldValue('roleId', newValue ? newValue.id : null);
                }}
                disabled={loading}
                renderInput={(params) => (
                  <CustomTextField
                    {...params}
                    label={t('Field.Role')}
                    required
                    error={formik.touched.roleId && Boolean(formik.errors.roleId)}
                    helperText={formik.touched.roleId && formik.errors.roleId}
                    placeholder={t('Placeholder.Select')}
                  />
                )}
              />
            </Grid>

            <Grid item size={{ xs: 12, sm: 6 }}>
              <Autocomplete
                options={departments || []}
                getOptionLabel={(option) => option.name || ''}
                value={departments?.find((d) => d.id === formik.values.departmentId) || null}
                onChange={(e, newValue) => {
                  formik.setFieldValue('departmentId', newValue ? newValue.id : null);
                }}
                disabled={loading}
                renderInput={(params) => (
                  <CustomTextField
                    {...params}
                    label={t('Field.Department')}
                    required
                    error={formik.touched.departmentId && Boolean(formik.errors.departmentId)}
                    helperText={formik.touched.departmentId && formik.errors.departmentId}
                    placeholder={t('Placeholder.Select')}
                  />
                )}
              />
            </Grid>

            <Grid item size={{ xs: 12, sm: 6 }}>
              <Autocomplete
                options={positions || []}
                getOptionLabel={(option) => option.name || ''}
                value={positions?.find((p) => p.id === formik.values.positionId) || null}
                onChange={(e, newValue) => {
                  formik.setFieldValue('positionId', newValue ? newValue.id : null);
                }}
                disabled={loading}
                renderInput={(params) => (
                  <CustomTextField
                    {...params}
                    label={t('Field.Position')}
                    required
                    error={formik.touched.positionId && Boolean(formik.errors.positionId)}
                    helperText={formik.touched.positionId && formik.errors.positionId}
                    placeholder={t('Placeholder.Select')}
                  />
                )}
              />
            </Grid>

            <Grid item size={{ xs: 12, sm: 6 }}>
              <Autocomplete
                options={warehouses || []}
                getOptionLabel={(option) => option.name || ''}
                value={warehouses?.find((w) => w.id === formik.values.warehouseId) || null}
                onChange={(e, newValue) => {
                  formik.setFieldValue('warehouseId', newValue ? newValue.id : null);
                }}
                disabled={loading}
                renderInput={(params) => (
                  <CustomTextField
                    {...params}
                    label={t('Field.Warehouse')}
                    required
                    error={formik.touched.warehouseId && Boolean(formik.errors.warehouseId)}
                    helperText={formik.touched.warehouseId && formik.errors.warehouseId}
                    placeholder={t('Placeholder.Select')}
                  />
                )}
              />
            </Grid>

            <Grid item size={{ xs: 12 }}>
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
            </Grid>
          </Grid>
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
