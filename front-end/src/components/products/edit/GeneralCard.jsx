import Box from '@mui/material/Box';
import { Typography, Grid } from '@mui/material';
import { Controller, useFormContext } from 'react-hook-form';
import CustomFormLabel from 'src/components/forms/theme-elements/CustomFormLabel';
import CustomTextField from 'src/components/forms/theme-elements/CustomTextField';
import TiptapEdit from 'src/views/forms/from-tiptap/TiptapEdit';
import { useTranslation } from 'react-i18next';

const GeneralCard = () => {
  const { control } = useFormContext();
  const { t } = useTranslation();

  return (
    <Box p={3}>
      <Typography variant="h5">{t('Menu.General')}</Typography>

      <Grid container mt={3}>
        <Grid item size={12}>
          <CustomFormLabel>
            {t('Field.Name')}{' '}
            <Typography color="error.main" component="span">
              *
            </Typography>
          </CustomFormLabel>

          <Controller
            name="name"
            control={control}
            rules={{ required: `${t('Field.Name')} ${t('Message.Required')}` }}
            render={({ field, fieldState }) => (
              <CustomTextField
                {...field}
                fullWidth
                placeholder={t('Field.Name')}
                error={!!fieldState.error}
                helperText={fieldState.error?.message}
              />
            )}
          />

          <Typography variant="body2">
            {t('Description.UniqueName') ||
              'A product name is required and recommended to be unique.'}
          </Typography>
        </Grid>

        <Grid item size={12} mt={3}>
          <CustomFormLabel>
            {t('Field.Code')}{' '}
            <Typography color="error.main" component="span">
              *
            </Typography>
          </CustomFormLabel>

          <Controller
            name="prodCode"
            control={control}
            rules={{ required: `${t('Field.Code')} ${t('Message.Required')}` }}
            render={({ field, fieldState }) => (
              <CustomTextField
                {...field}
                fullWidth
                placeholder={t('Field.Code')}
                error={!!fieldState.error}
                helperText={fieldState.error?.message}
              />
            )}
          />

          <Typography variant="body2">
            {t('Description.UniqueCode') ||
              'A product code is required and recommended to be unique.'}
          </Typography>
        </Grid>

        {/* DESCRIPTION */}
        <Grid item size={12} mt={3}>
          <CustomFormLabel>{t('Field.Description')}</CustomFormLabel>

          <Controller
            name="description"
            control={control}
            render={({ field }) => <TiptapEdit value={field.value} onChange={field.onChange} />}
          />

          <Typography variant="body2">
            {t('Description.DescriptionField') ||
              'Set a description to the product for better visibility.'}
          </Typography>
        </Grid>
      </Grid>
    </Box>
  );
};

export default GeneralCard;
