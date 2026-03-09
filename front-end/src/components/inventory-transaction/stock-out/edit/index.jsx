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
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Snackbar,
  Radio,
  RadioGroup,
  FormControlLabel,
  FormControl,
  CircularProgress,
} from '@mui/material';

import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';

import { DatePicker } from '@mui/x-date-pickers/DatePicker';

import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';

import { useNavigate, useParams } from 'react-router-dom';

import { format, isValid } from 'date-fns';

import { IconPlus, IconTrash, IconSquareRoundedPlus } from '@tabler/icons-react';

import CustomFormLabel from 'src/components/forms/theme-elements/CustomFormLabel';

import CustomTextField from 'src/components/forms/theme-elements/CustomTextField';

import { vi, enUS } from 'date-fns/locale';

import { useTranslation } from 'react-i18next';

import { StockOutContext } from '../../../../context/StockOutContext';

import { useMasterData, useProducts } from 'src/hooks/useMasterData';

const STOCK_OUT_TYPES = {
  USAGE: 1,

  TRANSFER: 2,

  ADJUSTMENT: 3,
};

const EditStockOutApp = () => {
  const { t, i18n } = useTranslation();

  const router = useNavigate();

  const { id } = useParams();

  const { getStockOutById, updateStockOut } = useContext(StockOutContext);

  const { warehouses, departments, units, requesters } = useMasterData();

  const [alertInfo, setAlertInfo] = useState({
    open: false,

    severity: 'success',

    message: '',
  });

  const [openConfirm, setOpenConfirm] = useState(false);

  const [loading, setLoading] = useState(false);

  const [dataLoading, setDataLoading] = useState(true);

  const [errors, setErrors] = useState({});

  const [formData, setFormData] = useState({
    id: 0,

    code: '',

    type: STOCK_OUT_TYPES.USAGE,

    requester: null,

    warehouse: null,

    toWarehouse: null,

    department: null,

    note: '',

    status: 'Pending',

    orders: [],

    date: new Date().toISOString().split('T')[0],
  });

  const { products } = useProducts('local', formData.warehouse?.id);

  useEffect(() => {
    const loadStockOut = async () => {
      setDataLoading(true);

      try {
        const data = await getStockOutById(Number(id));

        if (data) {
          const detailItems = data.items || data.stockOutDetails || [];

          setFormData({
            id: data.id,

            code: data.code,

            type: data.type,

            requester: requesters.find((r) => r.id === data.requesterId) || null,

            warehouse: warehouses.find((w) => w.id === data.warehouseId) || null,

            department: departments.find((d) => d.id === data.departmentId) || null,

            toWarehouse: warehouses.find((w) => w.id === data.toWarehouseId) || null,

            note: data.note || '',

            status: data.status,

            date: data.stockOutDate,

            orders: detailItems.map((item) => ({
              productId: item.productId,

              itemName: item.productName || '',

              unit: units.find((u) => u.id === item.unitId) || null,

              stock: 0,

              quantity: item.quantity,
            })),
          });
        }
      } catch (err) {
        setAlertInfo({ open: true, severity: 'error', message: t('Message.ErrorFetchingData') });
      } finally {
        setDataLoading(false);
      }
    };

    if (requesters.length > 0 && warehouses.length > 0) {
      loadStockOut();
    }
  }, [id, getStockOutById, warehouses, departments, requesters, units]);

  useEffect(() => {
    if (products.length > 0 && formData.orders.length > 0) {
      setFormData((prev) => ({
        ...prev,

        orders: prev.orders.map((order) => {
          const productInfo = products.find((p) => p.id === order.productId);

          return {
            ...order,

            itemName: productInfo ? productInfo.name : order.itemName,

            stock: productInfo ? productInfo.quantity : 0,

            unit: order.unit ? order.unit : units.find((u) => u.id === productInfo?.unitId) || null,
          };
        }),
      }));
    }
  }, [products, units]);

  const getAvailableProductsForDropdown = (currentIndex) => {
    const currentList = products || [];

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
    }));

    setErrors({});
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

      updatedOrders[index] = { ...updatedOrders[index], unit: newUnit };

      return { ...prevData, orders: updatedOrders };
    });
  };

  const handleOrderChange = (index, field, value) => {
    setFormData((prevData) => {
      const updatedOrders = [...prevData.orders];

      updatedOrders[index] = { ...updatedOrders[index], [field]: value };

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

    if (formData.orders.length === 0) {
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

  const handleSubmit = (e) => {
    e.preventDefault();

    if (!validateForm()) return;

    setOpenConfirm(true);
  };

  const getStatusLabel = (status) => {
    if (!status) return '';

    const key = status.charAt(0).toUpperCase() + status.slice(1).toLowerCase();

    return t(`Status.${key}`);
  };

  const getStatusColor = (status) => {
    switch (status) {
      case 'PENDING':
        return 'warning';

      case 'APPROVED':
        return 'success';

      case 'RETURNED':
        return 'success';

      case 'COMPLETED':
        return 'success';

      case 'CANCELLED':
        return 'error';

      case 'REJECTED':
        return 'error';

      default:
        return 'default';
    }
  };

  const handleConfirmSave = async () => {
    const payload = {
      id: formData.id,

      stockOutDate: formData.date,

      type: formData.type,

      requesterId: formData.requester?.id,

      warehouseId: formData.warehouse?.id,

      departmentId: formData.type === STOCK_OUT_TYPES.USAGE ? formData.department?.id : null,

      toWarehouseId: formData.type === STOCK_OUT_TYPES.TRANSFER ? formData.toWarehouse?.id : null,

      note: formData.note,

      isActivated: true,

      items: formData.orders.map((o) => ({
        productId: o.productId,

        quantity: Number(o.quantity),
      })),
    };

    setLoading(true);

    try {
      const success = await updateStockOut(payload);

      if (success) {
        setAlertInfo({ open: true, severity: 'success', message: t('Message.Success') });

        setOpenConfirm(false);

        setTimeout(() => router('/inventory/stock-out/list'), 1000);
      } else {
        setAlertInfo({ open: true, severity: 'error', message: t('Message.Error') });

        setOpenConfirm(false);
      }
    } catch (error) {
      setAlertInfo({
        open: true,

        severity: 'error',

        message: error.message || t('Message.Error'),
      });

      setOpenConfirm(false);
    } finally {
      setLoading(false);
    }
  };

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

  const displayDateFormat =
    i18n.language === 'vi' ? "EEEE, 'ngày' dd 'tháng' MM, yyyy" : 'EEEE, MMMM dd, yyyy';

  const formattedDate = isValid(new Date(formData.date))
    ? format(new Date(formData.date), displayDateFormat, {
        locale: i18n.language === 'vi' ? vi : enUS,
      })
    : '';

  if (dataLoading) {
    return (
      <Box display="flex" justifyContent="center" p={5}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <>
      <Dialog open={openConfirm} onClose={() => setOpenConfirm(false)}>
        <DialogTitle>{t('Action.Update')}</DialogTitle>

        <DialogContent>
          <Typography>
            {t('Message.ConfirmChange')}
          </Typography>
        </DialogContent>

        <DialogActions>
          <Button onClick={() => setOpenConfirm(false)}>{t('Action.Cancel')}</Button>

          <Button
            variant="contained"
            color="primary"
            onClick={handleConfirmSave}
            disabled={loading}
          >
            {loading ? t('Action.Saving') : t('Action.Confirm')}
          </Button>
        </DialogActions>
      </Dialog>

      <form onSubmit={handleSubmit}>
        <Box>
          <Stack direction="row" spacing={2} justifyContent="space-between" mb={3}>
            <Typography variant="h5">
              {t('Action.Edit')} {t('Menu.StockOut')} {formData.code ? `#${formData.code}` : ''}
            </Typography>

            <Box display="flex" gap={1}>
              <Button
                variant="outlined"
                color="error"
                onClick={() => router('/inventory/stock-out/list')}
              >
                {t('Action.Cancel')}
              </Button>

              <Button type="submit" variant="contained" color="primary" disabled={loading}>
                {t('Action.Save')}
              </Button>
            </Box>
          </Stack>

          <Divider />

          <Stack direction="row" justifyContent="space-between" alignItems="center" mb={3} mt={3}>
            <Box>
              <Box mt={1}>
                <Chip
                  label={getStatusLabel(formData.status)}
                  color={getStatusColor(formData.status)}
                  variant="filled"
                />
              </Box>
            </Box>

            <Box textAlign="right">
              <CustomFormLabel>{t('Field.Date')}</CustomFormLabel>

              <Typography variant="body1">{formattedDate}</Typography>
            </Box>
          </Stack>

          <Paper variant="outlined" sx={{ p: 2, mb: 4, bgcolor: 'grey.50' }}>
            <FormControl component="fieldset" disabled>
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
          </Paper>

          <Grid container spacing={3} mb={4}>
            <Grid item size={{ xs: 12, sm: 6 }}>
              <CustomFormLabel>{t('Entity.Requester')}</CustomFormLabel>

              <Autocomplete
                fullWidth
                options={requesters}
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

              <LocalizationProvider
                dateAdapter={AdapterDateFns}
                adapterLocale={i18n.language === 'vi' ? vi : enUS}
              >
                <DatePicker
                  format={i18n.language === 'vi' ? 'dd/MM/yyyy' : 'MM/dd/yyyy'}
                  value={formData.date ? new Date(formData.date) : null}
                  enableAccessibleFieldDOMStructure={false}
                  onChange={(newValue) => {
                    if (!newValue) return;

                    setFormData((prev) => ({
                      ...prev,

                      date: format(newValue, 'yyyy-MM-dd'),
                    }));
                  }}
                  slots={{ textField: CustomTextField }}
                  slotProps={{
                    textField: { fullWidth: true, error: !!errors.date, helperText: errors.date },
                  }}
                />
              </LocalizationProvider>
            </Grid>
          </Grid>

          <Grid container spacing={3} mb={4}>
            <Grid item size={{ xs: 12, sm: 6 }}>
              <CustomFormLabel>{t('Field.FromWarehouse')}</CustomFormLabel>

              <Autocomplete
                fullWidth
                options={warehouses}
                getOptionLabel={(option) => option.name}
                value={formData.warehouse}
                onChange={(e, v) => handleAutocompleteChange('warehouse', v)}
                renderInput={(params) => (
                  <CustomTextField
                    {...params}
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
                  {formData.orders.map((order, index) => {
                    const currentProduct = (products || []).find((p) => p.id === order.productId);
                    const filteredUnits = currentProduct
                      ? units.filter((u) => u.id === currentProduct.unitId)
                      : units;

                    return (
                      <TableRow key={index}>
                        <TableCell sx={{ verticalAlign: 'top' }}>
                          <Autocomplete
                            options={getAvailableProductsForDropdown(index)}
                            getOptionLabel={(option) => option.name}
                            value={
                              currentProduct ||
                              (order.productId
                                ? { id: order.productId, name: order.itemName }
                                : null)
                            }
                            isOptionEqualToValue={(option, value) => option.id === value?.id}
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
                            options={filteredUnits || []}
                            getOptionLabel={(option) => option.name}
                            value={order.unit}
                            isOptionEqualToValue={(option, value) => option.id === value?.id}
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
                    );
                  })}
                </TableBody>
              </Table>
            </TableContainer>
          </Paper>

          <Snackbar
            open={alertInfo.open}
            autoHideDuration={4000}
            onClose={() => setAlertInfo({ ...alertInfo, open: false })}
            anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
          >
            <Alert
              severity={alertInfo.severity}
              onClose={() => setAlertInfo({ ...alertInfo, open: false })}
              sx={{ width: '100%' }}
            >
              {alertInfo.message}
            </Alert>
          </Snackbar>
        </Box>
      </form>
    </>
  );
};

export default EditStockOutApp;
