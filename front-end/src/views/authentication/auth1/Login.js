import React, { useState, useContext } from 'react';
import { Link } from 'react-router-dom'; // Đã sửa lại đúng import từ react-router-dom
import { Grid, Box, Stack, Typography, Alert } from '@mui/material';
import PageContainer from 'src/components/container/PageContainer';
import img1 from 'src/assets/images/backgrounds/login-bg.svg';
import Logo from 'src/layouts/full/shared/logo/Logo';
import AuthLogin from '../authForms/AuthLogin';
import { useLogin } from '../hooks/useLogin';
import { useNavigate } from 'react-router-dom';
import { AuthContext } from 'src/context/AuthContext';
import { useTranslation } from 'react-i18next'; // Import bộ dịch

const Login = () => {
  const { t } = useTranslation(); // Khởi tạo hook dịch
  const { saveUserInfo } = useContext(AuthContext);
  const { login, loading } = useLogin();
  const navigate = useNavigate();
  const [errors, setErrors] = useState([]);

  const handleLogin = async (account, password) => {
    setErrors([]);
    const res = await login(account, password);

    if (res.success == true) {
      navigate('/');
      saveUserInfo(res.data);
      return;
    }
    setErrors([res?.message || t('Message.LoginFailed') || 'Username or Password is incorrect!']);
  };

  return (
    <PageContainer title={t('Page.LoginTitle') || 'Login'} description={t('Page.LoginDescription') || 'This is Login page'}>
      <Grid container spacing={0} sx={{ overflowX: 'hidden' }}>
        <Grid
          size={{ xs: 12, sm: 12, lg: 7, xl: 8 }}
          sx={{
            position: 'relative',
            '&:before': {
              content: '""',
              background: 'radial-gradient(#d2f1df, #d3d7fa, #bad8f4)',
              backgroundSize: '400% 400%',
              animation: 'gradient 15s ease infinite',
              position: 'absolute',
              height: '100%',
              width: '100%',
              opacity: 0.3,
            },
          }}
        >
          <Box position="relative">
            <Box px={3}>
              <Logo />
            </Box>
            <Box
              alignItems="center"
              justifyContent="center"
              height={'calc(100vh - 75px)'}
              sx={{
                display: {
                  xs: 'none',
                  lg: 'flex',
                },
              }}
            >
              <img
                src={img1}
                alt="bg"
                style={{
                  width: '100%',
                  maxWidth: '500px',
                }}
              />
            </Box>
          </Box>
        </Grid>

        <Grid
          size={{ xs: 12, sm: 12, lg: 5, xl: 4 }}
          display="flex"
          justifyContent="center"
          alignItems="center"
        >
          <Box p={4} width="100%">
            <Stack spacing={1} mb={2}>
              {errors.length > 0 && <Alert severity="error">{errors[0]}</Alert>}
            </Stack>

            <AuthLogin
              onLogin={handleLogin}
              loading={loading}
              title={t('Auth.WelcomeTitle') || 'Welcome to Modernize'}
              subtext={
                <Typography variant="subtitle1" color="textSecondary" mb={1}>
                  {t('Auth.AdminDashboard') || 'Your Admin Dashboard'}
                </Typography>
              }
            />
          </Box>
        </Grid>
      </Grid>
    </PageContainer>
  );
};

export default Login;