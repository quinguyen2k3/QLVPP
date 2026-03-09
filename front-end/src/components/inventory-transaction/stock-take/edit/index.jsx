import React, { useState, useContext, useEffect } from 'react';
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
  CircularProgress,
} from '@mui/material';
import { useNavigate, useParams } from 'react-router-dom';
import { format, isValid } from 'date-fns';
import { IconPlus, IconTrash, IconSquareRoundedPlus, IconArrowLeft } from '@tabler/icons-react';
import CustomFormLabel from 'src/components/forms/theme-elements/CustomFormLabel';
import CustomTextField from 'src/components/forms/theme-elements/CustomTextField';
import { useTranslation } from 'react-i18next';

import { StockTakeContext } from '../../../../context/StockTakeContext';
import { useMasterData, useProducts } from 'src/hooks/useMasterData';

const EditStockTakeApp = () => {
  const { t } = useTranslation();
  const router = useNavigate();
  const { id } = useParams();

  const { getStockTakeById, updateStockTake } = useContext(StockTakeContext);
  const { warehouses, requesters } = useMasterData();

  const [showAlert, setShowAlert] = useState(false);
  const [loading, setLoading] = useState(false);
  const [fetching, setFetching] = useState(true);
  const [errors, setErrors] = useState({});

  const [formData, setFormData] = useState({
    id: null,
    code: '',
    requester: null,
    warehouse: null,
    purpose: '',
    status: 'Pending',
    items: [],
    date: new Date().toISOString(),
  });

  const { products } = useProducts('local', formData.warehouse?.id);

  useEffect(() => {
    const loadStockTake = async () => {
      if (!id) return;
      setFetching(true);
      const data = await getStockTakeById(Number(id));
      if (data) {
        setFormData({
          id: data.id,
          code: data.code,
          requester: requesters?.find((r) => r.id === data.requesterId) || null,
          warehouse: warehouses?.find((w) => w.id === data.warehouseId) || null,
          purpose: data.purpose || '',
          status: data.status,
          date: data.createdDate || new Date().toISOString(),
          items:
            data.items?.map((d) => ({
              productId: d.productId,
              stock: d.sysQty,
              actualQty: d.actualQty,
            })) || [],
        });
      }
      setFetching(false);
    };

    if (warehouses?.length > 0 && requesters?.length > 0) {
      loadStockTake();
    }
  }, [id, getStockTakeById, warehouses, requesters]);

  const isReadOnly = formData.status?.toLowerCase() !== 'pending';

  const getAvailableProductsForDropdown = (currentIndex) => {
    const currentList = products || [];
    const selectedProductIds = formData.items
      .filter((_, i) => i !== currentIndex)
      .map((item) => item.productId)
      .filter(Boolean);

    return currentList.filter((product) => !selectedProductIds.includes(product.id));
  };

  const handleAutocompleteChange = (name, newValue) => {
    setFormData((prev) => ({ ...prev, [name]: newValue }));
  };

  const handleWarehouseChange = (e, newValue) => {
    setFormData((prev) => ({
      ...prev,
      warehouse: newValue,
      items: [],
    }));
  };

  const handleProductSelect = (index, product) => {
    setFormData((prevData) => {
      const updatedItems = [...prevData];
      if (product) {
        updatedItems.items[index] = {
          ...updatedItems.items[index],
          productId: product.id,
          itemName: product.name,
          stock: product.quantity ?? 0,
          actualQty: product.quantity ?? 0,
        };
      }
      return updatedItems;
    });
  };

  const handleItemChange = (index, field, value) => {
    setFormData((prevData) => {
      const updatedItems = [...prevData.items];
      updatedItems[index] = { ...updatedItems[index], [field]: value };
      return { ...prevData, items: updatedItems };
    });
  };

  const handleAddItem = () => {
    setFormData((prevData) => ({
      ...prevData,
      items: [...prevData.items, { productId: null, itemName: '', stock: 0, actualQty: 0 }],
    }));
  };

  const handleDeleteItem = (index) => {
    setFormData((prevData) => ({
      ...prevData,
      items: prevData.items.filter((_, i) => i !== index),
    }));
  };

  const validateForm = () => {
    const newErrors = {};
    if (!formData.requester) newErrors.requester = t('Validation.Required');
    if (!formData.warehouse) newErrors.warehouse = t('Validation.Required');
    if (!formData.purpose.trim()) newErrors.purpose = t('Validation.Required');
    if (!formData.items.length) newErrors.items = t('Validation.AtLeastOneItem');

    formData.items.forEach((item, index) => {
      if (!item.productId) newErrors[`product_${index}`] = t('Validation.Required');
      if (Number(item.actualQty) < 0)
        newErrors[`actualQty_${index}`] = t('Validation.GreaterThanOrEqualZero');
    });

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validateForm()) return;

    const payload = {
      id: formData.id,
      purpose: formData.purpose,
      requesterId: formData.requester?.id,
      warehouseId: formData.warehouse?.id,
      items: formData.items.map((o) => ({
        productId: o.productId,
        actualQty: Number(o.actualQty),
      })),
    };

    setLoading(true);
    try {
      const success = await updateStockTake(payload);
      if (success) {
        setShowAlert(true);
        setTimeout(() => router('/inventory/stock-take/list'), 800);
      }
    } catch (error) {
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  if (fetching) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="200px">
        <CircularProgress />
      </Box>
    );
  }

  const getStatusLabel = (status) => {
    if (!status) return '';
    return t(`Status.${status.charAt(0).toUpperCase() + status.slice(1).toLowerCase()}`);
  };

  const getStatusColor = (status) => {
    switch (status) {
      case 'PENDING':
        return 'warning';
      case 'APPROVED':
      case 'RETURNED':
      case 'COMPLETED':
        return 'success';
      case 'CANCELLED':
      case 'REJECTED':
        return 'error';
      default:
        return 'default';
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <Box>
        <Stack direction="row" spacing={2} justifyContent="space-between" mb={3}>
          <Stack direction="row" alignItems="center" spacing={1}>
            <IconButton onClick={() => router('/inventory/stock-take/list')}>
              <IconArrowLeft size="20" />
            </IconButton>
            <Typography variant="h5">
              {t('Page.EditStockTake')} {formData.code && `#${formData.code}`}
            </Typography>
          </Stack>
          <Box display="flex" gap={1}>
            <Button
              variant="outlined"
              color="error"
              onClick={() => router('/inventory/stock-take/list')}
            >
              {t('Action.Cancel')}
            </Button>
            {!isReadOnly && (
              <Button type="submit" variant="contained" color="primary" disabled={loading}>
                {loading ? t('Action.Save') : t('Action.Update')}
              </Button>
            )}
          </Box>
        </Stack>
        <Divider />

        <Stack direction="row" justifyContent="space-between" alignItems="center" mb={3} mt={3}>
          <Box mt={1}>
            <Chip
              label={getStatusLabel(formData.status)}
              color={getStatusColor(formData.status)}
              variant="filled"
            />
          </Box>
          <Box textAlign="right">
            <CustomFormLabel>{t('Field.Date')}</CustomFormLabel>
            <Typography variant="body1">
              {isValid(new Date(formData.date))
                ? format(new Date(formData.date), 'dd/MM/yyyy')
                : ''}
            </Typography>
          </Box>
        </Stack>

        <Grid container spacing={3} mb={4}>
          <Grid item size={{ xs: 12, sm: 6 }}>
            <CustomFormLabel>{t('Entity.Requester')}</CustomFormLabel>
            <Autocomplete
              fullWidth
              disabled={isReadOnly}
              options={requesters || []}
              getOptionLabel={(option) => option.name}
              value={formData.requester}
              onChange={(e, v) => handleAutocompleteChange('requester', v)}
              renderInput={(params) => (
                <CustomTextField
                  {...params}
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
              disabled={isReadOnly}
              options={warehouses || []}
              getOptionLabel={(option) => option.name}
              value={formData.warehouse}
              onChange={handleWarehouseChange}
              renderInput={(params) => (
                <CustomTextField
                  {...params}
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
              disabled={isReadOnly}
              value={formData.purpose}
              onChange={(e) => setFormData({ ...formData, purpose: e.target.value })}
              error={!!errors.purpose}
              helperText={errors.purpose}
            />
          </Grid>
        </Grid>

        <Stack direction="row" justifyContent="space-between" mb={2}>
          <Typography variant="h6">{t('Field.Items')}</Typography>
          {!isReadOnly && (
            <Button onClick={handleAddItem} variant="contained" startIcon={<IconPlus width={18} />}>
              {t('Action.Add')}
            </Button>
          )}
        </Stack>

        <Paper variant="outlined">
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell width="35%">{t('Entity.Product')}</TableCell>
                  <TableCell width="15%" align="center">
                    {t('Field.SystemQuantity')}
                  </TableCell>
                  <TableCell width="20%">{t('Field.ActualQuantity')}</TableCell>
                  <TableCell width="15%" align="center">
                    {t('Field.Discrepancy')}
                  </TableCell>
                  <TableCell width="15%" align="center">
                    {t('Field.Action')}
                  </TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {formData.items.map((item, index) => {
                  const diff = Number(item.actualQty) - Number(item.stock);
                  return (
                    <TableRow key={index}>
                      <TableCell>
                        <Autocomplete
                          disabled={isReadOnly}
                          options={getAvailableProductsForDropdown(index)}
                          getOptionLabel={(o) => o.name}
                          value={
                            (products || []).find((p) => p.id === item.productId) || {
                              name: item.itemName,
                              id: item.productId,
                            }
                          }
                          onChange={(e, value) => handleProductSelect(index, value)}
                          renderInput={(params) => (
                            <CustomTextField {...params} error={!!errors[`product_${index}`]} />
                          )}
                        />
                      </TableCell>
                      <TableCell align="center">
                        <Chip label={item.stock} size="small" sx={{ fontWeight: 'bold' }} />
                      </TableCell>
                      <TableCell>
                        <CustomTextField
                          type="number"
                          disabled={isReadOnly}
                          value={item.actualQty}
                          error={!!errors[`actualQty_${index}`]}
                          onChange={(e) => handleItemChange(index, 'actualQty', e.target.value)}
                          fullWidth
                        />
                      </TableCell>
                      <TableCell align="center">
                        <Typography
                          fontWeight="bold"
                          color={
                            diff > 0 ? 'success.main' : diff < 0 ? 'error.main' : 'text.secondary'
                          }
                        >
                          {diff > 0 ? `+${diff}` : diff}
                        </Typography>
                      </TableCell>
                      <TableCell align="center">
                        {!isReadOnly && (
                          <IconButton onClick={() => handleDeleteItem(index)} color="error">
                            <IconTrash width={22} />
                          </IconButton>
                        )}
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
          autoHideDuration={2000}
          onClose={() => setShowAlert(false)}
          anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
        >
          <Alert severity="success" variant="filled">
            {t('Message.Success')}
          </Alert>
        </Snackbar>
      </Box>
    </form>
  );
};

export default EditStockTakeApp;