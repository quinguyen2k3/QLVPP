import { useFormContext, Controller } from 'react-hook-form';
import Box from '@mui/material/Box';
import { Grid, Typography } from '@mui/material';
import { MenuItem, Avatar } from '@mui/material';
import CustomSelect from 'src/components/forms/theme-elements/CustomSelect';
import { useTranslation } from 'react-i18next';

const AssetCard = () => {
  const { t } = useTranslation();
  const { control, watch } = useFormContext();
  const currentIsAsset = watch('isAsset');

  return (
    <Box p={3}>
      <Box display="flex" alignItems="center" justifyContent="space-between">
        <Typography variant="h5">{t('Field.Asset')}</Typography>

        <Avatar
          sx={{
            backgroundColor: currentIsAsset === true ? 'success.main' : 'error.main',
            '& svg': { display: 'none' },
            width: 15,
            height: 15,
          }}
        />
      </Box>
      <Grid container mt={3}>
        <Grid item size={12}>
          <Controller
            name="isAsset"
            control={control}
            render={({ field }) => (
              <CustomSelect {...field} fullWidth>
                <MenuItem value={true}>{t('Option.Yes')}</MenuItem>
                <MenuItem value={false}>{t('Option.No')}</MenuItem>
              </CustomSelect>
            )}
          />
          <Typography variant="body2">
            {t('Description.AssetField') || 'Set whether the product is an asset.'}
          </Typography>
        </Grid>
      </Grid>
    </Box>
  );
};

export default AssetCard;