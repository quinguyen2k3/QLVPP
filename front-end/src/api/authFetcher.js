import { getToken, setToken, removeToken } from 'src/services/tokenService';
import { refreshTokenApi } from 'src/api/authentication/authApi';

let isRefreshing = false;
let subscribers = [];

const subscribe = (cb) => subscribers.push(cb);
const notifySubscribers = (token) => {
  subscribers.forEach((cb) => cb(token));
  subscribers = [];
};

export const authFetcher = async (url, options = {}) => {
  const token = getToken();

  const isPublicEndpoint = url.includes('/login') || url.includes('/refresh');

  if (!token && !isPublicEndpoint) {
    removeToken();
    window.location.href = '/auth/login';
    return Promise.reject(new Error('Unauthorized'));
  }

  const headers = {
    ...options.headers,
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
  };

  let response;
  try {
    response = await fetch(url, {
      ...options,
      headers,
      credentials: 'include',
    });
  } catch (err) {
    throw err;
  }

  if (response.status !== 401) {
    return response;
  }

  if (isRefreshing) {
    return new Promise((resolve, reject) => {
      subscribe((newToken) => {
        fetch(url, {
          ...options,
          headers: {
            ...headers,
            Authorization: `Bearer ${newToken}`,
          },
          credentials: 'include',
        })
          .then((res) => {
            if (res.status === 401) {
              removeToken();
              window.location.href = '/auth/login';
              reject(new Error('Unauthorized'));
            } else {
              resolve(res);
            }
          })
          .catch(reject);
      });
    });
  }

  isRefreshing = true;

  try {
    const data = await refreshTokenApi();
    const newToken = data.data;

    if (!newToken) {
      throw new Error('Invalid refresh token');
    }

    setToken(newToken);
    isRefreshing = false;
    notifySubscribers(newToken);

    const retryRes = await fetch(url, {
      ...options,
      headers: {
        ...headers,
        Authorization: `Bearer ${newToken}`,
      },
      credentials: 'include',
    });

    if (retryRes.status === 401) {
      throw new Error('Retry unauthorized');
    }

    return retryRes;
  } catch (error) {
    isRefreshing = false;
    subscribers = [];
    removeToken();
    window.location.href = '/auth/login';
    throw error;
  }
};