import { postFetcher } from 'src/api/globalFetcher';
import { getToken } from 'src/services/tokenService';

const api = import.meta.env.VITE_API_BASE_URL;

export const loginApi = (payload) => {
  return postFetcher(`${api}/auth/login`, payload);
};

export const registerApi = (payload) => {
  return postFetcher(`${api}/auth/register`, payload);
};

export const logoutApi = async () => {
  return postFetcher(`${api}/auth/logout`);
};

export const changePasswordApi = (payload) => {
  return postFetcher(`${api}/auth/change-password`, payload);
}

export const refreshTokenApi = async () => {
  const res = await fetch(`${api}/auth/refresh-token`, {
    method: 'POST',
    credentials: 'include',
    headers: {
      'X-Expired-Access-Token': getToken(),
    },
  });

  if (!res.ok) {
    throw new Error('Refresh token failed');
  }

  return res.json();
};
