import React, { useState } from 'react';
import { useFormik } from 'formik';
import * as yup from 'yup';
import { useTranslation } from 'react-i18next';
import {
  Box,
  Button,
  Typography,
  InputAdornment,
  IconButton,
  Stack,
  Card,
  CardContent,
  Snackbar,
  Alert,
} from '@mui/material';
import { IconEye, IconEyeOff, IconCheck, IconX } from '@tabler/icons-react';
import CustomTextField from 'src/components/forms/theme-elements/CustomTextField';
import { changePasswordApi } from 'src/api/authentication/authApi';

const ChangePassword = () => {
  const { t } = useTranslation();
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

  const [toast, setToast] = useState({
    open: false,
    message: '',
    severity: 'success',
  });

  const handleCloseToast = (event, reason) => {
    if (reason === 'clickaway') return;
    setToast({ ...toast, open: false });
  };

  const validationSchema = yup.object({
    newPassword: yup
      .string()
      .required(t('Field.NewPassword') + ' ' + t('Message.Required'))
      .matches(/[a-z]/, t('Message.RequireLower'))
      .matches(/[A-Z]/, t('Message.RequireUpper'))
      .matches(/[0-9]/, t('Message.RequireNumber'))
      .matches(/[@$!%*?&#]/, t('Message.RequireSpecial'))
      .min(8, t('Message.MinLength8')),
    confirmPassword: yup
      .string()
      .required(t('Field.ConfirmPassword') + ' ' + t('Message.Required'))
      .oneOf([yup.ref('newPassword'), null], t('Message.PasswordMismatch')),
  });

  const formik = useFormik({
    initialValues: { newPassword: '', confirmPassword: '' },
    validationSchema,
    onSubmit: async (values) => {
      try {
        await changePasswordApi({
          newPassword: values.newPassword,
          confirmPassword: values.confirmPassword,
        });
        setToast({
          open: true,
          message: t('Message.Success') || 'Đổi mật khẩu thành công!',
          severity: 'success',
        });
        formik.resetForm();
      } catch (error) {
        setToast({
          open: true,
          message: error?.response?.data?.message || t('Message.Error') || 'Có lỗi xảy ra',
          severity: 'error',
        });
      }
    },
  });
  const passwordValue = formik.values.newPassword;
  const requirements = [
    { label: t('Require.MinLength'), met: passwordValue.length >= 8 },
    { label: t('Require.Upper'), met: /[A-Z]/.test(passwordValue) },
    { label: t('Require.Lower'), met: /[a-z]/.test(passwordValue) },
    { label: t('Require.Number'), met: /[0-9]/.test(passwordValue) },
    { label: t('Require.Special'), met: /[@$!%*?&#]/.test(passwordValue) },
  ];

  return (
    <Box display="flex" justifyContent="center" alignItems="center" minHeight="60vh">
      <Snackbar
        open={toast.open}
        autoHideDuration={5000}
        onClose={handleCloseToast}
        anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
      >
        <Alert onClose={handleCloseToast} severity={toast.severity} variant="filled" sx={{ width: '100%' }}>
          {toast.message}
        </Alert>
      </Snackbar>

      <Card sx={{ maxWidth: 450, width: '100%', boxShadow: 3 }}>
        <CardContent sx={{ p: 4 }}>
          <Typography variant="h4" fontWeight={700} textAlign="center" mb={3}>
            {t('Page.ChangePassword')}
          </Typography>
          <Typography variant="body2" color="text.secondary" textAlign="center" mb={4}>
            {t('Description.ChangePassword')}
          </Typography>

          <Box component="form" onSubmit={formik.handleSubmit}>
            <Stack spacing={3}>
              <Box>
                <Typography variant="subtitle1" fontWeight={600} mb={1}>
                  {t('Field.NewPassword')}
                </Typography>
                <CustomTextField
                  fullWidth
                  name="newPassword"
                  type={showPassword ? 'text' : 'password'}
                  value={formik.values.newPassword}
                  onChange={formik.handleChange}
                  onBlur={formik.handleBlur}
                  error={formik.touched.newPassword && Boolean(formik.errors.newPassword)}
                  InputProps={{
                    endAdornment: (
                      <InputAdornment position="end">
                        <IconButton onClick={() => setShowPassword(!showPassword)} edge="end">
                          {showPassword ? <IconEye size={20} /> : <IconEyeOff size={20} />}
                        </IconButton>
                      </InputAdornment>
                    ),
                  }}
                />
              </Box>

              <Box sx={{ bgcolor: 'grey.50', p: 2, borderRadius: 2, border: '1px solid', borderColor: 'grey.200' }}>
                <Typography variant="subtitle2" fontWeight={600} mb={1}>
                  {t('Field.PasswordRequirements')}
                </Typography>
                <Stack spacing={0.5}>
                  {requirements.map((req, index) => (
                    <Stack key={index} direction="row" alignItems="center" spacing={1}>
                      {req.met ? <IconCheck size={16} color="green" /> : <IconX size={16} color="red" />}
                      <Typography variant="body2" color={req.met ? 'success.main' : 'error.main'}>
                        {req.label}
                      </Typography>
                    </Stack>
                  ))}
                </Stack>
              </Box>

              <Box>
                <Typography variant="subtitle1" fontWeight={600} mb={1}>
                  {t('Field.ConfirmPassword')}
                </Typography>
                <CustomTextField
                  fullWidth
                  name="confirmPassword"
                  type={showConfirmPassword ? 'text' : 'password'}
                  value={formik.values.confirmPassword}
                  onChange={formik.handleChange}
                  onBlur={formik.handleBlur}
                  error={formik.touched.confirmPassword && Boolean(formik.errors.confirmPassword)}
                  helperText={formik.touched.confirmPassword && formik.errors.confirmPassword}
                  InputProps={{
                    endAdornment: (
                      <InputAdornment position="end">
                        <IconButton onClick={() => setShowConfirmPassword(!showConfirmPassword)} edge="end">
                          {showConfirmPassword ? <IconEye size={20} /> : <IconEyeOff size={20} />}
                        </IconButton>
                      </InputAdornment>
                    ),
                  }}
                />
              </Box>

              <Button
                color="primary"
                variant="contained"
                size="large"
                fullWidth
                type="submit"
                disabled={!(formik.isValid && formik.values.newPassword && formik.values.confirmPassword)}
              >
                {t('Action.Confirm')}
              </Button>
            </Stack>
          </Box>
        </CardContent>
      </Card>
    </Box>
  );
};

export default ChangePassword;