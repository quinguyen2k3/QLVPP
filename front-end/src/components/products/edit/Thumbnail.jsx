import React, { useState, useRef, useEffect } from 'react';
import Box from '@mui/material/Box';
import { Typography } from '@mui/material';
import { useFormContext } from 'react-hook-form';
import { useTranslation } from 'react-i18next'; // Import i18n

import noImage from 'src/assets/images/empty/thumbs-noimage.webp';

const getImageUrl = (path) => {
  if (!path) return null;
  if (path.startsWith('data:') || path.startsWith('http')) return path;

  let baseURL = import.meta.env.VITE_API_BASE_URL || '';
  baseURL = baseURL.replace(/\/api\/?$/, '');
  return `${baseURL}${path.startsWith('/') ? '' : '/'}${path}`;
};

const Thumbnail = () => {
  const { t } = useTranslation(); // Khởi tạo hook
  const { setValue, watch } = useFormContext();
  const fileInputRef = useRef(null);

  const imagePath = watch('imagePath');
  const [imageUrl, setImageUrl] = useState(null);

  useEffect(() => {
    setImageUrl(getImageUrl(imagePath));
  }, [imagePath]);

  const handleImageClick = () => {
    if (fileInputRef.current) {
      fileInputRef.current.click();
    }
  };

  const handleFileChange = (event) => {
    const file = event.target.files?.[0];
    if (!file) return;

    if (file.size > 5 * 1024 * 1024) {
      alert(t('Message.FileTooLarge') || 'File size is too large! Max 5MB allowed.');
      return;
    }

    const reader = new FileReader();
    reader.onload = (e) => {
      setValue('imagePath', e.target.result, { shouldValidate: true });
    };
    reader.readAsDataURL(file);

    setValue('image', file, { shouldValidate: true });
  };

  return (
    <Box p={3}>
      <Typography variant="h5">{t('Field.Thumbnail')}</Typography>
      <Box mt={3} mb={2} textAlign="center">
        <input
          type="file"
          accept="image/*"
          ref={fileInputRef}
          onChange={handleFileChange}
          style={{ display: 'none' }}
        />

        {imageUrl && (
          <Box onClick={handleImageClick} sx={{ display: 'inline-block', cursor: 'pointer' }}>
            <img
              src={imageUrl}
              alt="Preview"
              style={{
                maxWidth: '300px',
                maxHeight: '300px',
                borderRadius: '7px',
                display: 'block',
                objectFit: 'cover',
              }}
              onError={(e) => {
                e.currentTarget.onerror = null;
                e.currentTarget.src = noImage;
              }}
            />
          </Box>
        )}

        <Typography variant="body2" textAlign="center" sx={{ mt: 1 }}>
          {t('Description.ClickToChangeImage') || 'Click on image to change'}
        </Typography>
      </Box>
    </Box>
  );
};

export default Thumbnail;