import { Controller, useFormContext } from 'react-hook-form';
import Box from '@mui/material/Box';
import { Grid, Typography } from '@mui/material';
import CustomFormLabel from 'src/components/forms/theme-elements/CustomFormLabel';
import CustomTextField from 'src/components/forms/theme-elements/CustomTextField';
import { useTranslation } from 'react-i18next';

const DimensionCard = () => {
  const { t } = useTranslation();
  const { control } = useFormContext();

  return (
    <Box p={3}>
      <Typography variant="h5">{t('Field.Dimensions')}</Typography>

      <Grid container spacing={3} mt={1}>
        <Grid item size={{ xs: 12, sm: 6, lg: 3 }}>
          <CustomFormLabel htmlFor="p_weight" sx={{ mt: 0 }}>
            {t('Field.Weight')} (kg)
          </CustomFormLabel>
          <Controller
            name="weight"
            control={control}
            render={({ field }) => (
              <CustomTextField
                {...field}
                id="p_weight"
                type="number"
                placeholder="0"
                fullWidth
              />
            )}
          />
        </Grid>

        <Grid item size={{ xs: 12, sm: 6, lg: 3 }}>
          <CustomFormLabel htmlFor="p_width" sx={{ mt: 0 }}>
            {t('Field.Width')} (cm)
          </CustomFormLabel>
          <Controller
            name="width"
            control={control}
            render={({ field }) => (
              <CustomTextField
                {...field}
                id="p_width"
                type="number"
                placeholder="0"
                fullWidth
              />
            )}
          />
        </Grid>

        <Grid item size={{ xs: 12, sm: 6, lg: 3 }}>
          <CustomFormLabel htmlFor="p_height" sx={{ mt: 0 }}>
            {t('Field.Height')} (cm)
          </CustomFormLabel>
          <Controller
            name="height"
            control={control}
            render={({ field }) => (
              <CustomTextField
                {...field}
                id="p_height"
                type="number"
                placeholder="0"
                fullWidth
              />
            )}
          />
        </Grid>

        <Grid item size={{ xs: 12, sm: 6, lg: 3 }}>
          <CustomFormLabel htmlFor="p_depth" sx={{ mt: 0 }}>
            {t('Field.Depth')} (cm)
          </CustomFormLabel>
          <Controller
            name="depth"
            control={control}
            render={({ field }) => (
              <CustomTextField
                {...field}
                id="p_depth"
                type="number"
                placeholder="0"
                fullWidth
              />
            )}
          />
        </Grid>
      </Grid>
    </Box>
  );
};

export default DimensionCard;