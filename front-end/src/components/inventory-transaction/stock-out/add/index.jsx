import React, { useState, useContext, useEffect, useMemo } from 'react';
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
  Radio,
  RadioGroup,
  FormControlLabel,
  FormControl,
  Fade,
  CircularProgress,
} from '@mui/material';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';

import { useNavigate, useLocation } from 'react-router-dom';
import { format, isValid } from 'date-fns';
import { IconPlus, IconTrash, IconSquareRoundedPlus } from '@tabler/icons-react';
import CustomFormLabel from 'src/components/forms/theme-elements/CustomFormLabel';
import CustomTextField from 'src/components/forms/theme-elements/CustomTextField';
import { useTranslation } from 'react-i18next';

import { StockOutContext } from '../../../../context/StockOutContext';
import { useMasterData, useProducts } from 'src/hooks/useMasterData';
import { stockTakeApi } from 'src/api/inventory-transaction/stock-take/stockTakeApi';

const STOCK_OUT_TYPES = {
  USAGE: 1,
  TRANSFER: 2,
  ADJUSTMENT: 3,
};

const CreateStockOutApp = ({ type = STOCK_OUT_TYPES.USAGE }) => {
  const { t } = useTranslation();
  const router = useNavigate();
  const location = useLocation();
  const { addStockOut } = useContext(StockOutContext);

  const { warehouses, departments, units, requesters } = useMasterData();

  const [toast, setToast] = useState({ open: false, type: 'success', message: '' });
  const [loading, setLoading] = useState(false);
  const [errors, setErrors] = useState({});
  const [referenceOptions, setReferenceOptions] = useState([]);
  const [isRefLoading, setIsRefLoading] = useState(false);

  const [formData, setFormData] = useState({
    type: type,
    requester: null,
    warehouse: null,
    toWarehouse: null,
    department: null,
    selectedReference: null,
    note: '',
    status: 'Pending',
    orders: [{ productId: null, itemName: '', unit: null, stock: 0, quantity: 0 }],
    date: new Date().toISOString().split('T')[0],
  });

  const { products } = useProducts('local', formData.warehouse?.id);

  const referenceProductIds = useMemo(() => {
    if (!formData.selectedReference) return [];
    const itemsSource =
      formData.selectedReference.stockTakeDetails ||
      formData.selectedReference.items ||
      formData.selectedReference.details ||
      [];

    return itemsSource
      .filter((item) => {
        const diff = item.difference !== undefined ? item.difference : item.actualQty - item.sysQty;
        return diff > 0;
      })
      .map((item) => item.productId);
  }, [formData.selectedReference]);

  useEffect(() => {
    const fetchReferences = async () => {
      if (formData.type !== STOCK_OUT_TYPES.ADJUSTMENT) {
        setReferenceOptions([]);
        return;
      }

      setIsRefLoading(true);
      try {
        const response = await stockTakeApi.getAll();
        const data = response?.data || response || [];
        const options = Array.isArray(data) ? data : [];
        setReferenceOptions(options);

        const queryParams = new URLSearchParams(location.search);
        const refId = queryParams.get('refId');
        if (refId && options.length > 0) {
          const autoSelect = options.find((opt) => opt.id === Number(refId) || opt.code === refId);
          if (autoSelect) {
            handleReferenceSelect(autoSelect);
          }
        }
      } catch (error) {
        setReferenceOptions([]);
      } finally {
        setIsRefLoading(false);
      }
    };

    fetchReferences();
  }, [formData.type, location.search]);

  useEffect(() => {
    if (type !== formData.type) {
      setFormData((prev) => ({
        ...prev,
        type: type,
        toWarehouse: null,
        department: null,
        selectedReference: null,
        orders: [{ productId: null, itemName: '', unit: null, stock: 0, quantity: 0 }],
      }));
      setErrors({});
    }
  }, [type]);

  const getAvailableProductsForDropdown = (currentIndex) => {
    let currentList = products || [];

    if (formData.selectedReference && referenceProductIds.length > 0) {
      currentList = currentList.filter((p) => referenceProductIds.includes(p.id));
    }

    const selectedProductIds = formData.orders
      .filter((_, i) => i !== currentIndex)
      .map((order) => order.productId)
      .filter(Boolean);

    return currentList.filter((product) => !selectedProductIds.includes(product.id));
  };

  const handleTypeChange = (event) => {
    const newType = parseInt(event.target.value);
    setFormData((prev) => ({
      ...prev,
      type: newType,
      toWarehouse: null,
      department: null,
      selectedReference: null,
      orders: [{ productId: null, itemName: '', unit: null, stock: 0, quantity: 0 }],
    }));
    setErrors({});
  };

  const handleReferenceSelect = async (reference) => {
    if (!reference) {
      setFormData((prev) => ({
        ...prev,
        selectedReference: null,
        orders: [{ productId: null, itemName: '', unit: null, stock: 0, quantity: 0 }],
      }));
      return;
    }

    setIsRefLoading(true);
    try {
      const response = await stockTakeApi.getById(reference.id);
      const fullDetail = response?.data || response;

      const itemsSource =
        fullDetail.stockTakeDetails || fullDetail.items || fullDetail.details || [];

      const mappedItems = itemsSource
        .filter((detail) => {
          const diff =
            detail.difference !== undefined ? detail.difference : detail.actualQty - detail.sysQty;
          return diff > 0;
        })
        .map((detail) => {
          const productInfo = (products || []).find((p) => p.id === detail.productId);
          const qty = Math.abs(detail.difference || detail.actualQty - detail.sysQty || 0);

          return {
            productId: detail.productId,
            itemName: productInfo?.name || detail.productName || '',
            unit: units.find((u) => u.id === (productInfo?.unitId || detail.unitId)) || null,
            stock: productInfo?.quantity ?? 0,
            quantity: qty,
          };
        });

      setFormData((prev) => ({
        ...prev,
        selectedReference: fullDetail,
        orders: mappedItems.length > 0 ? mappedItems : prev.orders,
      }));
    } catch (error) {
      console.error(error);
    } finally {
      setIsRefLoading(false);
    }
  };

  const handleAutocompleteChange = (name, newValue) => {
    if (name === 'warehouse') {
      setFormData((prev) => ({
        ...prev,
        warehouse: newValue,
        toWarehouse: prev.toWarehouse?.id === newValue?.id ? null : prev.toWarehouse,
        orders: [{ productId: null, itemName: '', unit: null, stock: 0, quantity: 0 }],
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
      const updatedOrders = [...prevData.orders];

      if (product) {
        const defaultUnit = units.find((u) => u.id === product.unitId) || null;

        updatedOrders[index] = {
          ...updatedOrders[index],
          productId: product.id,
          itemName: product.name,
          unit: defaultUnit,
          stock: product.quantity ?? 0,
          quantity: 1,
        };
      } else {
        updatedOrders[index] = {
          ...updatedOrders[index],
          productId: null,
          itemName: '',
          unit: null,
          stock: 0,
          quantity: 0,
        };
      }

      return { ...prevData, orders: updatedOrders };
    });
  };

  const handleUnitChange = (index, newUnit) => {
    setFormData((prevData) => {
      const updatedOrders = [...prevData.orders];
      updatedOrders[index] = {
        ...updatedOrders[index],
        unit: newUnit,
      };
      return { ...prevData, orders: updatedOrders };
    });
  };

  const handleOrderChange = (index, field, value) => {
    setFormData((prevData) => {
      const updatedOrders = [...prevData.orders];
      updatedOrders[index] = {
        ...updatedOrders[index],
        [field]: value,
      };
      return { ...prevData, orders: updatedOrders };
    });
  };

  const handleAddItem = () => {
    setFormData((prevData) => {
      const updatedOrders = [
        ...prevData.orders,
        { productId: null, itemName: '', unit: null, stock: 0, quantity: 0 },
      ];
      return { ...prevData, orders: updatedOrders };
    });
  };

  const handleDeleteItem = (index) => {
    setFormData((prevData) => {
      const updatedOrders = prevData.orders.filter((_, i) => i !== index);
      return { ...prevData, orders: updatedOrders };
    });
  };

  const validateForm = () => {
    const newErrors = {};

    if (!formData.requester) newErrors.requester = t('Validation.Required');
    if (!formData.warehouse) newErrors.warehouse = t('Validation.Required');
    if (!formData.date) newErrors.date = t('Validation.Required');

    if (formData.type === STOCK_OUT_TYPES.USAGE && !formData.department) {
      newErrors.department = t('Validation.Required');
    }
    if (formData.type === STOCK_OUT_TYPES.TRANSFER && !formData.toWarehouse) {
      newErrors.toWarehouse = t('Validation.Required');
    }

    if (!formData.orders.length) {
      newErrors.orders = t('Validation.AtLeastOneItem');
    } else {
      formData.orders.forEach((order, index) => {
        if (!order.productId) newErrors[`product_${index}`] = t('Validation.Required');
        if (!order.unit) newErrors[`unit_${index}`] = t('Validation.Required');

        const qty = Number(order.quantity);
        const stock = Number(order.stock);

        if (!qty || qty <= 0) {
          newErrors[`quantity_${index}`] = t('Validation.GreaterThanZero');
        }

        if (order.productId && qty > stock) {
          newErrors[`quantity_${index}`] = `${t('Validation.ExceedStock')} (Max: ${stock})`;
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
      stockOutDate: formData.date,
      type: formData.type,
      requesterId: formData.requester?.id,
      warehouseId: formData.warehouse?.id,
      departmentId: formData.type === STOCK_OUT_TYPES.USAGE ? formData.department?.id : null,
      toWarehouseId: formData.type === STOCK_OUT_TYPES.TRANSFER ? formData.toWarehouse?.id : null,
      referenceId: formData.selectedReference?.code,
      note: formData.note,
      isActivated: true,
      items: formData.orders
        .filter((o) => o.productId)
        .map((o) => ({
          productId: o.productId,
          quantity: Number(o.quantity),
        })),
    };

    setLoading(true);
    try {
      const success = await addStockOut(payload);
      if (success) {
        setToast({ open: true, type: 'success', message: t('Message.Success') });
        setTimeout(() => {
          router('/inventory/stock-out/list');
        }, 1500);
      } else {
        setToast({ open: true, type: 'error', message: t('Message.Error') || 'Failed to create stock out.' });
      }
    } catch (error) {
      console.error(error);
      setToast({ open: true, type: 'error', message: error.message || t('Message.Error') || 'An error occurred.' });
    } finally {
      setLoading(false);
    }
  };

  const parsedDate = isValid(new Date(formData.date)) ? new Date(formData.date) : new Date();
  const formattedDate = format(parsedDate, 'dd/MM/yyyy');

  const renderDestinationField = () => {
    if (formData.type === STOCK_OUT_TYPES.USAGE) {
      return (
        <Grid item size={{ xs: 12, sm: 6 }}>
          <CustomFormLabel>{t('Entity.Department')}</CustomFormLabel>
          <Autocomplete
            fullWidth
            options={departments || []}
            getOptionLabel={(option) => option.name}
            value={formData.department}
            onChange={(e, v) => handleAutocompleteChange('department', v)}
            renderInput={(params) => (
              <CustomTextField
                {...params}
                placeholder={t('Placeholder.Select')}
                error={!!errors.department}
                helperText={errors.department}
              />
            )}
          />
        </Grid>
      );
    }

    if (formData.type === STOCK_OUT_TYPES.TRANSFER) {
      return (
        <Grid item size={{ xs: 12, sm: 6 }}>
          <CustomFormLabel>{t('Field.ToWarehouse')}</CustomFormLabel>
          <Autocomplete
            fullWidth
            options={(warehouses || []).filter((w) => w.id !== formData.warehouse?.id)}
            getOptionLabel={(option) => option.name}
            value={formData.toWarehouse}
            onChange={(e, v) => handleAutocompleteChange('toWarehouse', v)}
            renderInput={(params) => (
              <CustomTextField
                {...params}
                placeholder={t('Placeholder.Select')}
                error={!!errors.toWarehouse}
                helperText={errors.toWarehouse}
              />
            )}
          />
        </Grid>
      );
    }

    return null;
  };

  return (
    <>
      <form onSubmit={handleSubmit}>
        <Box>
          <Stack direction="row" spacing={2} justifyContent="space-between" mb={3}>
            <Typography variant="h5">{t('Menu.StockOut')}</Typography>
            <Box display="flex" gap={1}>
              <Button
                variant="outlined"
                color="error"
                onClick={() => router('/inventory/stock-out/list')}
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

          <Paper variant="outlined" sx={{ p: 2, mb: 4, bgcolor: 'grey.50' }}>
            <Box
              display="flex"
              flexDirection={{ xs: 'column', md: 'row' }}
              justifyContent="space-between"
              alignItems={{ xs: 'start', md: 'center' }}
              gap={2}
            >
              <FormControl component="fieldset">
                <CustomFormLabel sx={{ mt: 0, mb: 0.5 }}>{t('Field.Type')}</CustomFormLabel>
                <RadioGroup row value={formData.type} onChange={handleTypeChange}>
                  <FormControlLabel
                    value={STOCK_OUT_TYPES.USAGE}
                    control={<Radio />}
                    label={t('StockOutType.Usage')}
                  />
                  <FormControlLabel
                    value={STOCK_OUT_TYPES.TRANSFER}
                    control={<Radio />}
                    label={t('StockOutType.Transfer')}
                  />
                  <FormControlLabel
                    value={STOCK_OUT_TYPES.ADJUSTMENT}
                    control={<Radio />}
                    label={t('StockOutType.Adjustment')}
                  />
                </RadioGroup>
              </FormControl>

              <Fade in={formData.type === STOCK_OUT_TYPES.ADJUSTMENT} unmountOnExit>
                <Box sx={{ minWidth: { xs: '100%', md: 350 } }}>
                  <CustomFormLabel sx={{ mt: 0, mb: 0.5 }}>{t('Field.Reference')}</CustomFormLabel>
                  <Autocomplete
                    fullWidth
                    size="small"
                    loading={isRefLoading}
                    options={referenceOptions}
                    getOptionLabel={(option) => option.code || ''}
                    isOptionEqualToValue={(option, value) => option.id === value?.id}
                    value={formData.selectedReference}
                    onChange={(e, v) => handleReferenceSelect(v)}
                    noOptionsText={t('Message.NoData')}
                    renderInput={(params) => (
                      <CustomTextField
                        {...params}
                        placeholder={t('Placeholder.Select')}
                        sx={{ bgcolor: 'white' }}
                        InputProps={{
                          ...params.InputProps,
                          endAdornment: (
                            <React.Fragment>
                              {isRefLoading ? <CircularProgress color="inherit" size={20} /> : null}
                              {params.InputProps.endAdornment}
                            </React.Fragment>
                          ),
                        }}
                      />
                    )}
                  />
                </Box>
              </Fade>
            </Box>
          </Paper>

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
              <CustomFormLabel>{t('Field.Date')}</CustomFormLabel>
              <LocalizationProvider dateAdapter={AdapterDateFns}>
                <DatePicker
                  format="dd/MM/yyyy"
                  value={formData.date ? new Date(formData.date) : null}
                  onChange={(newValue) => {
                    if (!newValue) return;
                    setFormData((prev) => ({
                      ...prev,
                      date: format(newValue, 'yyyy-MM-dd'),
                    }));
                  }}
                  enableAccessibleFieldDOMStructure={false}
                  slots={{ textField: CustomTextField }}
                  slotProps={{
                    textField: { fullWidth: true, error: !!errors.date, helperText: errors.date },
                  }}
                  disableFuture
                />
              </LocalizationProvider>
            </Grid>
          </Grid>

          <Grid container spacing={3} mb={4}>
            <Grid item size={{ xs: 12, sm: 6 }}>
              <CustomFormLabel>{t('Field.FromWarehouse')}</CustomFormLabel>
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

            {renderDestinationField()}

            <Grid item size={{ xs: 12 }}>
              <CustomFormLabel>{t('Field.Note')}</CustomFormLabel>
              <CustomTextField
                fullWidth
                multiline
                rows={2}
                value={formData.note}
                onChange={(e) => setFormData({ ...formData, note: e.target.value })}
                placeholder={t('Placeholder.Note')}
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
            {errors.orders && (
              <Alert severity="error" sx={{ mb: 2 }}>
                {errors.orders}
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

                    <TableCell width="15%">
                      <Typography variant="h6" fontSize="14px">
                        {t('Menu.Unit')}
                      </Typography>
                    </TableCell>

                    <TableCell width="15%">
                      <Typography variant="h6" fontSize="14px">
                        {t('Field.OnHand')}
                      </Typography>
                    </TableCell>

                    <TableCell width="15%">
                      <Typography variant="h6" fontSize="14px">
                        {t('Field.Quantity')}
                      </Typography>
                    </TableCell>

                    <TableCell width="10%" align="center">
                      <Typography variant="h6" fontSize="14px">
                        {t('Field.Action')}
                      </Typography>
                    </TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {formData.orders.map((order, index) => (
                    <TableRow key={index}>
                      <TableCell sx={{ verticalAlign: 'top' }}>
                        <Autocomplete
                          options={getAvailableProductsForDropdown(index)}
                          getOptionLabel={(option) => option.name}
                          value={(products || []).find((p) => p.id === order.productId) || null}
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

                      <TableCell sx={{ verticalAlign: 'top' }}>
                        <Autocomplete
                          options={units || []}
                          getOptionLabel={(option) => option.name}
                          value={order.unit}
                          onChange={(e, value) => handleUnitChange(index, value)}
                          renderInput={(params) => (
                            <CustomTextField
                              {...params}
                              error={!!errors[`unit_${index}`]}
                              helperText={errors[`unit_${index}`]}
                            />
                          )}
                          fullWidth
                        />
                      </TableCell>

                      <TableCell>
                        <Chip
                          label={order.stock}
                          color={order.stock > 0 ? 'success' : 'error'}
                          size="small"
                          sx={{ mt: 0.5, fontWeight: 'bold' }}
                        />
                      </TableCell>

                      <TableCell sx={{ verticalAlign: 'top' }}>
                        <CustomTextField
                          type="number"
                          value={order.quantity}
                          error={!!errors[`quantity_${index}`]}
                          helperText={errors[`quantity_${index}`]}
                          onChange={(e) => handleOrderChange(index, 'quantity', e.target.value)}
                          fullWidth
                          inputProps={{ min: 1, max: order.stock }}
                        />
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
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          </Paper>

          <Snackbar
            open={toast.open}
            autoHideDuration={6000}
            onClose={() => setToast((prev) => ({ ...prev, open: false }))}
            anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
          >
            <Alert 
              onClose={() => setToast((prev) => ({ ...prev, open: false }))} 
              severity={toast.type} 
              sx={{ width: '100%' }}
            >
              {toast.message}
            </Alert>
          </Snackbar>
        </Box>
      </form>
    </>
  );
};

export default CreateStockOutApp;