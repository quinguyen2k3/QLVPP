import React, {useContext} from 'react';
import { Box, Avatar, Typography, IconButton, Tooltip, useMediaQuery } from '@mui/material';
import { CustomizerContext } from 'src/context/CustomizerContext';
import img1 from 'src/assets/images/profile/user-1.jpg';
import { IconPower } from '@tabler/icons';
import { Link } from "react-router";
import { useLogout } from '../../../shared/hooks/useLogout';
import {AuthContext} from 'src/context/AuthContext';

export const Profile = () => {
  const {user} = useContext(AuthContext);
  const { isSidebarHover, isCollapse } = useContext(CustomizerContext);
  const { logout } = useLogout(); 

  const lgUp = useMediaQuery((theme) => theme.breakpoints.up('lg'));
  const hideMenu = lgUp ? isCollapse == 'mini-sidebar' && !isSidebarHover : '';

  const handleLogoutClick = async () => {
    await logout();
  };

  return (
    <Box
      display={'flex'}
      alignItems="center"
      gap={2}
      sx={{ m: 3, p: 2, bgcolor: `${'secondary.light'}` }}
    >
      {!hideMenu ? (
        <>
          <Avatar alt="Remy Sharp" src={img1} />

          <Box>
            <Typography variant="h6" color="textPrimary">{user?.name?.trim().split(' ').pop() || 'Khách'}</Typography>
            <Typography variant="caption" color="textSecondary">{user.position}</Typography>
          </Box>
          <Box sx={{ ml: 'auto' }}>
            <Tooltip title="Logout" placement="top">
              <IconButton color="primary" aria-label="logout" size="small" onClick={handleLogoutClick}>
                <IconPower size="20" />
              </IconButton>
            </Tooltip>
          </Box>
        </>
      ) : (
        ''
      )}
    </Box>
  );
};
