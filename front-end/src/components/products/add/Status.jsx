import { useFormContext, Controller } from 'react-hook-form';
import Box from '@mui/material/Box';
import { Grid, Typography } from '@mui/material';
import { MenuItem, Avatar } from '@mui/material';
import CustomSelect from 'src/components/forms/theme-elements/CustomSelect';
import { useTranslation } from 'react-i18next';

const StatusCard = () => {
  const { t } = useTranslation();
  const { control, watch } = useFormContext();

  const currentIsActivated = watch('isActivated');
  return (
    <Box p={3}>
      <Box display="flex" alignItems="center" justifyContent="space-between">
        <Typography variant="h5">{t('Field.Status')}</Typography>

        <Avatar
          sx={{
            backgroundColor: currentIsActivated === true ? 'success.main' : 'error.main',
            '& svg': { display: 'none' },
            width: 15,
            height: 15,
          }}
        />
      </Box>
      <Grid container mt={3}>
        <Grid item size={12}>
          <Controller
            name="isActivated"
            control={control}
            render={({ field }) => (
              <CustomSelect {...field} fullWidth>
                <MenuItem value={true}>{t('Status.Active')}</MenuItem>
                <MenuItem value={false}>{t('Status.Inactive')}</MenuItem>
              </CustomSelect>
            )}
          />
          <Typography variant="body2">
            {t('Description.StatusField') || 'Set the product status.'}
          </Typography>
        </Grid>
      </Grid>
    </Box>
  );
};

export default StatusCard;