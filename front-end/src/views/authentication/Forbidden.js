import { Box, Container, Typography, Button } from '@mui/material';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import ErrorImg from 'src/assets/images/backgrounds/forbiddenimg.svg';

const Forbidden = () => {
  const { t } = useTranslation();

  return (
    <Box
      display="flex"
      flexDirection="column"
      height="100vh"
      textAlign="center"
      justifyContent="center"
    >
      <Container maxWidth="md">
        <img src={ErrorImg} alt="403" />
        <Typography align="center" variant="h1" mb={4}>
          {t('Error.403Title')}
        </Typography>
        <Typography align="center" variant="h4" mb={4}>
          {t('Error.403Description')}
        </Typography>
        <Button
          color="primary"
          variant="contained"
          component={Link}
          to="/dashboards/modern"
          disableElevation
        >
          {t('Error.BackToHome')}
        </Button>
      </Container>
    </Box>
  );
};

export default Forbidden;