import React, { useState, useContext } from 'react';
import {
  Alert,
  Button,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Typography,
  IconButton,
  Tooltip,
  Box,
  Stack,
  Divider,
  Grid,
  Autocomplete,
  Chip,
  Snackbar,
} from '@mui/material';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';

import { useNavigate } from 'react-router-dom';
import { format, isValid } from 'date-fns';
import { IconPlus, IconTrash, IconSquareRoundedPlus } from '@tabler/icons-react';
import CustomFormLabel from 'src/components/forms/theme-elements/CustomFormLabel';
import CustomTextField from 'src/components/forms/theme-elements/CustomTextField';
import { useTranslation } from 'react-i18next';

import { StockTakeContext } from '../../../../context/StockTakeContext';
import { useMasterData, useProducts } from 'src/hooks/useMasterData';

const CreateStockTakeApp = () => {
  const { t } = useTranslation();
  const router = useNavigate();
  const { addStockTake } = useContext(StockTakeContext);

  const { warehouses, requesters } = useMasterData();

  const [showAlert, setShowAlert] = useState(false);
  const [loading, setLoading] = useState(false);
  const [errors, setErrors] = useState({});

  const [formData, setFormData] = useState({
    requester: null,
    warehouse: null,
    purpose: '',
    status: 'Pending',
    items: [{ productId: null, itemName: '', stock: 0, actualQty: 0 }],
    date: new Date().toISOString().split('T')[0],
  });

  const { products } = useProducts('local', formData.warehouse?.id);

  const getAvailableProductsForDropdown = (currentIndex) => {
    const currentList = products || [];
    const selectedProductIds = formData.items
      .filter((_, i) => i !== currentIndex)
      .map((item) => item.productId)
      .filter(Boolean);

    return currentList.filter((product) => !selectedProductIds.includes(product.id));
  };

  const handleAutocompleteChange = (name, newValue) => {
    if (name === 'warehouse') {
      setFormData((prev) => ({
        ...prev,
        warehouse: newValue,
        items: [{ productId: null, itemName: '', stock: 0, actualQty: 0 }],
      }));
    } else {
      setFormData((prevData) => ({
        ...prevData,
        [name]: newValue,
      }));
    }
  };

  const handleProductSelect = (index, product) => {
    setFormData((prevData) => {
      const updatedItems = [...prevData.items];

      if (product) {
        updatedItems[index] = {
          ...updatedItems[index],
          productId: product.id,
          itemName: product.name,
          stock: product.quantity ?? 0,
          actualQty: product.quantity ?? 0,
        };
      } else {
        updatedItems[index] = {
          ...updatedItems[index],
          productId: null,
          itemName: '',
          stock: 0,
          actualQty: 0,
        };
      }

      return { ...prevData, items: updatedItems };
    });
  };

  const handleItemChange = (index, field, value) => {
    setFormData((prevData) => {
      const updatedItems = [...prevData.items];
      updatedItems[index] = {
        ...updatedItems[index],
        [field]: value,
      };
      return { ...prevData, items: updatedItems };
    });
  };

  const handleAddItem = () => {
    setFormData((prevData) => {
      const updatedItems = [
        ...prevData.items,
        { productId: null, itemName: '', stock: 0, actualQty: 0 },
      ];
      return { ...prevData, items: updatedItems };
    });
  };

  const handleDeleteItem = (index) => {
    setFormData((prevData) => {
      const updatedItems = prevData.items.filter((_, i) => i !== index);
      return { ...prevData, items: updatedItems };
    });
  };

  const validateForm = () => {
    const newErrors = {};

    if (!formData.requester) newErrors.requester = t('Validation.Required');
    if (!formData.warehouse) newErrors.warehouse = t('Validation.Required');
    if (!formData.purpose.trim()) newErrors.purpose = t('Validation.Required');

    if (!formData.items.length) {
      newErrors.items = t('Validation.AtLeastOneItem');
    } else {
      formData.items.forEach((item, index) => {
        if (!item.productId) newErrors[`product_${index}`] = t('Validation.Required');
        const qty = Number(item.actualQty);
        if (qty < 0 || isNaN(qty)) {
          newErrors[`actualQty_${index}`] = t('Validation.GreaterThanOrEqualZero');
        }
      });
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validateForm()) return;

    const payload = {
      purpose: formData.purpose,
      requesterId: formData.requester?.id,
      warehouseId: formData.warehouse?.id,
      items: formData.items
        .filter((o) => o.productId)
        .map((o) => ({
          productId: o.productId,
          actualQty: Number(o.actualQty),
        })),
    };

    setLoading(true);
    try {
      const success = await addStockTake(payload);
      if (success) {
        setShowAlert(true);
        setTimeout(() => {
          router('/inventory/stock-take/list');
        }, 800);
      }
    } catch (error) {
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  const parsedDate = isValid(new Date(formData.date)) ? new Date(formData.date) : new Date();
  const formattedDate = format(parsedDate, 'dd/MM/yyyy');

  return (
    <>
      <form onSubmit={handleSubmit}>
        <Box>
          <Stack direction="row" spacing={2} justifyContent="space-between" mb={3}>
            <Typography variant="h5">{t('Menu.StockTake')}</Typography>
            <Box display="flex" gap={1}>
              <Button
                variant="outlined"
                color="error"
                onClick={() => router('/inventory/stock-take/list')}
              >
                {t('Action.Cancel')}
              </Button>
              <Button type="submit" variant="contained" color="primary" disabled={loading}>
                {loading ? t('Action.Save') : t('Action.Add')}
              </Button>
            </Box>
          </Stack>
          <Divider />

          <Stack direction="row" justifyContent="space-between" alignItems="center" mb={3} mt={3}>
            <Box>
              <Box mt={1}>
                <Chip label={t('Status.Pending')} color="warning" variant="filled" />
              </Box>
            </Box>
            <Box textAlign="right">
              <CustomFormLabel>{t('Field.Date')}</CustomFormLabel>
              <Typography variant="body1">{formattedDate}</Typography>
            </Box>
          </Stack>

          <Grid container spacing={3} mb={4}>
            <Grid item size={{ xs: 12, sm: 6 }}>
              <CustomFormLabel>{t('Entity.Requester')}</CustomFormLabel>
              <Autocomplete
                fullWidth
                options={requesters || []}
                getOptionLabel={(option) => option.name}
                value={formData.requester}
                onChange={(e, v) => handleAutocompleteChange('requester', v)}
                renderInput={(params) => (
                  <CustomTextField
                    {...params}
                    placeholder={t('Placeholder.Select')}
                    error={!!errors.requester}
                    helperText={errors.requester}
                  />
                )}
              />
            </Grid>

            <Grid item size={{ xs: 12, sm: 6 }}>
              <CustomFormLabel>{t('Field.Warehouse')}</CustomFormLabel>
              <Autocomplete
                fullWidth
                options={warehouses || []}
                getOptionLabel={(option) => option.name}
                value={formData.warehouse}
                onChange={(e, v) => handleAutocompleteChange('warehouse', v)}
                renderInput={(params) => (
                  <CustomTextField
                    {...params}
                    placeholder={t('Placeholder.Select')}
                    error={!!errors.warehouse}
                    helperText={errors.warehouse}
                  />
                )}
              />
            </Grid>

            <Grid item size={{ xs: 12 }}>
              <CustomFormLabel>{t('Field.Purpose')}</CustomFormLabel>
              <CustomTextField
                fullWidth
                value={formData.purpose}
                onChange={(e) => setFormData({ ...formData, purpose: e.target.value })}
                placeholder={t('Placeholder.Enter') + ' ' + t('Field.Purpose')}
                error={!!errors.purpose}
                helperText={errors.purpose}
              />
            </Grid>
          </Grid>

          <Stack direction="row" justifyContent="space-between" mb={2}>
            <Typography variant="h6">{t('Field.Items')}</Typography>
            <Button
              onClick={handleAddItem}
              variant="contained"
              color="primary"
              startIcon={<IconPlus width={18} />}
              disabled={!formData.warehouse}
            >
              {t('Action.Add')}
            </Button>
          </Stack>

          <Paper variant="outlined">
            {errors.items && (
              <Alert severity="error" sx={{ mb: 2 }}>
                {errors.items}
              </Alert>
            )}
            <TableContainer sx={{ whiteSpace: { xs: 'nowrap', md: 'unset' } }}>
              <Table>
                <TableHead>
                  <TableRow>
                    <TableCell width="35%">
                      <Typography variant="h6" fontSize="14px">
                        {t('Entity.Product')}
                      </Typography>
                    </TableCell>

                    <TableCell width="15%" align="center">
                      <Typography variant="h6" fontSize="14px">
                        {t('Field.SystemQuantity')}
                      </Typography>
                    </TableCell>

                    <TableCell width="20%">
                      <Typography variant="h6" fontSize="14px">
                        {t('Field.ActualQuantity')}
                      </Typography>
                    </TableCell>

                    <TableCell width="15%" align="center">
                      <Typography variant="h6" fontSize="14px">
                        {t('Field.Discrepancy')}
                      </Typography>
                    </TableCell>

                    <TableCell width="15%" align="center">
                      <Typography variant="h6" fontSize="14px">
                        {t('Field.Action')}
                      </Typography>
                    </TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {formData.items.map((item, index) => {
                    const diff = Number(item.actualQty) - Number(item.stock);
                    const isPositive = diff > 0;
                    const isNegative = diff < 0;

                    return (
                      <TableRow key={index}>
                        <TableCell sx={{ verticalAlign: 'top' }}>
                          <Autocomplete
                            options={getAvailableProductsForDropdown(index)}
                            getOptionLabel={(option) => option.name}
                            value={(products || []).find((p) => p.id === item.productId) || null}
                            onChange={(e, value) => handleProductSelect(index, value)}
                            renderInput={(params) => (
                              <CustomTextField
                                {...params}
                                placeholder={t('Placeholder.SearchProduct')}
                                error={!!errors[`product_${index}`]}
                                helperText={errors[`product_${index}`]}
                              />
                            )}
                            fullWidth
                            disabled={!formData.warehouse}
                            noOptionsText={
                              !formData.warehouse
                                ? t('Message.SelectWarehouseFirst')
                                : t('Message.NoData')
                            }
                          />
                        </TableCell>

                        <TableCell align="center">
                          <Chip
                            label={item.stock}
                            color="default"
                            size="small"
                            sx={{ mt: 1, fontWeight: 'bold' }}
                          />
                        </TableCell>

                        <TableCell sx={{ verticalAlign: 'top' }}>
                          <CustomTextField
                            type="number"
                            value={item.actualQty}
                            error={!!errors[`actualQty_${index}`]}
                            helperText={errors[`actualQty_${index}`]}
                            onChange={(e) => handleItemChange(index, 'actualQty', e.target.value)}
                            fullWidth
                            inputProps={{ min: 0 }}
                          />
                        </TableCell>

                        <TableCell align="center">
                          <Typography
                            sx={{
                              mt: 1,
                              fontWeight: 'bold',
                              color: isPositive
                                ? 'success.main'
                                : isNegative
                                  ? 'error.main'
                                  : 'text.secondary',
                            }}
                          >
                            {isPositive ? `+${diff}` : diff}
                          </Typography>
                        </TableCell>

                        <TableCell align="center">
                          <Stack direction="row" justifyContent="center">
                            <Tooltip title={t('Action.Add')}>
                              <IconButton
                                onClick={handleAddItem}
                                color="primary"
                                disabled={!formData.warehouse}
                              >
                                <IconSquareRoundedPlus width={22} />
                              </IconButton>
                            </Tooltip>
                            <Tooltip title={t('Action.Delete')}>
                              <IconButton onClick={() => handleDeleteItem(index)} color="error">
                                <IconTrash width={22} />
                              </IconButton>
                            </Tooltip>
                          </Stack>
                        </TableCell>
                      </TableRow>
                    );
                  })}
                </TableBody>
              </Table>
            </TableContainer>
          </Paper>

          <Snackbar
            open={showAlert}
            autoHideDuration={6000}
            onClose={() => setShowAlert(false)}
            anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
          >
            <Alert onClose={() => setShowAlert(false)} severity="success" sx={{ width: '100%' }}>
              {t('Message.Success')}
            </Alert>
          </Snackbar>
        </Box>
      </form>
    </>
  );
};

export default CreateStockTakeApp;
