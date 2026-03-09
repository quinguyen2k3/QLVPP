import React, { useContext, useEffect } from 'react';
import { Avatar, IconButton, Menu, MenuItem, Typography, Stack } from '@mui/material';
import { CustomizerContext } from 'src/context/CustomizerContext';
import { useTranslation } from 'react-i18next';
import FlagEn from 'src/assets/images/flag/icon-flag-en.svg';
import FlagVn from 'src/assets/images/flag/icon-flag-vn.svg';

const Languages = [
  {
    flagname: 'English (UK)',
    icon: FlagEn,
    value: 'en',
  },
  {
    flagname: 'Tiếng Việt (Vietnamese)',
    icon: FlagVn,
    value: 'vi',
  },
];

const Language = () => {
  const [anchorEl, setAnchorEl] = React.useState(null);
  const { isLanguage, setIsLanguage } = useContext(CustomizerContext);
  const { i18n } = useTranslation();

  const open = Boolean(anchorEl);

  const currentLang = Languages.find((_lang) => _lang.value === isLanguage) || Languages[0];

  const handleClick = (event) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  useEffect(() => {
    i18n.changeLanguage(isLanguage);
  }, [isLanguage, i18n]);

  const handleLanguageChange = (langValue) => {
    setIsLanguage(langValue);
    handleClose();
  };

  return (
    <>
      <IconButton
        aria-label="select language"
        id="language-button"
        aria-controls={open ? 'language-menu' : undefined}
        aria-expanded={open ? 'true' : undefined}
        aria-haspopup="true"
        onClick={handleClick}
      >
        <Avatar src={currentLang.icon} alt={currentLang.value} sx={{ width: 20, height: 20 }} />
      </IconButton>

      <Menu
        id="language-menu"
        anchorEl={anchorEl}
        open={open}
        onClose={handleClose}
        sx={{
          '& .MuiMenu-paper': {
            width: '240px',
          },
        }}
      >
        {Languages.map((option, index) => (
          <MenuItem
            key={index}
            sx={{ py: 2, px: 3 }}
            onClick={() => handleLanguageChange(option.value)}
            selected={option.value === isLanguage}
          >
            <Stack direction="row" spacing={2} alignItems="center">
              <Avatar src={option.icon} alt={option.flagname} sx={{ width: 20, height: 20 }} />
              <Typography variant="body1">{option.flagname}</Typography>
            </Stack>
          </MenuItem>
        ))}
      </Menu>
    </>
  );
};

export default Language;
