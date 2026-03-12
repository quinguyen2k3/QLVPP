import React, { useContext } from 'react';
import { Navigate } from 'react-router';
import { AuthContext } from 'src/context/AuthContext';

const ProtectedRoute = ({ children, allowedRoles }) => {
  const { user, isAuthLoading } = useContext(AuthContext);

  if (isAuthLoading) {
    return null;
  }

  if (!user || !user.authenticated) {
    return <Navigate to="/auth/login" replace />;
  }

  if (allowedRoles && allowedRoles.length > 0) {
    const hasRole =
      allowedRoles.includes(user.role) ||
      allowedRoles.some((r) => user.position?.toLowerCase().includes(r.toLowerCase()));

    if (!hasRole) {
      return <Navigate to="/auth/403" replace />;
    }
  }

  return children;
};

export default ProtectedRoute;
