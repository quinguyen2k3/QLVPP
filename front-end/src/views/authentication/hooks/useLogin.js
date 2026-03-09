import { useState } from 'react';
import { setToken } from 'src/services/tokenService';
import { loginApi } from 'src/api/authentication/authApi';

export const useLogin = () => {
  const [loading, setLoading] = useState(false);

  const login = async (account, password) => {
    setLoading(true);
    try {
      const res = await loginApi({ account, password });

      const token = res?.accessToken || res?.data?.accessToken;

      if (token) {
        setToken(token);
        return { success: true, message: 'Login successful', data: res?.data };
      }

      return { success: false, message: res?.message || 'Login failed' };
    } catch (err) {
      console.error(err);
      if (err.status === 401) {
        return { success: false, message: 'Wrong username or password!' };
      }
      return { success: false, message: err.message || 'Login failed!' };
    } finally {
      setLoading(false);
    }
  };

  return { login, loading };
};
