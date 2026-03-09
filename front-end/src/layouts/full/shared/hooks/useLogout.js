import { useNavigate } from 'react-router-dom';
import { removeToken } from 'src/services/tokenService';
import { logoutApi } from 'src/api/authentication/authApi';

export const useLogout = () => {
  const navigate = useNavigate();

  const logout = async () => {
    try {
      await logoutApi();

      removeToken();
      
      navigate('/auth/login');
    } catch (error) {
      console.error('Logout failed:', error);
      localStorage.clear();
      navigate('/auth/login');
    }
  };

  return { logout };
};