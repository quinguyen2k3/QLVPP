// src/services/tokenService.js

const TOKEN_KEY = "AUTH_TOKEN";

/**
 * Lưu token vào localStorage
 * @param {string} token
 */
export const setToken = (token) => {
  localStorage.setItem(TOKEN_KEY, token);
};

/**
 * Lấy token từ localStorage
 * @returns {string|null}
 */
export const getToken = () => {
  return localStorage.getItem(TOKEN_KEY);
};

/**
 * Xóa token khỏi localStorage
 */
export const removeToken = () => {
  localStorage.removeItem(TOKEN_KEY);
};

/**
 * Kiểm tra token có tồn tại hay không
 * @returns {boolean}
 */
export const hasToken = () => {
  return !!getToken();
};
