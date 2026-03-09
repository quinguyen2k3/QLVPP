import React, { Suspense } from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';
import Spinner from './views/spinner/Spinner';
import './utils/i18n';
import { CustomizerContextProvider } from './context/CustomizerContext';

ReactDOM.createRoot(document.getElementById('root')).render(
  <CustomizerContextProvider>
    <Suspense fallback={<Spinner />}>
      <App />
    </Suspense>
  </CustomizerContextProvider>
);