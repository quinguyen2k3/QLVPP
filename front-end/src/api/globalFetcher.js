import { authFetcher } from './authFetcher';

const handleResponse = async (res) => {
  if (!res.ok) {
    const errData = await res.json().catch(() => ({}));
    const error = new Error(errData.message || 'Request failed');
    error.status = res.status;
    throw error;
  }
  return res.json();
};

// ================= GET =================
const getFetcher = (url) =>
  authFetcher(url, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
    },
  }).then(handleResponse);

// ================= POST =================
const postFetcher = (url, body) =>
  authFetcher(url, {
    method: 'POST',
    headers: body instanceof FormData ? {} : { 'Content-Type': 'application/json' },
    body: body instanceof FormData ? body : JSON.stringify(body),
  }).then(handleResponse);

// ================= PUT =================
const putFetcher = (url, body) =>
  authFetcher(url, {
    method: 'PUT',
    headers: body instanceof FormData ? {} : { 'Content-Type': 'application/json' },
    body: body instanceof FormData ? body : JSON.stringify(body),
  }).then(handleResponse);

// ================= PATCH =================
const patchFetcher = (url, body) =>
  authFetcher(url, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  }).then(handleResponse);

// ================= DELETE =================
const deleteFetcher = (url, body) =>
  authFetcher(url, {
    method: 'DELETE',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  }).then(handleResponse);

export { getFetcher, postFetcher, putFetcher, patchFetcher, deleteFetcher };
