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
  Fade,
} from '@mui/material';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { useNavigate, useParams } from 'react-router';
import { format, isValid } from 'date-fns';
import { IconPlus, IconTrash, IconSquareRoundedPlus } from '@tabler/icons-react';
import { vi, enUS } from 'date-fns/locale';
import { useTranslation } from 'react-i18next';

import CustomFormLabel from 'src/components/forms/theme-elements/CustomFormLabel';
import CustomTextField from 'src/components/forms/theme-elements/CustomTextField';

import { StockInContext } from '../../../../context/StockInContext';
import { useMasterData } from 'src/hooks/useMasterData';

const STOCK_IN_TYPES = {
  PURCHASE: 1,
  TRANSFER: 2,
  RETURN: 3,
  ADJUSTMENT: 4,
};

const EditStockInApp = () => {
  const { t, i18n } = useTranslation();
  const router = useNavigate();
  const { id } = useParams();
  const { getStockInById, updateStockIn, clearError, error } = useContext(StockInContext);

  const { warehouses, suppliers, products, units, requesters, departments, stockOuts } =
    useMasterData();

  const [showAlert, setShowAlert] = useState(false);
  const [openConfirm, setOpenConfirm] = useState(false);
  const [loading, setLoading] = useState(false);
  const [errors, setErrors] = useState({});

  const [formData, setFormData] = useState({
    id: 0,
    code: '',
    type: STOCK_IN_TYPES.PURCHASE,
    requester: null,
    warehouse: null,

    supplier: null,
    fromWarehouse: null,
    fromDepartment: null,
    selectedStockOut: null,
    note: '',

    status: 'PENDING',
    orders: [],
    subtotal: 0,
    grandTotal: 0,
    date: '',
  });

  const isReadOnly = formData.status !== 'PENDING';

  useEffect(() => {
    if (!products || products.length === 0) return;

    const loadStockIn = async () => {
      const data = await getStockInById(Number(id));
      if (!data) return;

      const orders =
        data.items?.map((item) => {
          const product = products.find((p) => p.id === item.productId);
          const defaultUnit = units.find((u) => u.id === product?.unitId) || null;

          return {
            productId: item.productId,
            itemName: product?.name || '',
            unit: defaultUnit,
            unitPrice: item.unitPrice,
            units: item.quantity,
            unitTotalPrice: item.unitPrice * item.quantity,
          };
        }) ?? [];

      const subtotal = orders.reduce((s, o) => s + o.unitTotalPrice, 0);
      const linkedStockOut = stockOuts?.find((s) => s.id === data.referenceId) || null;

      setFormData({
        id: data.id,
        code: data.code,
        type: data.type || STOCK_IN_TYPES.PURCHASE,
        requester: requesters.find((r) => r.id === data.requesterId) || null,
        warehouse: warehouses.find((w) => w.id === data.warehouseId) || null,
        supplier: suppliers.find((s) => s.id === data.supplierId) || null,
        fromWarehouse: warehouses.find((w) => w.id === data.fromWarehouseId) || null,
        fromDepartment: departments?.find((d) => d.id === data.fromDepartmentId) || null,
        selectedStockOut: linkedStockOut,
        note: data.note || '',
        status: data.status,
        orders,
        subtotal,
        grandTotal: subtotal,
        date: data.stockInDate,
      });
    };

    loadStockIn();
  }, [id, products, units, requesters, suppliers, warehouses, departments, stockOuts, getStockInById]);

  const handleStockOutSelect = (stockOut) => {
    if (isReadOnly) return;
    if (!stockOut) {
      setFormData((prev) => ({ ...prev, selectedStockOut: null }));
      return;
    }

    let autoFromWarehouse = formData.fromWarehouse;
    let autoFromDepartment = formData.fromDepartment;

    if (formData.type === STOCK_IN_TYPES.TRANSFER) {
      autoFromWarehouse = warehouses.find((w) => w.id === stockOut.warehouseId) || null;
    } else if (formData.type === STOCK_IN_TYPES.RETURN) {
      autoFromDepartment = departments?.find((d) => d.id === stockOut.departmentId) || null;
    }

    setFormData((prev) => ({
      ...prev,
      selectedStockOut: stockOut,
      fromWarehouse: autoFromWarehouse,
      fromDepartment: autoFromDepartment,
    }));
  };

  const calculateTotals = (orders) => {
    if (formData.type !== STOCK_IN_TYPES.PURCHASE) return { subtotal: 0, grandTotal: 0 };
    let subtotal = 0;
    orders.forEach((o) => {
      const total = (Number(o.units) || 0) * (Number(o.unitPrice) || 0);
      o.unitTotalPrice = total;
      subtotal += total;
    });
    return { subtotal, grandTotal: subtotal };
  };

  const getAvailableProducts = (currentIndex) => {
    const currentProducts = products || [];
    const selectedIds = formData.orders
      .filter((_, i) => i !== currentIndex)
      .map((o) => o.productId)
      .filter(Boolean);
    return currentProducts.filter((p) => !selectedIds.includes(p.id));
  };

  const handleAutocompleteChange = (name, value) => {
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleProductSelect = (index, product) => {
    setFormData((prev) => {
      const orders = [...prev.orders];
      if (product) {
        const defaultUnit = units.find((u) => u.id === product.unitId) || null;
        orders[index] = {
          ...orders[index],
          productId: product.id,
          itemName: product.name,
          unit: defaultUnit,
          unitPrice: formData.type === STOCK_IN_TYPES.PURCHASE ? product.purchasePrice || 0 : 0,
        };
      } else {
        orders[index] = {
          ...orders[index],
          productId: null,
          itemName: '',
          unit: null,
          unitPrice: '',
          units: '',
          unitTotalPrice: 0,
        };
      }
      return { ...prev, orders, ...calculateTotals(orders) };
    });
  };

  const handleUnitChange = (index, unit) => {
    setFormData((prev) => {
      const orders = [...prev.orders];
      orders[index] = { ...orders[index], unit };
      return { ...prev, orders };
    });
  };

  const handleOrderChange = (index, field, value) => {
    setFormData((prev) => {
      const orders = [...prev.orders];
      orders[index] = { ...orders[index], [field]: value };
      return { ...prev, orders, ...calculateTotals(orders) };
    });
  };

  const handleAddItem = () => {
    setFormData((prev) => {
      const orders = [
        ...prev.orders,
        { productId: null, itemName: '', unit: null, unitPrice: '', units: '', unitTotalPrice: 0 },
      ];
      return { ...prev, orders, ...calculateTotals(orders) };
    });
  };

  const handleDeleteItem = (index) => {
    setFormData((prev) => {
      const orders = prev.orders.filter((_, i) => i !== index);
      return { ...prev, orders, ...calculateTotals(orders) };
    });
  };

  const validateForm = () => {
    const errs = {};
    if (!formData.requester) errs.requester = t('Placeholder.Select');
    if (!formData.warehouse) errs.warehouse = t('Placeholder.Select');
    if (formData.type === STOCK_IN_TYPES.PURCHASE && !formData.supplier) errs.supplier = t('Placeholder.Select');
    if (formData.type === STOCK_IN_TYPES.TRANSFER && !formData.fromWarehouse) errs.fromWarehouse = t('Placeholder.Select');
    if (formData.type === STOCK_IN_TYPES.RETURN && !formData.fromDepartment) errs.fromDepartment = t('Placeholder.Select');
    
    if (!formData.orders.length) errs.orders = t('Message.Error');
    formData.orders.forEach((o, i) => {
      if (!o.productId) errs[`product_${i}`] = t('Placeholder.Select');
      if (!o.unit) errs[`unit_${i}`] = t('Placeholder.Select');
      if (!o.units || Number(o.units) <= 0) errs[`units_${i}`] = t('Message.Error');
    });

    setErrors(errs);
    return Object.keys(errs).length === 0;
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!validateForm()) return;
    setOpenConfirm(true);
  };

  const handleConfirmSave = async () => {
    const payload = {
      id: formData.id,
      stockInDate: formData.date,
      type: formData.type,
      warehouseId: formData.warehouse.id,
      requesterId: formData.requester.id,
      supplierId: formData.supplier?.id || null,
      fromWarehouseId: formData.fromWarehouse?.id || null,
      fromDepartmentId: formData.fromDepartment?.id || null,
      referenceId: formData.selectedStockOut?.code || null,
      note: formData.note || null,
      isActivated: true,
      items: formData.orders.map((o) => ({
        productId: o.productId,
        quantity: Number(o.units),
        unitPrice: Number(o.unitPrice),
      })),
    };

    setLoading(true);
    const success = await updateStockIn(payload);
    setLoading(false);

    if (success) {
      setOpenConfirm(false);
      setShowAlert(true);
      setTimeout(() => router('/inventory/stock-in/list'), 800);
    }
  };

  const parsedDate = isValid(new Date(formData.date)) ? new Date(formData.date) : new Date();
  const isVietnamese = i18n.language && i18n.language.startsWith('vi');
  const formattedDate = format(
    parsedDate,
    isVietnamese ? "EEEE, 'ngày' dd 'tháng' MM, yyyy" : 'EEEE, MMMM dd, yyyy',
    { locale: isVietnamese ? vi : enUS },
  );

  const getStatusLabel = (status) => {
    if (!status) return '';
    return t(`Status.${status.charAt(0).toUpperCase() + status.slice(1).toLowerCase()}`);
  };
  
  const getStatusColor = (status) => {
    switch (status) {
      case 'PENDING': return 'warning';
      case 'APPROVED':
      case 'RETURNED':
      case 'COMPLETED': return 'success';
      case 'CANCELLED':
      case 'REJECTED': return 'error';
      default: return 'default';
    }
  };

  const renderSourceField = () => {
    const commonSize = { xs: 12, sm: 6 };
    switch (formData.type) {
      case STOCK_IN_TYPES.PURCHASE:
        return (
          <Grid item size={commonSize}>
            <CustomFormLabel>{t('Entity.Supplier')}</CustomFormLabel>
            <Autocomplete
              fullWidth
              disabled={isReadOnly}
              options={suppliers}
              getOptionLabel={(o) => o.name}
              value={formData.supplier}
              onChange={(e, v) => handleAutocompleteChange('supplier', v)}
              renderInput={(p) => <CustomTextField {...p} error={!!errors.supplier} helperText={errors.supplier} />}
            />
          </Grid>
        );
      case STOCK_IN_TYPES.TRANSFER:
        return (
          <Grid item size={commonSize}>
            <CustomFormLabel>{t('Field.FromWarehouse')}</CustomFormLabel>
            <Autocomplete
              fullWidth
              disabled={isReadOnly}
              options={warehouses.filter((w) => w.id !== formData.warehouse?.id)}
              getOptionLabel={(o) => o.name}
              value={formData.fromWarehouse}
              onChange={(e, v) => handleAutocompleteChange('fromWarehouse', v)}
              renderInput={(p) => <CustomTextField {...p} error={!!errors.fromWarehouse} helperText={errors.fromWarehouse} />}
            />
          </Grid>
        );
      case STOCK_IN_TYPES.RETURN:
        return (
          <Grid item size={commonSize}>
            <CustomFormLabel>{t('Field.FromDepartment')}</CustomFormLabel>
            <Autocomplete
              fullWidth
              disabled={isReadOnly}
              options={departments || []}
              getOptionLabel={(o) => o.name}
              value={formData.fromDepartment}
              onChange={(e, v) => handleAutocompleteChange('fromDepartment', v)}
              renderInput={(p) => <CustomTextField {...p} error={!!errors.fromDepartment} helperText={errors.fromDepartment} />}
            />
          </Grid>
        );
      default:
        return null;
    }
  };

  return (
    <>
      <Dialog open={openConfirm} onClose={() => setOpenConfirm(false)}>
        <DialogTitle>{t('Action.Update')}</DialogTitle>
        <DialogContent>
          <Typography>{t('Message.ConfirmChange')}</Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenConfirm(false)}>{t('Action.Cancel')}</Button>
          <Button variant="contained" color="primary" onClick={handleConfirmSave} disabled={loading}>
            {loading ? t('Action.Save') : t('Action.Confirm')}
          </Button>
        </DialogActions>
      </Dialog>

      <form onSubmit={handleSubmit}>
        <Box>
          <Stack direction="row" spacing={2} justifyContent="space-between" mb={3}>
            <Typography variant="h5">
              {t('Menu.StockIn')} {formData.code ? `#${formData.code}` : `#${formData.id}`}
            </Typography>
            <Box display="flex" gap={1}>
              <Button variant="outlined" color="error" onClick={() => router('/inventory/stock-in/list')}>
                {t('Action.Cancel')}
              </Button>
              <Button type="submit" variant="contained" disabled={isReadOnly}>
                {t('Action.Save')}
              </Button>
            </Box>
          </Stack>
          <Divider sx={{ mb: 3 }} />

          <Stack direction="row" justifyContent="space-between" alignItems="center" mb={3}>
            <Box mt={1}>
              <Chip label={getStatusLabel(formData.status)} color={getStatusColor(formData.status)} variant="filled" />
            </Box>
            <Box textAlign="right">
              <CustomFormLabel>{t('Field.Date')}</CustomFormLabel>
              <Typography variant="body1">{formattedDate}</Typography>
            </Box>
          </Stack>

          <Paper variant="outlined" sx={{ p: 2, mb: 4, bgcolor: 'grey.100' }}>
            <Box display="flex" flexDirection={{ xs: 'column', md: 'row' }} justifyContent="space-between" alignItems={{ xs: 'start', md: 'center' }} gap={2}>
              <FormControl component="fieldset">
                <CustomFormLabel sx={{ mt: 0, mb: 1 }}>{t('Field.Type')}</CustomFormLabel>
                <RadioGroup row value={formData.type}>
                  <FormControlLabel value={STOCK_IN_TYPES.PURCHASE} control={<Radio />} label={t('StockInType.Purchase')} disabled />
                  <FormControlLabel value={STOCK_IN_TYPES.TRANSFER} control={<Radio />} label={t('StockInType.Transfer')} disabled />
                  <FormControlLabel value={STOCK_IN_TYPES.RETURN} control={<Radio />} label={t('StockInType.Return')} disabled />
                  <FormControlLabel value={STOCK_IN_TYPES.ADJUSTMENT} control={<Radio />} label={t('StockInType.Adjustment')} disabled />
                </RadioGroup>
              </FormControl>

              <Fade in={[STOCK_IN_TYPES.RETURN, STOCK_IN_TYPES.TRANSFER].includes(formData.type)} unmountOnExit>
                <Box sx={{ minWidth: { xs: '100%', md: 350 } }}>
                  <CustomFormLabel sx={{ mt: 0, mb: 0.5 }}>{t('Field.Reference')}</CustomFormLabel>
                  <Autocomplete
                    fullWidth size="small" disabled={isReadOnly}
                    options={stockOuts || []}
                    getOptionLabel={(option) => `${option.code} - ${option.requesterName}`}
                    value={formData.selectedStockOut}
                    onChange={(e, v) => handleStockOutSelect(v)}
                    renderInput={(params) => <CustomTextField {...params} placeholder="Tìm phiếu xuất kho..." sx={{ bgcolor: 'white' }} />}
                  />
                </Box>
              </Fade>
            </Box>
          </Paper>

          <Grid container spacing={3} mb={4}>
            <Grid item size={{ xs: 12, sm: 6 }}>
              <CustomFormLabel>{t('Entity.Requester')}</CustomFormLabel>
              <Autocomplete
                fullWidth disabled={isReadOnly}
                options={requesters}
                getOptionLabel={(o) => o.name}
                value={formData.requester}
                onChange={(e, v) => handleAutocompleteChange('requester', v)}
                renderInput={(p) => <CustomTextField {...p} error={!!errors.requester} helperText={errors.requester} />}
              />
            </Grid>
            <Grid item size={{ xs: 12, sm: 6 }}>
              <CustomFormLabel>{t('Field.ToWarehouse')}</CustomFormLabel>
              <Autocomplete
                fullWidth disabled={isReadOnly}
                options={warehouses}
                getOptionLabel={(o) => o.name}
                value={formData.warehouse}
                onChange={(e, v) => handleAutocompleteChange('warehouse', v)}
                renderInput={(p) => <CustomTextField {...p} error={!!errors.warehouse} helperText={errors.warehouse} />}
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
                    setFormData((prev) => ({ ...prev, date: format(newValue, 'yyyy-MM-dd') }));
                  }}
                  enableAccessibleFieldDOMStructure={false}
                  slots={{ textField: CustomTextField }}
                  slotProps={{ textField: { fullWidth: true, error: !!errors.date, helperText: errors.date } }}
                  disableFuture
                />
              </LocalizationProvider>
            </Grid>
            {renderSourceField()}

            <Grid item size={{ xs: 12 }}>
              <CustomFormLabel>{t('Field.Note')}</CustomFormLabel>
              <CustomTextField
                fullWidth multiline rows={2}
                disabled={isReadOnly}
                value={formData.note}
                onChange={(e) => setFormData({ ...formData, note: e.target.value })}
                placeholder={t('Placeholder.Note')}
              />
            </Grid>
          </Grid>

          <Stack direction="row" justifyContent="space-between" mb={2}>
            <Typography variant="h6">{t('Field.Items')}</Typography>
            {!isReadOnly && (
              <Button onClick={handleAddItem} startIcon={<IconPlus />}>{t('Action.Add')}</Button>
            )}
          </Stack>

          <Paper variant="outlined">
            <TableContainer>
              <Table>
                <TableHead>
                  <TableRow>
                    <TableCell width="30%">{t('Entity.Product')}</TableCell>
                    <TableCell width="15%">{t('Menu.Unit')}</TableCell>
                    <TableCell width="15%">{t('Field.Price')}</TableCell>
                    <TableCell width="15%">{t('Field.Quantity')}</TableCell>
                    <TableCell width="15%">{t('Field.Total')}</TableCell>
                    <TableCell width="10%">{t('Field.Action')}</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {formData.orders.map((order, index) => (
                    <TableRow key={index}>
                      <TableCell sx={{ verticalAlign: 'top' }}>
                        <Autocomplete
                          disabled={isReadOnly}
                          options={getAvailableProducts(index)}
                          getOptionLabel={(o) => o.name}
                          isOptionEqualToValue={(o, v) => o.id === v.id}
                          value={products.find((p) => p.id === order.productId) || null}
                          onChange={(e, v) => handleProductSelect(index, v)}
                          renderInput={(p) => <CustomTextField {...p} error={!!errors[`product_${index}`]} helperText={errors[`product_${index}`]} />}
                        />
                      </TableCell>
                      <TableCell sx={{ verticalAlign: 'top' }}>
                        <Autocomplete
                          disabled={isReadOnly}
                          options={units}
                          getOptionLabel={(o) => o.name}
                          isOptionEqualToValue={(o, v) => o.id === v.id}
                          value={units.find((u) => u.id === order.unit?.id) || null}
                          onChange={(e, v) => handleUnitChange(index, v)}
                          renderInput={(p) => <CustomTextField {...p} error={!!errors[`unit_${index}`]} helperText={errors[`unit_${index}`]} />}
                        />
                      </TableCell>
                      <TableCell sx={{ verticalAlign: 'top' }}>
                        <CustomTextField
                          type="number"
                          disabled={isReadOnly || formData.type !== STOCK_IN_TYPES.PURCHASE}
                          value={order.unitPrice}
                          onChange={(e) => handleOrderChange(index, 'unitPrice', e.target.value)}
                        />
                      </TableCell>
                      <TableCell sx={{ verticalAlign: 'top' }}>
                        <CustomTextField
                          type="number" disabled={isReadOnly}
                          value={order.units}
                          onChange={(e) => handleOrderChange(index, 'units', e.target.value)}
                          error={!!errors[`units_${index}`]}
                          helperText={errors[`units_${index}`]}
                        />
                      </TableCell>
                      <TableCell>
                        <Typography variant="body1" fontWeight={600}>
                          {formData.type === STOCK_IN_TYPES.PURCHASE ? order.unitTotalPrice.toLocaleString() : '-'}
                        </Typography>
                      </TableCell>
                      <TableCell>
                        {!isReadOnly && (
                          <IconButton onClick={() => handleDeleteItem(index)} color="error">
                            <IconTrash width={22} />
                          </IconButton>
                        )}
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          </Paper>

          {formData.type === STOCK_IN_TYPES.PURCHASE && (
            <Box p={3} bgcolor="primary.light" mt={3}>
              <Box display="flex" justifyContent="end" gap={3}>
                <Typography variant="body1" fontWeight={600}>{t('Field.Total')}:</Typography>
                <Typography variant="body1" fontWeight={600}>{formData.grandTotal.toLocaleString()}</Typography>
              </Box>
            </Box>
          )}
        </Box>
        <Snackbar
          open={!!error}
          autoHideDuration={4000}
          onClose={() => clearError()}
          anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
        >
          <Alert severity="error" onClose={() => clearError()}>
            {error?.response?.data?.message || t('Message.Error')}
          </Alert>
        </Snackbar>
      </form>
    </>
  );
};

export default EditStockInApp;