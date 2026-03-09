import { Controller, useFormContext } from 'react-hook-form';
import Box from '@mui/material/Box';
import { Autocomplete, Grid, Typography } from '@mui/material';
import CustomFormLabel from 'src/components/forms/theme-elements/CustomFormLabel';
import CustomTextField from 'src/components/forms/theme-elements/CustomTextField';
import { useTranslation } from 'react-i18next';

import { useMasterData } from 'src/hooks/useMasterData';

const ProductDetails = () => {
  const { t } = useTranslation();
  const {
    control,
    formState: { errors },
  } = useFormContext();

  const { categories = [], units = [], loading } = useMasterData();

  if (loading) return <div>{t('Message.Loading')}...</div>;

  return (
    <Box p={3}>
      <Typography variant="h5">{t('Field.ProductDetails')}</Typography>

      <Grid container mt={3}>
        <Grid item size={12}>
          <CustomFormLabel>{t('Menu.Category')}</CustomFormLabel>
          <Controller
            name="categoryId"
            control={control}
            render={({ field: { onChange, value } }) => (
              <Autocomplete
                fullWidth
                options={categories}
                getOptionLabel={(option) => option.name || ''}
                value={categories.find((c) => c.id === value) || null}
                onChange={(e, newValue) => onChange(newValue ? newValue.id : null)}
                renderInput={(params) => (
                  <CustomTextField
                    {...params}
                    placeholder={`${t('Placeholder.Select')} ${t('Menu.Category')}`}
                    error={!!errors.categoryId}
                    helperText={errors.categoryId?.message}
                  />
                )}
              />
            )}
          />
        </Grid>

        <Grid item size={12}>
          <CustomFormLabel sx={{ mt: 2 }}>{t('Menu.Unit')}</CustomFormLabel>
          <Controller
            name="unitId"
            control={control}
            render={({ field: { onChange, value } }) => (
              <Autocomplete
                fullWidth
                options={units}
                getOptionLabel={(option) => option.name || ''}
                value={units.find((u) => u.id === value) || null}
                onChange={(e, newValue) => onChange(newValue ? newValue.id : null)}
                renderInput={(params) => (
                  <CustomTextField
                    {...params}
                    placeholder={`${t('Placeholder.Select')} ${t('Menu.Unit')}`}
                    error={!!errors.unitId}
                    helperText={errors.unitId?.message}
                  />
                )}
              />
            )}
          />
        </Grid>
      </Grid>
    </Box>
  );
};

export default ProductDetails;
