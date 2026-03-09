import { useCallback, useEffect } from 'react';
import { useFormContext } from 'react-hook-form';
import Box from '@mui/material/Box';
import { Typography, useTheme, Avatar, Button, FormHelperText } from '@mui/material';
import { useDropzone } from 'react-dropzone';
import { IconTrash, IconUpload } from '@tabler/icons-react';
import { useTranslation } from 'react-i18next';

const MAX_FILE_SIZE = 5 * 1024 * 1024;

const Thumbnail = () => {
  const { t } = useTranslation();
  const theme = useTheme();

  const { control, watch, setValue, setError, clearErrors, formState: { errors } } = useFormContext();

  const imageFile = watch('image');

  const onDrop = useCallback(
    (acceptedFiles, fileRejections) => {
      if (acceptedFiles.length > 0) {
        const file = acceptedFiles[0];
        const fileWithPreview = Object.assign(file, {
          preview: URL.createObjectURL(file),
        });
        setValue('image', fileWithPreview, { shouldValidate: true });
        clearErrors('image');
      }

      if (fileRejections.length > 0) {
        const rejection = fileRejections[0];
        if (rejection.errors[0].code === 'file-too-large') {
          setError('image', {
            type: 'manual',
            message: t('Message.FileTooLarge') || 'File quá lớn! Kích thước tối đa là 5MB.',
          });
        } else {
          setError('image', {
            type: 'manual',
            message: t('Message.InvalidFile') || 'File không hợp lệ.',
          });
        }
      }
    },
    [setValue, setError, clearErrors, t],
  );

  const { getRootProps, getInputProps, isDragActive } = useDropzone({
    onDrop,
    accept: {
      'image/png': [],
      'image/jpeg': [],
      'image/jpg': [],
    },
    maxFiles: 1,
    maxSize: MAX_FILE_SIZE,
  });

  const handleRemove = () => {
    setValue('image', null);
  };

  useEffect(() => {
    return () => {
      if (imageFile && imageFile.preview) {
        URL.revokeObjectURL(imageFile.preview);
      }
    };
  }, [imageFile]);

  return (
    <Box p={3}>
      <Typography variant="h5">{t('Field.Thumbnail')}</Typography>

      <Box
        mt={3}
        fontSize="12px"
        sx={{
          backgroundColor: isDragActive ? 'action.hover' : 'primary.light',
          color: 'primary.main',
          padding: '30px',
          textAlign: 'center',
          border: `1px dashed`,
          borderColor: errors.image ? 'error.main' : 'primary.main',
          cursor: 'pointer',
        }}
        {...getRootProps({ className: 'dropzone' })}
      >
        <input {...getInputProps()} />
        <Box display="flex" flexDirection="column" alignItems="center">
          <IconUpload size={40} stroke={1.5} />
          <p>
            {t('Placeholder.Dropzone') || "Drag 'n' drop thumbnail here, or click to select"}
          </p>
        </Box>
      </Box>

      {errors.image && (
        <FormHelperText error sx={{ textAlign: 'center', mt: 1 }}>
          {errors.image.message}
        </FormHelperText>
      )}

      <Typography variant="body2" textAlign="center" mt={1}>
        {t('Description.ThumbnailField') || 'Set the product thumbnail image. Only *.png, *.jpg and *.jpeg image files are accepted. Max 5MB.'}
      </Typography>

      {imageFile && !errors.image && (
        <Box
          mt={2}
          display="flex"
          alignItems="center"
          justifyContent="space-between"
          sx={{ border: `1px solid ${theme.palette.divider}`, p: 2, borderRadius: 1 }}
        >
          <Box display="flex" alignItems="center">
            <Avatar
              src={imageFile.preview}
              alt="preview"
              variant="rounded"
              sx={{ width: 56, height: 56, mr: 2 }}
            />
            <Box>
              <Typography variant="body1" fontWeight="500">
                {imageFile.name}
              </Typography>
              <Typography variant="caption" color="textSecondary">
                {Math.round(imageFile.size / 1024)} KB
              </Typography>
            </Box>
          </Box>
          <Button color="error" onClick={handleRemove}>
            <IconTrash size={18} />
          </Button>
        </Box>
      )}
    </Box>
  );
};

export default Thumbnail;