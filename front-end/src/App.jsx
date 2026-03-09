import { ThemeSettings } from './theme/Theme';
import RTL from './layouts/full/shared/customizer/RTL';
import { CssBaseline, ThemeProvider } from '@mui/material';
import { RouterProvider } from 'react-router';
import router from './routes/Router';
import { CustomizerContext } from 'src/context/CustomizerContext';
import { useContext } from 'react';
import { AuthProvider } from 'src/context/AuthContext';

function App() {
  const theme = ThemeSettings();
  const { activeDir } = useContext(CustomizerContext);

  return (
    <AuthProvider>
      <ThemeProvider theme={theme}>
        <RTL direction={activeDir}>
          <CssBaseline />
          <RouterProvider router={router} />
        </RTL>
      </ThemeProvider>
    </AuthProvider>
  );
}

export default App;
