import React, { useState } from 'react';
import { useContext } from 'react';
import { Link } from 'react-router';
import { Box, Menu, Avatar, Typography, Divider, Button, IconButton } from '@mui/material';
import * as dropdownData from './data';

import { IconMail } from '@tabler/icons';
import { Stack } from '@mui/system';

import ProfileImg from 'src/assets/images/profile/user-1.jpg';
// import unlimitedImg from 'src/assets/images/backgrounds/unlimited-bg.png';
import Scrollbar from 'src/components/custom-scroll/Scrollbar';
import { useLogout } from '../../shared/hooks/useLogout';
import { useTranslation } from 'react-i18next';
import { AuthContext } from 'src/context/AuthContext';

const Profile = () => {
  const { user } = useContext(AuthContext);
  const { t } = useTranslation();
  const [anchorEl2, setAnchorEl2] = useState(null);
  const { logout } = useLogout();

  const handleClick2 = (event) => {
    setAnchorEl2(event.currentTarget);
  };

  const handleClose2 = () => {
    setAnchorEl2(null);
  };

  const handleLogoutClick = async () => {
    handleClose2();
    await logout();
  };

  return (
    <Box>
      <IconButton
        size="large"
        aria-label="show 11 new notifications"
        color="inherit"
        aria-controls="msgs-menu"
        aria-haspopup="true"
        sx={{
          ...(typeof anchorEl2 === 'object' && {
            color: 'primary.main',
          }),
        }}
        onClick={handleClick2}
      >
        <Avatar
          src={ProfileImg}
          alt={ProfileImg}
          sx={{
            width: 35,
            height: 35,
          }}
        />
      </IconButton>
      {/* ------------------------------------------- */}
      {/* Message Dropdown */}
      {/* ------------------------------------------- */}
      <Menu
        id="msgs-menu"
        anchorEl={anchorEl2}
        keepMounted
        open={Boolean(anchorEl2)}
        onClose={handleClose2}
        anchorOrigin={{ horizontal: 'right', vertical: 'bottom' }}
        transformOrigin={{ horizontal: 'right', vertical: 'top' }}
        sx={{
          '& .MuiMenu-paper': {
            width: '360px',
          },
        }}
      >
        <Scrollbar sx={{ height: '100%', maxHeight: '85vh' }}>
          <Box p={3}>
            <Typography variant="h5">{t('Field.UserProfile')}</Typography>
            <Stack direction="row" py={3} spacing={2} alignItems="center">
              <Avatar src={ProfileImg} alt={ProfileImg} sx={{ width: 95, height: 95 }} />
              <Box>
                <Typography variant="subtitle2" color="textPrimary" fontWeight={600}>
                  {user.name}
                </Typography>
                <Typography variant="subtitle2" color="textSecondary">
                  {user.position}
                </Typography>
                <Typography
                  variant="subtitle2"
                  color="textSecondary"
                  display="flex"
                  alignItems="center"
                  gap={1}
                >
                  <IconMail width={15} height={15} />
                  {user.email}
                </Typography>
              </Box>
            </Stack>
            <Divider />
            {dropdownData.profile.map((profile) => (
              <Box key={profile.title}>
                <Box sx={{ py: 2, px: 0 }} className="hover-text-primary">
                  <Link to={profile.href}>
                    <Stack direction="row" spacing={2}>
                      <Box
                        width="45px"
                        height="45px"
                        bgcolor="primary.light"
                        display="flex"
                        alignItems="center"
                        justifyContent="center"
                      >
                        <Avatar
                          src={profile.icon}
                          alt={profile.icon}
                          sx={{
                            width: 24,
                            height: 24,
                            borderRadius: 0,
                          }}
                        />
                      </Box>
                      <Box>
                        <Typography
                          variant="subtitle2"
                          fontWeight={600}
                          color="textPrimary"
                          className="text-hover"
                          noWrap
                          sx={{
                            width: '240px',
                          }}
                        >
                          {t(profile.title)}
                        </Typography>
                        <Typography
                          color="textSecondary"
                          variant="subtitle2"
                          sx={{
                            width: '240px',
                          }}
                          noWrap
                        >
                          {t(profile.subtitle)}
                        </Typography>
                      </Box>
                    </Stack>
                  </Link>
                </Box>
              </Box>
            ))}
            <Box mt={2}>
              <Button variant="outlined" color="primary" onClick={handleLogoutClick} fullWidth>
                {t('Action.Logout')}
              </Button>
            </Box>
          </Box>
        </Scrollbar>
      </Menu>
    </Box>
  );
};

export default Profile;
