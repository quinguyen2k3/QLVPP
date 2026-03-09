import React, { createContext, useState, useEffect } from 'react';

export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [isAuthLoading, setIsAuthLoading] = useState(true);

  useEffect(() => {
    const storedUser = localStorage.getItem('user_info');
    if (storedUser) {
      setUser(JSON.parse(storedUser));
    }
    setIsAuthLoading(false);
  }, []);

  const saveUserInfo = (authData) => {
    localStorage.setItem('user_info', JSON.stringify(authData));
    setUser(authData);
  };

  const logout = () => {
    localStorage.removeItem('user_info');
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, saveUserInfo, logout, isAuthLoading }}>
      {children}
    </AuthContext.Provider>
  );
};