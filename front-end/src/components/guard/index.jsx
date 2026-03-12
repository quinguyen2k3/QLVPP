import { useContext } from 'react';
import { AuthContext } from 'src/context/AuthContext';

const RequireRole = ({ allowedRoles, children }) => {
  const { user } = useContext(AuthContext);

  if (!user || !user.authenticated) {
    return null;
  }

  const hasRole = 
    allowedRoles.includes(user.role) || 
    allowedRoles.some(r => user.position?.toLowerCase().includes(r.toLowerCase()));

  if (!hasRole) {
    return null;
  }

  return children;
};

export default RequireRole;